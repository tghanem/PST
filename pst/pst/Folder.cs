namespace pst
{
    public class Folder
    {
        internal Folder()
        {
        }

        public Folder[] GetSubFolders()
        {
            return new Folder[0];
        }

        public string DisplayName
        {
            get
            {
                return null;
            }
        }
    }
}
