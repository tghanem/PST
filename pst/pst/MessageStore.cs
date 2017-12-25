using pst.core;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst
{
    public class MessageStore
    {
        private readonly IPropertyContextBasedComponent propertyContextBasedComponent;

        internal MessageStore(IPropertyContextBasedComponent propertyContextBasedComponent)
        {
            this.propertyContextBasedComponent = propertyContextBasedComponent;
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new NumericalTaggedPropertyPath(NodePath.OfValue(Constants.NID_MESSAGE_STORE), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new StringTaggedPropertyPath(NodePath.OfValue(Constants.NID_MESSAGE_STORE), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new TaggedPropertyPath(NodePath.OfValue(Constants.NID_MESSAGE_STORE), propertyTag));
        }
    }
}
