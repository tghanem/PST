using pst.core;
using pst.interfaces.messaging;
using pst.utilities;

namespace pst
{
    public class MessageStore
    {
        private readonly IPropertyContextBasedReadOnlyComponent propertyContextBasedReadOnlyComponent;

        internal MessageStore(IPropertyContextBasedReadOnlyComponent propertyContextBasedReadOnlyComponent)
        {
            this.propertyContextBasedReadOnlyComponent = propertyContextBasedReadOnlyComponent;
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                propertyContextBasedReadOnlyComponent.GetProperty(
                    new NumericalTaggedPropertyPath(NodePath.OfValue(Globals.NID_MESSAGE_STORE), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                propertyContextBasedReadOnlyComponent.GetProperty(
                    new StringTaggedPropertyPath(NodePath.OfValue(Globals.NID_MESSAGE_STORE), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                propertyContextBasedReadOnlyComponent.GetProperty(
                    new TaggedPropertyPath(NodePath.OfValue(Globals.NID_MESSAGE_STORE), propertyTag));
        }
    }
}
