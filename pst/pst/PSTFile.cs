using pst.encodables.ndb;
using pst.impl.decoders.ndb;
using pst.impl.decoders.primitives;
using pst.impl.io;
using pst.impl.ndb.bbt;
using pst.impl.ndb.nbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pst
{
    public class PSTFile
    {
        private readonly NodeBTree nodeBTree;
        private readonly BlockBTree blockBTree;

        public PSTFile(string filePath)
        {
            var fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);

            var dataReader = new StreamBasedDataReader(fileStream);

            var headerDecoder =
                new HeaderDecoder(
                    new Int32Decoder(),
                    new RootDecoder(
                        new Int32Decoder(),
                        new Int64Decoder(),
                        new BREFDecoder(
                             new BIDDecoder(),
                             new IBDecoder())),
                    new BIDDecoder(),
                    new NIDDecoder());

            var header =
                headerDecoder.Decode(dataReader.Read(IB.Zero, 564));

            nodeBTree =
                new NodeBTree(dataReader, header.Root.NBTRootPage);

            blockBTree =
                new BlockBTree(dataReader, header.Root.BBTRootPage);
        }

        public MessageStore GetMessageStore()
        {
            var lnbtEntry = nodeBTree.Find(new NID(0x21));

            var lbbtEntry = blockBTree.Find(lnbtEntry.DataBlockId);

            return null;
        }
    }
}
