using System;
using pst.encodables.messaging;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging
{
    class NAMEIDDecoder : IDecoder<NAMEID>
    {
        public NAMEID Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var propertyId = parser.TakeAndSkip(4).ToInt32();

            var typeNameGuidIndex = parser.TakeAndSkip(2).ToInt32();

            var propertyIndex = parser.TakeAndSkip(2).ToInt32();

            return 
                new NAMEID(
                    propertyId, 
                    typeNameGuidIndex & 0x0001,
                    (typeNameGuidIndex & 0xFFFE) >> 1,
                    propertyIndex);
        }
    }
}
