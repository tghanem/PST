using pst.utilities;

namespace pst.encodables.messaging
{
    class NAMEID
    {
        //4 bytes
        public int PropertyId { get; }

        //1 bit
        public int Type { get; }

        //15 bits
        public int GuidIndex { get; }

        //2 bytes
        public int PropertyIndex { get; }

        public NAMEID(int propertyId, int type, int guidIndex, int propertyIndex)
        {
            PropertyId = propertyId;
            Type = type;
            GuidIndex = guidIndex;
            PropertyIndex = propertyIndex;
        }

        public static NAMEID OfValue(BinaryData encodedData)
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
