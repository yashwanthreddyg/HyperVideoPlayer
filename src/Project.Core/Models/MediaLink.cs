using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Models
{
    /// <summary>
    /// Represents the Link between videos as stored in the metadata file
    /// This must later be interpreted per frame to get the actual box rendered on the video
    /// </summary>
    public class MediaLink
    {
        public MediaLink(uint fromFrame, uint toFrame,
            uint fromX, uint toX,
            uint fromY, uint toY,
            uint initialHeight, uint finalHeight,
            uint initialWidth, uint finalWidth,
            String toVideo, uint linkedFrame, string linkName)
        {
            this.Id = Guid.NewGuid();
            this.FromFrame = fromFrame;
            this.ToFrame = toFrame;
            this.FromX = fromX;
            this.ToX = toX;
            this.FromY = fromY;
            this.ToY = toY;
            this.InitialWidth = initialWidth;
            this.FinalWidth = finalWidth;
            this.InitialHeight = initialHeight;
            this.FinalHeight = finalHeight;
            this.ToVideo = toVideo;
            this.LinkName = linkName;
            this.LinkedFrame = linkedFrame;
            
            TimeSpan time = TimeSpan.FromSeconds(FromFrame / 30.0);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            this.FrameTime = time.ToString(@"mm\:ss");
        }
        public Guid Id { get; internal set; }
        public uint FromFrame { get; }
        public uint ToFrame { get; }
        public uint FromX { get; }
        public uint FromY { get; }
        public uint ToX { get; }
        public uint ToY { get; }
        public uint InitialHeight { get; }
        public uint InitialWidth { get; }
        public uint FinalHeight { get; }
        public uint FinalWidth { get; }
        public String ToVideo { get; }
        public uint LinkedFrame { get; }
        public string LinkName { get; }
        public string FrameTime { get; }
    }
}
