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
            String toVideo)
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
    }
}
