using System;
using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.maps;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.rawallocation;
using pst.utilities;

namespace pst.impl.rawallocation
{
    class AMapBasedAllocationFinder : IAllocationFinder
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<AMap> amapDecoder;

        public AMapBasedAllocationFinder(IDataReader dataReader, IDecoder<AMap> amapDecoder)
        {
            this.dataReader = dataReader;
            this.amapDecoder = amapDecoder;
        }

        public Maybe<AllocationInfo> Find(int sizeOfDataInBytes)
        {
            return Find(IB.OfValue(0x4400), sizeOfDataInBytes);
        }

        public Maybe<AllocationInfo> Find(IB mapOffset, int sizeOfDataInBytes)
        {
            var numberOfConsequtiveBitsToFind = Convert.ToInt32(Math.Ceiling(sizeOfDataInBytes / 64.0));

            return SearchAMap(mapOffset, numberOfConsequtiveBitsToFind);
        }

        private Maybe<AllocationInfo> SearchAMap(IB mapOffset, int numberOfContiguousBitsToFind)
        {
            var map = amapDecoder.Decode(dataReader.Read(0x4400, 512));

            var bitIndex = SearchAMap(map, numberOfContiguousBitsToFind);

            if (bitIndex.HasValue)
            {
                return new AllocationInfo(mapOffset, bitIndex.Value, bitIndex.Value + numberOfContiguousBitsToFind);
            }

            var nextMapOffset = mapOffset.Add(496 * 8 * 64);

            if (!dataReader.CanRead(nextMapOffset.Value))
            {
                return Maybe<AllocationInfo>.NoValue();
            }

            return SearchAMap(nextMapOffset, numberOfContiguousBitsToFind);
        }

        private Maybe<int> SearchAMap(AMap map, int numberOfContiguousBitsToFind)
        {
            var bits = map.Data.Value.ToBits();

            for (var i = 0; i < bits.Length; i++)
            {
                if (bits[i] == 0)
                {
                    var found = true;

                    for (var j = 0; j < numberOfContiguousBitsToFind; j++)
                    {
                        if (bits[i + j] == 1)
                        {
                            found = false;
                            i = i + j;
                            break;
                        }
                    }

                    if (found)
                    {
                        return Maybe<int>.OfValue(i);
                    }
                }
            }

            return Maybe<int>.NoValue();
        }
    }
}
