using pst.encodables.ndb;
using pst.interfaces;
using System.Collections.Generic;
using System.Text;

namespace pst
{
    public class Folder
    {
        private readonly IFactory<NID, Folder[]> subFoldersFactory;

        private readonly Dictionary<PropertyId, PropertyValue> properties;
        private readonly NID nodeId;

        internal Folder(
            Dictionary<PropertyId, PropertyValue> properties,
            IFactory<NID, Folder[]> subFoldersFactory,
            NID nodeId)
        {
            this.subFoldersFactory = subFoldersFactory;
            this.properties = properties;
            this.nodeId = nodeId;
        }

        public Folder[] GetSubFolders()
        {
            return subFoldersFactory.Create(nodeId);
        }

        public string DisplayName
        {
            get
            {
                var propertyId = new PropertyId(0x3001);

                if (!properties.ContainsKey(propertyId))
                    return null;

                return Encoding.Unicode.GetString(properties[propertyId].Value);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
