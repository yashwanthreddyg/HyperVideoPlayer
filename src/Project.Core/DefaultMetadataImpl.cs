using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Core
{
    internal class DefaultMetadataImpl : IMetadata
    {
        List<MediaLink> _links;

        string _videoPath;
        string _audioFileName;
        string _audioPath;
        string _metadataFilePath;
        IDictionary<uint, ISet<Guid>> _frameGuidMap;

        Dictionary<Guid, MediaLink> _mediaLinkMap;

        public DefaultMetadataImpl(string videoFilePath)
        {
            _videoPath = videoFilePath;
            string _videoName = Path.GetFileName(videoFilePath);
            _audioFileName = _videoName + ".wav";
            _audioPath = Path.Combine(videoFilePath, _audioFileName);
            _metadataFilePath = Path.Combine(videoFilePath, _videoName + ".hpvd");
            _mediaLinkMap = new Dictionary<Guid, MediaLink>();
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

            _frameGuidMap = new Dictionary<uint, ISet<Guid>>();

            foreach (MediaLink link in _links)
            {
                for (uint i = link.FromFrame; i <= link.InitialHeight; i++)
                {
                    if (!_frameGuidMap.ContainsKey(i))
                    {
                        _frameGuidMap.Add(i, new HashSet<Guid>());
                    }
                    _frameGuidMap[i].Add(link.Id);
                }
            }
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

        public List<Box> GetBoxesForFrame(uint frameNumber)
        {
            List<Box> ret = new List<Box>();
            ISet<Guid> linksInCurrentFrame = _frameGuidMap[frameNumber];
            if (linksInCurrentFrame == null)
                return ret;
            foreach (Guid id in linksInCurrentFrame)
            {
                MediaLink link = _mediaLinkMap[id];
                uint diffInTargetX = link.ToX - link.FromX;
                uint diffInTargetY = link.ToY - link.FromY;
                uint diffInTargetHeight = link.FinalHeight - link.InitialHeight;
                uint diffInTargetWidth = link.FinalWidth - link.InitialWidth;

                uint diffInOriginalFrames = link.FromFrame - link.ToFrame;

                float progress = (float)(frameNumber - link.FromFrame) / diffInOriginalFrames;

                uint finalX = (uint)(link.FromX  + diffInTargetX * progress);
                uint finalY = (uint)(link.FromY + diffInTargetY * progress);
                uint finalHeight = (uint)(link.InitialHeight + diffInTargetHeight * progress);
                uint finalWidth = (uint)(link.InitialWidth + diffInTargetWidth * progress);
                Box box = new Box(finalX, finalY, finalHeight, finalWidth, link);
                ret.Add(box);
            }
            return ret;
        }

        public MediaLink GetMediaLink(Guid linkId)
        {
            return _mediaLinkMap[linkId];
        }

        public IReadOnlyCollection<MediaLink> GetMediaLinks()
        {
            return _links.AsReadOnly();
        }

        public void RemoveMediaLink(Guid linkId)
        {
            _links.Remove(_mediaLinkMap[linkId]);
            MediaLink link = _mediaLinkMap[linkId];
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
