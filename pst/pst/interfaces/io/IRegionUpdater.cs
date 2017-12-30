using pst.encodables.ndb;
using System;

namespace pst.interfaces.io
{
    interface IRegionUpdater<TType>
    {
        void Update(IB regionOffset, Func<TType, TType> processRegion);
    }
}