using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextBasedPropertyReader<TRowId>
    {
        Maybe<PropertyValue> Read(NID[] nodePath, TRowId rowId, PropertyTag propertyTag);
    }
}