using pst.encodables.ndb;
using System;

namespace pst.interfaces.io
{
    interface IStreamRegionUpdater<TType>
    {
        void UpdateRegion(IB regionOffset, Func<TType, TType> processRegion);
    }
}