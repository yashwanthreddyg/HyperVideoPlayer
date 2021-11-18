using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public class MediaManager
    {
        ILogger _logger;
        Func<Bitmap, object> _bitmapToBitmapImageConverter;
        public MediaManager(ILogger<MediaManager> logger, Func<Bitmap, object> bitmapToBitmapImageConverter)
        {
            this._bitmapToBitmapImageConverter = bitmapToBitmapImageConverter;
            this._logger = logger;
        }
        public IMetadata GetMetadataFor(string videoFilePath)
        {
            return new DefaultMetadataImpl(_logger, videoFilePath, _bitmapToBitmapImageConverter);
        }
    }
}
