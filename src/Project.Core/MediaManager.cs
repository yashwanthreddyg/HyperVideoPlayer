using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public class MediaManager
    {
        public static IMetadata GetMetadataFor(String videoFilePath)
        {
            return new DefaultMetadataImpl(videoFilePath);
        }
    }
}
