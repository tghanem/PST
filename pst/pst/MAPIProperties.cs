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

        public static readonly PropertyTag PidTagMessageClass = new PropertyTag(new PropertyId(0x001A), PropertyType.PtypString);
        public static readonly PropertyTag PidTagMessageFlags = new PropertyTag(new PropertyId(0x0E07), PropertyType.PtypInteger32);
        public static readonly PropertyTag PidTagMessageSize = new PropertyTag(new PropertyId(0x0E08), PropertyType.PtypInteger32);
        public static readonly PropertyTag PidTagMessageStatus = new PropertyTag(new PropertyId(0x0E17), PropertyType.PtypInteger32);
        public static readonly PropertyTag PidTagCreationTime = new PropertyTag(new PropertyId(0x3007), PropertyType.PtypTime);
        public static readonly PropertyTag PidTagLastModificationTime = new PropertyTag(new PropertyId(0x3008), PropertyType.PtypTime);
        public static readonly PropertyTag PidTagSearchKey = new PropertyTag(new PropertyId(0x300B), PropertyType.PtypBinary);
    }
}
