using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces.blockallocation.datatree;
using pst.interfaces.ltp.hn;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeGenerator : IHeapOnNodeGenerator
    {
        private readonly IHeapOnNodeEncoder encoder;
        private readonly IDataTreeAllocator dataTreeAllocator;

        private readonly List<ExternalDataBlockForHeapOnNode> externalDataBlocks;

        private ExternalDataBlockForHeapOnNode currentExternalDataBlock;

        public HeapOnNodeGenerator(IHeapOnNodeEncoder encoder, IDataTreeAllocator dataTreeAllocator)
        {
            this.encoder = encoder;
            this.dataTreeAllocator = dataTreeAllocator;

            externalDataBlocks = new List<ExternalDataBlockForHeapOnNode>();
            currentExternalDataBlock = new ExternalDataBlockForHeapOnNode(blockIndex: 0);
        }

        public HID AllocateItem(BinaryData value, bool isUserRoot = false)
        {
            return AllocateItemToCurrentExternalDataBlock(value, isUserRoot);
        }

        public BREF Commit(int clientSignature)
        {
            var encodedExternalDataBlocks = encoder.Encode(externalDataBlocks.ToArray(), clientSignature);

            return dataTreeAllocator.Allocate(encodedExternalDataBlocks);
        }

        private HID AllocateItemToCurrentExternalDataBlock(BinaryData value, bool isUserRoot)
        {
            if (currentExternalDataBlock.FreeSpaceSize > value.Length + 2)
            {
                return currentExternalDataBlock.AddItem(value, isUserRoot);
            }

            externalDataBlocks.Add(currentExternalDataBlock);

            currentExternalDataBlock = new ExternalDataBlockForHeapOnNode(currentExternalDataBlock.BlockIndex + 1);

            return currentExternalDataBlock.AddItem(value, isUserRoot);
        }
    }
}
