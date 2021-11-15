using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Models
{
    /// <summary>
    /// Represents the Link between videos as stored in the metadata file
    /// This must later be interpreter per frame to get the actual box redered on the video
    /// </summary>
    public class MediaLink
    {
        Guid Id { get; set; }
        uint FromFrame { get; set; }
        uint ToFrame { get; set; }
        uint FromX { get; set; }
        uint FromY { get; set; }
        uint ToX { get; set; }
        uint ToY { get; set; }
        String ToVideo { get; set; }
        uint InitialHeight { get; set; }
        uint InitialWidth { get; set; }
        uint FinalHeight { get; set; }
        uint FinalWidth { get; set; }
    }
}
