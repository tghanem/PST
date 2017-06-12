namespace pst
{
    public static class ExtensionMethods
    {
        public static string GetDisplayNameUnicode(this Folder folder)
        {
            var displayNamePropertyValue = folder.GetProperty(MAPIProperties.PidTagDisplayName);

            return displayNamePropertyValue.Value.Value.ToUnicode();
        }

        public static string GetDisplayNameUnicode(this Message message)
        {
            var displayNamePropertyValue = message.GetProperty(MAPIProperties.PidTagDisplayName);

            return displayNamePropertyValue.Value.Value.ToUnicode();
        }
    }
}
