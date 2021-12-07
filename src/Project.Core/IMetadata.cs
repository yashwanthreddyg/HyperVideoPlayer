using Project.Core.Models;
using System.Drawing;

namespace Project.Core
{
    public interface IMetadata
    {
        string GetPath();
        List<Box> GetBoxesForFrame(uint frameNumber);
        IReadOnlyCollection<MediaLink> GetMediaLinks();
        MediaLink GetMediaLink(Guid linkId);
        Guid AddMediaLink(MediaLink link);
        uint GetFrameCount();
        object GetBitmapImageForFrame(uint frameNumber);
        void RemoveMediaLink(Guid linkId);
        Uri GetAudioPath();
        void Save();
    }
}