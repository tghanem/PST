using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.interfaces.blockallocation.subnodebtree;
using pst.utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.impl.blockallocation.subnodebtree
{
    class SubnodeBTreeAllocator : ISubnodeBTreeAllocator
    {
        private const int MaximumSizeOfSubnodeBTreeBlock = 8 * 1024 - 24;

        private readonly IEncoder<SLEntry> slEntryEncoder;
        private readonly IEncoder<SIEntry> siEntryEncoder;
        private readonly IDataTreeAllocator dataTreeAllocator;

        public SubnodeBTreeAllocator(
            IEncoder<SLEntry> slEntryEncoder,
            IEncoder<SIEntry> siEntryEncoder,
            IDataTreeAllocator dataTreeAllocator)
        {
            this.slEntryEncoder = slEntryEncoder;
            this.siEntryEncoder = siEntryEncoder;
            this.dataTreeAllocator = dataTreeAllocator;
        }

        public BID Allocate(SLEntry[] entriesForSubnodes)
        {
            var orderedEntriesForSubnodes = entriesForSubnodes.OrderBy(e => e.LocalSubnodeId).ToArray();

            var slices = orderedEntriesForSubnodes.Slice(24, MaximumSizeOfSubnodeBTreeBlock);

            if (slices.Length == 1)
            {
                return dataTreeAllocator.Allocate(new[] { EncodeSLBlock(slices[0]) });
            }

            var siEntries = new List<SIEntry>();

            foreach (var slice in slices)
            {
                var encodedSLBlock = EncodeSLBlock(slice);

                var slBlockId = dataTreeAllocator.Allocate(new[] { encodedSLBlock });

                siEntries.Add(new SIEntry(slice[0].LocalSubnodeId, slBlockId));
            }

            var encodedSIBlock = EncodeSIBlock(siEntries.ToArray());

            return dataTreeAllocator.Allocate(new[] { encodedSIBlock });
        }

        private BinaryData EncodeSIBlock(SIEntry[] blockEntries)
        {
            var generator = BinaryDataGenerator.New();

            generator.Append((byte)0x02);
            generator.Append((byte)0x01);
            generator.Append((short)blockEntries.Length);
            generator.Append(BinaryData.OfSize(4));

            Array.ForEach(blockEntries, e => generator = generator.Append(e, siEntryEncoder));

            return generator.GetData();
        }

        private BinaryData EncodeSLBlock(SLEntry[] blockEntries)
        {
            var generator = BinaryDataGenerator.New();

            generator.Append((byte)0x02);
            generator.Append((byte)0x00);
            generator.Append((short)blockEntries.Length);
            generator.Append(BinaryData.OfSize(4));

            Array.ForEach(blockEntries, e => generator = generator.Append(e, slEntryEncoder));

            return generator.GetData();
        }
    }
}
