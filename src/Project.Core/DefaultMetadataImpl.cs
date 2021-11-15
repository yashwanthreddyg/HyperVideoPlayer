using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    internal class DefaultMetadataImpl : IMetadata
    {
        public DefaultMetadataImpl(String metadataFileName)
        {

        }

        public Guid AddMediaLink(MediaLink link)
        {
            throw new NotImplementedException();
        }

        public List<Box> GetBoxesForFrame(uint frameNumber)
        {
            throw new NotImplementedException();
        }

        public MediaLink GetMediaLink(Guid linkId)
        {
            throw new NotImplementedException();
        }

        public List<MediaLink> GetMediaLinks()
        {
            throw new NotImplementedException();
        }

        public void RemoveMediaLink(Guid linkId)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
