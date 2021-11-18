using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public class MediaManager
    {
        public MediaManager(ILogger<MediaManager> logger)
        {

        }
        public static IMetadata GetMetadataFor(String videoFilePath)
        {
            return new DefaultMetadataImpl(videoFilePath);
        }
    }
}
