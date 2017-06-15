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
    }
}
