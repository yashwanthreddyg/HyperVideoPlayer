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
        uint X { get; set; }
        uint Y { get; set; }
        uint Width { get; set; }
        uint Height { get; set; }
        MediaLink MediaLink { get; set; }
    }
}
