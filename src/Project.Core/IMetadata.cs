using Project.Core.Models;

namespace Project.Core
{
    public interface IMetadata
    {
        List<Box> GetBoxesForFrame(uint frameNumber);
        List<MediaLink> GetMediaLinks();
        MediaLink GetMediaLink(Guid linkId);
        Guid AddMediaLink(MediaLink link);
        void RemoveMediaLink(Guid linkId);
        void Save();
    }
}