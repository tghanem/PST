using pst.encodables.ltp.tc;
using pst.interfaces.ltp.tc;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ltp.tc
{
    class RowValuesExtractor : IRowValuesExtractor
    {
        public IReadOnlyDictionary<int, BinaryData> Extract(BinaryData rowData, TCOLDESC[] columnDescriptors, int cebOffset)
        {
            var ceb = ToBits(rowData.Take(cebOffset, rowData.Length - cebOffset));

            var values = new Dictionary<int, BinaryData>();

            for (int i = 0; i < columnDescriptors.Length; i++)
            {
                var iBit = columnDescriptors[i].CellExistenceBitmapIndex;

                if (ceb[iBit])
                {
                    var cellValue = rowData.Take(columnDescriptors[i].DataOffset, columnDescriptors[i].DataSize);

                    values.Add(columnDescriptors[i].Tag, cellValue);
                }
            }

            return values;
        }

        private bool[] ToBits(BinaryData ceb)
        {
            var result = new bool[ceb.Length * 8];

            for (var i = 0; i < ceb.Length; i++)
            {
                var b = ceb.Value[i];

                result[i * 8] = (b & 0x80) > 0;
                result[i * 8 + 1] = (b & 0x40) > 0;
                result[i * 8 + 2] = (b & 0x20) > 0;
                result[i * 8 + 3] = (b & 0x10) > 0;
                result[i * 8 + 4] = (b & 0x08) > 0;
                result[i * 8 + 5] = (b & 0x04) > 0;
                result[i * 8 + 6] = (b & 0x02) > 0;
                result[i * 8 + 7] = (b & 0x01) > 0;
            }

            return result;
        }
    }
}
