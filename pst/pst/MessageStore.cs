using pst.impl.ltp.pc;
using System.Text;

namespace pst
{
    public class MessageStore
    {
        private readonly PropertyContext propertyContext;

        internal MessageStore(PropertyContext propertyContext)
        {
            this.propertyContext = propertyContext;
        }

        public string DisplayName
        {
            get
            {
                var propertyValue =
                    propertyContext.GetPropertyValue(new PropertyId(0x3001));

                if (propertyValue.HasNoValue)
                {
                    return null;
                }

                return Encoding.Unicode.GetString(propertyValue.Value.Value);
            }
        }
    }
}
