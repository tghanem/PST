using pst.interfaces.ltp.tc;
using pst.encodables.ltp.tc;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces;
using pst.encodables.ltp.bth;
using System.Linq;

namespace pst.impl.ltp.tc
{
    class RowIndexLoader : IRowIndexLoader
    {
        private readonly IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter;
        private readonly IBTreeOnHeapLeafKeysEnumerator bthLeafKeysEnumerator;
        private readonly IDecoder<TCINFO> tcinfoDecoder;

        public RowIndexLoader(
            IConverter<DataRecord, TCROWID> dataRecordToTCROWIDConverter,
            IBTreeOnHeapLeafKeysEnumerator bthLeafKeysEnumerator,
            IDecoder<TCINFO> tcinfoDecoder)
        {
            this.dataRecordToTCROWIDConverter = dataRecordToTCROWIDConverter;
            this.bthLeafKeysEnumerator = bthLeafKeysEnumerator;
            this.tcinfoDecoder = tcinfoDecoder;
        }

        public TCROWID[] Load(HeapOnNode heapOnNode)
        {
            var tcinfo =
                tcinfoDecoder.Decode(heapOnNode.Root);
            
            var dataRecords =
                bthLeafKeysEnumerator.Enumerate(
                    heapOnNode.ChangeRoot(tcinfo.RowIndex));

            return
                dataRecords
                .Select(dataRecordToTCROWIDConverter.Convert)
                .ToArray();
        }
    }
}
