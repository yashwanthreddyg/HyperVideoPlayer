using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public class MediaManager
    {
        IMetadata GetMetadataFor(String videoFileName)
        {
            return new DefaultMetadataImpl(videoFileName);
        }
    }
}
