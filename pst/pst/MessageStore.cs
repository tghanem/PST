using System.Collections.Generic;
using System.Text;

namespace pst
{
    public class MessageStore
    {
        private readonly IDictionary<PropertyId, PropertyValue> properties;

        internal MessageStore(IDictionary<PropertyId, PropertyValue> properties)
        {
            this.properties = properties;
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
    }
}
