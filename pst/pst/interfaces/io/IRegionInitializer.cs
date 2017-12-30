using pst.encodables.ndb;

namespace pst.interfaces.io
{
    interface IRegionInitializer<TType>
    {
        void Initialize(IB regionOffset, TType data);
    }
}