﻿using pst.core;
using pst.interfaces.ndb;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextBasedPropertyReader<TRowId>
    {
        Maybe<PropertyValue> Read(NodePath nodePath, TRowId rowId, PropertyTag propertyTag);
    }
}