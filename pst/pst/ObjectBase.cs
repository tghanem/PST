using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.utilities;

namespace pst
{
    public abstract class ObjectBase
    {
        private readonly NodePath nodePath;
        private readonly ObjectTypes objectType;
        private readonly IChangesTracker changesTracker;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;

        internal ObjectBase(
            NodePath nodePath,
            ObjectTypes objectType,
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.objectType = objectType;
            this.changesTracker = changesTracker;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(resolvedTag.Value);
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(resolvedTag.Value);
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                changesTracker.ReadProperty(
                    nodePath,
                    objectType, 
                    propertyTag,
                    () => propertyContextBasedPropertyReader.Read(nodePath.AllocatedIds, propertyTag));
        }
    }
}
