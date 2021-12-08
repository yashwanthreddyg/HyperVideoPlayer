using Microsoft.Extensions.Logging;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Core
{
    internal class DefaultMetadataImpl : IMetadata
    {

        private readonly uint FRAME_WIDTH = 352;
        private readonly uint FRAME_HEIGHT = 288;

        List<MediaLink> _links;

        string _videoPath;
        string _videoName;
        string _audioFileName;
        string _audioPath;
        string _metadataFilePath;
        ILogger _logger;
        IDictionary<uint, ISet<Guid>> _frameGuidMap;
        IDictionary<uint, object> _frameBitmapImageMap;
        string[] _frameFilePaths;
        uint _frameCount = 0;
        Func<Bitmap, object> _bitmapToBitmapImageConverter;

        Dictionary<Guid, MediaLink> _mediaLinkMap;

        public DefaultMetadataImpl(ILogger logger, string videoFilePath, Func<Bitmap, object> bitmapToBitmapImageConverter)
        {
            _bitmapToBitmapImageConverter = bitmapToBitmapImageConverter;
            _videoPath = videoFilePath;
            _videoName = Path.GetFileName(videoFilePath);
            _audioFileName = _videoName + ".wav";
            _audioPath = Path.Combine(videoFilePath, _audioFileName);
            _metadataFilePath = Path.Combine(videoFilePath, _videoName + ".vdm");
            _mediaLinkMap = new Dictionary<Guid, MediaLink>();
            _frameBitmapImageMap = new Dictionary<uint, object>();
            _frameGuidMap = new Dictionary<uint, ISet<Guid>>();

            this._logger = logger;
            if (File.Exists(_metadataFilePath))
            {
                String jsonStr = File.ReadAllText(_metadataFilePath, Encoding.UTF8);
                _links = new List<MediaLink>();
                try
                {
                    _links = JsonSerializer.Deserialize<List<MediaLink>>(jsonStr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                _links = new List<MediaLink>();
            }

            foreach (MediaLink link in _links)
            {
                _mediaLinkMap.Add(link.Id, link);
            }

            foreach (MediaLink link in _links)
            {
                for (uint i = link.FromFrame; i <= link.ToFrame; i++)
                {
                    if (!_frameGuidMap.ContainsKey(i))
                    {
                        _frameGuidMap.Add(i, new HashSet<Guid>());
                    }
                    _frameGuidMap[i].Add(link.Id);
                }
            }

            // Get number files ending with .rgb in videoFilePath
            _frameFilePaths = Directory.GetFiles(videoFilePath, "*.rgb");
            _frameCount = (uint)_frameFilePaths.Length;

            // sort _frameFilePaths alphabetically
            Array.Sort(_frameFilePaths);
        }

        public Guid AddMediaLink(MediaLink link)
        {
            //todo: validate link
            link.Id = Guid.NewGuid();
            _links.Add(link);
            _mediaLinkMap.Add(link.Id, link);
            for (uint i = link.FromFrame; i <= link.ToFrame; i++)
            {
                if (!_frameGuidMap.ContainsKey(i))
                {
                    _frameGuidMap.Add(i, new HashSet<Guid>());
                }
                _frameGuidMap[i].Add(link.Id);
            }
            return link.Id;
        }

        public Uri GetAudioPath()
        {
            return new Uri(Path.GetFullPath(_audioPath));
        }

        public object GetBitmapImageForFrame(uint frameNumber)
        {
            if (_frameBitmapImageMap.ContainsKey(frameNumber))
            {
                _logger.LogTrace("Found bitmap in the cache. reusing it");
                return _frameBitmapImageMap[frameNumber];
            }

            _logger.LogTrace("Fetching bitmap from the disk");
            object bmpImg;
            Bitmap bmp;
            using (var streamReader = new System.IO.StreamReader(_frameFilePaths[frameNumber - 1]))
            using (var br = new BinaryReader(streamReader.BaseStream))
            {
                var bytes = br.ReadBytes((int)(3 * FRAME_WIDTH * FRAME_HEIGHT));
                bmp = new Bitmap((int)FRAME_WIDTH, (int)FRAME_HEIGHT);
                int ind = 0;
                for (int y = 0; y < FRAME_HEIGHT; y++)
                {
                    for (int x = 0; x < FRAME_WIDTH; x++)
                    {
                        byte r = bytes[ind];
                        byte g = bytes[ind + FRAME_HEIGHT * FRAME_WIDTH];
                        byte b = bytes[ind + FRAME_HEIGHT * FRAME_WIDTH * 2];

                        int pix = (int)(0xff000000 | ((r & 0xff) << 16) | ((g & 0xff) << 8) | (b & 0xff));
                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(pix));
                        ind++;
                    }
                }
            }
            bmpImg = _bitmapToBitmapImageConverter(bmp);
            _frameBitmapImageMap[frameNumber] = bmpImg;
            return bmpImg;
        }

        public List<Box> GetBoxesForFrame(uint frameNumber)
        {
            List<Box> ret = new List<Box>();
            
            if (!_frameGuidMap.ContainsKey(frameNumber))
                return new List<Box>();

            ISet<Guid> linksInCurrentFrame = _frameGuidMap[frameNumber];
            if (linksInCurrentFrame == null)
                return ret;
            foreach (Guid id in linksInCurrentFrame)
            {
                MediaLink link = _mediaLinkMap[id];
                uint diffInTargetX = link.ToX >= link.FromX ? (link.ToX - link.FromX) : (link.FromX - link.ToX);
                uint diffInTargetY = link.ToY >= link.FromY ? (link.ToY - link.FromY) : (link.FromY - link.ToY);
                int diffInTargetHeight = (int)(link.FinalHeight - link.InitialHeight);
                int diffInTargetWidth = (int)(link.FinalWidth - link.InitialWidth);

                uint diffInOriginalFrames = link.ToFrame - link.FromFrame;

                float progress = (float)(frameNumber - link.FromFrame) / diffInOriginalFrames;

                uint finalX = (uint)(link.FromX + diffInTargetX * progress);
                uint finalY = (uint)(link.FromY + diffInTargetY * progress);
                uint finalHeight = (uint)(link.InitialHeight + diffInTargetHeight * progress);
                uint finalWidth = (uint)(link.InitialWidth + diffInTargetWidth * progress);
                Box box = new Box(finalX, finalY, finalWidth, finalHeight, link);
                ret.Add(box);
            }
            return ret;
        }

        public uint GetFrameCount()
        {
            return _frameCount;
        }

        public MediaLink GetMediaLink(Guid linkId)
        {
            return _mediaLinkMap[linkId];
        }

        public IReadOnlyCollection<MediaLink> GetMediaLinks()
        {
            return _links.AsReadOnly();
        }

        public string GetPath()
        {
            return _videoPath;
        }

        public void RemoveMediaLink(Guid linkId)
        {
            try
            {
                _links.Remove(_mediaLinkMap[linkId]);
            }
            catch (Exception ex)
            {

            }
            if (!_mediaLinkMap.ContainsKey(linkId))
                return;
            MediaLink link = _mediaLinkMap[linkId];
            if (link != null)
                _mediaLinkMap.Remove(linkId);
            for (uint i = link.FromFrame; i <= link.ToFrame; i++)
            {
                _frameGuidMap[i].Remove(link.Id);
            }
            return;
        }

        public void Save()
        {
            string jsonString = JsonSerializer.Serialize(_links, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_metadataFilePath, jsonString, Encoding.UTF8);
        }
    }
}
