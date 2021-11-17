using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Models
{
    /// <summary>
    /// Link as represented as a box while rendering a frame. This is interpreted from the MediaLink's properties
    /// </summary>
    public class Box
    {
        public Box(uint x, uint y, uint width, uint height, MediaLink link)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.MediaLink = link;
        }
        public uint X { get; }
        public uint Y { get; }
        public uint Width { get; }
        public uint Height { get; }
        public MediaLink MediaLink { get; }
    }
}
