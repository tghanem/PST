using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.impl.ltp.bth
{
    class BTreeOnHeapGenerator<TKey, TValue> : IBTreeOnHeapGenerator<TKey, TValue> where TKey : IComparable<TKey>
    {
        private const int MaximumIndexOrLeafBlockSize = 3580;

        private readonly int keySize;
        private readonly int valueSize;

        private readonly IEncoder<HID> hidEncoder;
        private readonly IEncoder<TKey> keyEncoder;
        private readonly IEncoder<TValue> valueEncoder;

        public BTreeOnHeapGenerator(
            int keySize,
            int valueSize,
            IEncoder<HID> hidEncoder,
            IEncoder<TKey> keyEncoder,
            IEncoder<TValue> valueEncoder)
        {
            this.keySize = keySize;
            this.valueSize = valueSize;
            this.hidEncoder = hidEncoder;
            this.keyEncoder = keyEncoder;
            this.valueEncoder = valueEncoder;
        }

        public HID Generate(Tuple<TKey, TValue>[] dataRecords, IHeapOnNodeGenerator hnGenerator)
        {
            var orderedDataRecords = dataRecords.OrderBy(t => t.Item1).ToArray();

            var leafBlocks = orderedDataRecords.Slice(keySize + valueSize, MaximumIndexOrLeafBlockSize);

            if (leafBlocks.Length == 1)
            {
                var encodedBlock = EncodeLeafBlock(leafBlocks[0]);

                return hnGenerator.AllocateItem(encodedBlock, isUserRoot: true);
            }

            var hidForLeafBlocks = new List<Tuple<TKey, HID>>();

            foreach (var slice in leafBlocks)
            {
                var encodedLeafBlock = EncodeLeafBlock(slice);

                var leafBlockId = hnGenerator.AllocateItem(encodedLeafBlock);

                hidForLeafBlocks.Add(Tuple.Create(slice[0].Item1, leafBlockId));
            }

            return Generate(hidForLeafBlocks.ToArray(), hnGenerator);
        }

        private HID Generate(Tuple<TKey, HID>[] blockItems, IHeapOnNodeGenerator hnGenerator)
        {
            var indexBlocks = blockItems.Slice(keySize + 4, MaximumIndexOrLeafBlockSize);

            if (indexBlocks.Length == 1)
            {
                var encodedBlock = EncodeIndexBlock(indexBlocks[0]);

                return hnGenerator.AllocateItem(encodedBlock, isUserRoot: true);
            }

            var hidForIndexBlocks = new List<Tuple<TKey, HID>>();

            foreach (var slice in indexBlocks)
            {
                var encodedIndexBlock = EncodeIndexBlock(slice);

                var indexBlockId = hnGenerator.AllocateItem(encodedIndexBlock);

                hidForIndexBlocks.Add(Tuple.Create<TKey, HID>(slice[0].Item1, indexBlockId));
            }

            return Generate(hidForIndexBlocks.ToArray(), hnGenerator);
        }

        private BinaryData EncodeIndexBlock(Tuple<TKey, HID>[] blockItems)
        {
            var generator = BinaryDataGenerator.New();

            foreach (var item in blockItems)
            {
                generator.Append(item.Item1, keyEncoder);
                generator.Append(item.Item2, hidEncoder);
            }

            return generator.GetData();
        }

        private BinaryData EncodeLeafBlock(Tuple<TKey, TValue>[] blockItems)
        {
            var generator = BinaryDataGenerator.New();

            foreach (var item in blockItems)
            {
                generator.Append(item.Item1, keyEncoder);
                generator.Append(item.Item2, valueEncoder);
            }

            return generator.GetData();
        }
    }
}
