using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.pc;
using pst.utilities;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.pc
{
    class PropertyContextGenerator : IPropertyContextGenerator
    {
        private const int MaximumHeapOnNodeAllocation = 3580;

        private readonly IEncoder<HNID> hnidEncoder;
        private readonly IDataTreeAllocator dataTreeAllocator;
        private readonly IFactory<IHeapOnNodeGenerator> heapOnNodeGeneratorFactory;
        private readonly IBTreeOnHeapGenerator<PropertyId, BinaryData> bthGenerator;

        public PropertyContextGenerator(
            IEncoder<HNID> hnidEncoder,
            IDataTreeAllocator dataTreeAllocator,
            IFactory<IHeapOnNodeGenerator> heapOnNodeGeneratorFactory,
            IBTreeOnHeapGenerator<PropertyId, BinaryData> bthGenerator)
        {
            this.hnidEncoder = hnidEncoder;
            this.dataTreeAllocator = dataTreeAllocator;
            this.heapOnNodeGeneratorFactory = heapOnNodeGeneratorFactory;
            this.bthGenerator = bthGenerator;
        }

        public DataTreeWithPossibleSubnodes Generate(Tuple<PropertyTag, PropertyValue>[] properties)
        {
            var dataRecords = new List<Tuple<PropertyId, BinaryData>>();

            var propertiesAllocatedOnSubnodes = new List<Tuple<NID, BID>>();

            var nidAllocator = new InternalNIDAllocator();

            var hnGenerator = heapOnNodeGeneratorFactory.Create();

            foreach (var property in properties)
            {
                dataRecords.Add(
                    EncodeDataRecord(
                        property.Item1,
                        property.Item2,
                        hnGenerator,
                        nidAllocator,
                        propertiesAllocatedOnSubnodes));
            }

            bthGenerator.Generate(2, 6, dataRecords.ToArray(), hnGenerator);

            var dataTreeRootBlockId = hnGenerator.Commit(Constants.bTypePC);

            return new DataTreeWithPossibleSubnodes(dataTreeRootBlockId, propertiesAllocatedOnSubnodes.ToArray());
        }

        private Tuple<PropertyId, BinaryData> EncodeDataRecord(
            PropertyTag propertyTag,
            PropertyValue propertyValue,
            IHeapOnNodeGenerator heapOnNodeGenerator,
            InternalNIDAllocator nidAllocator,
            List<Tuple<NID, BID>> propertiesAllocatedOnSubnodes)
        {
            if (propertyTag.Type.IsFixedLength())
            {
                var size = propertyTag.Type.GetFixedLengthTypeSize();

                if (size <= 4)
                {
                    return Tuple.Create(propertyTag.Id, propertyValue.Value);
                }

                return AllocatePropertyOnTheHN(propertyTag, propertyValue, heapOnNodeGenerator);
            }

            if (propertyTag.Type.IsMultiValueFixedLength() || propertyTag.Type.IsVariableLength() || propertyTag.Type.IsMultiValueVariableLength())
            {
                if (propertyValue.Value.Length <= MaximumHeapOnNodeAllocation)
                {
                    return AllocatePropertyOnTheHN(propertyTag, propertyValue, heapOnNodeGenerator);
                }
            }

            return AllocatePropertyOnSubnode(propertyTag, propertyValue, nidAllocator, propertiesAllocatedOnSubnodes);
        }

        private Tuple<PropertyId, BinaryData> AllocatePropertyOnSubnode(
            PropertyTag propertyTag,
            PropertyValue propertyValue,
            InternalNIDAllocator nidAllocator,
            List<Tuple<NID, BID>> propertiesAllocatedOnSubnodes)
        {
            var dataTreeRootBlockId = dataTreeAllocator.Allocate(new[] { propertyValue.Value });

            var nid = nidAllocator.New();

            var hnid = new HNID(nid);

            propertiesAllocatedOnSubnodes.Add(Tuple.Create(nid, dataTreeRootBlockId));

            return Tuple.Create(propertyTag.Id, hnidEncoder.Encode(hnid));
        }

        private Tuple<PropertyId, BinaryData> AllocatePropertyOnTheHN(
            PropertyTag propertyTag,
            PropertyValue propertyValue,
            IHeapOnNodeGenerator hnGenerator)
        {
            var propertyValueInHN =
                BinaryDataGenerator
                .New()
                .Append((short)propertyTag.Type.Value)
                .Append(propertyValue.Value)
                .GetData();

            var hid = hnGenerator.AllocateItem(propertyValueInHN);

            var hnid = new HNID(hid);

            return Tuple.Create(propertyTag.Id, hnidEncoder.Encode(hnid));
        }

        class InternalNIDAllocator
        {
            private int index;

            public NID New() => new NID(Constants.NID_TYPE_INTERNAL, index++);
        }
    }
}
