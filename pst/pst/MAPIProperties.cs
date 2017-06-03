namespace pst
{
    public static class MAPIProperties
    {
        public static readonly PropertyTag PidTagRecordKey = new PropertyTag(new PropertyId(0x0FF9), PropertyType.PtypBinary);
        public static readonly PropertyTag PidTagDisplayName = new PropertyTag(new PropertyId(0x3001), PropertyType.PtypString);
        public static readonly PropertyTag PidTagIpmSubTreeEntryId = new PropertyTag(new PropertyId(0x35E0), PropertyType.PtypBinary);
        public static readonly PropertyTag PidTagIpmWastebasketEntryId = new PropertyTag(new PropertyId(0x35E3), PropertyType.PtypBinary);
        public static readonly PropertyTag PidTagFinderEntryId = new PropertyTag(new PropertyId(0x35E7), PropertyType.PtypBinary);

        public static readonly PropertyTag PidTagContentCount = new PropertyTag(new PropertyId(0x3602), PropertyType.PtypInteger32);
        public static readonly PropertyTag PidTagContentUnreadCount = new PropertyTag(new PropertyId(0x3603), PropertyType.PtypInteger32);
        public static readonly PropertyTag PidTagSubfolders = new PropertyTag(new PropertyId(0x360A), PropertyType.PtypBoolean);
    }
}
