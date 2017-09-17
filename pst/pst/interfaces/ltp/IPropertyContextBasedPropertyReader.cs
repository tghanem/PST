using pst.core;
using pst.interfaces.ndb;

namespace pst.interfaces.ltp
{
    interface IPropertyContextBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(NodePath nodePath, PropertyTag propertyTag);
    }

    interface ITableContextBasedPropertyReader<TRowId>
    {
        Maybe<PropertyValue> ReadProperty(NodePath nodePath, TRowId rowId, PropertyTag propertyTag);
    }
}
