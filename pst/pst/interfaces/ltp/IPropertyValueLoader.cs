using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.utilities;

namespace pst.interfaces.ltp
{
    interface IPropertyValueLoader
    {
        PropertyValue Load(
            PropertyId id,
            PropertyType type,
            BinaryData encodedValue,
            IDataBlockReader<LBBTEntry> reader,
            IMapper<NID, SLEntry> nidToSLEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
