using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;

namespace pst.tests
{
    [TestFixture]
    public class MessageStoreTests
    {
        [Test]
        public void ShouldCorrectlyReadThePSTDisplayName()
        {
            //Arrange
            var sut = new PSTFile(new MemoryStream(Resources.PST));

            //Act
            var store = sut.GetMessageStore();

            //Assert
            Assert.AreEqual("Test", store.DisplayName);
        }

        [Test]
        public void FolderCalledInboxExistsAsSubFolderToRootFolder()
        {
            //Arrange
            var sut = new PSTFile(new MemoryStream(Resources.PST));

            //Act
            var folder = sut.GetRootFolder();
            var displayName = folder.DisplayName;
            //Assert
            Assert.IsTrue(folder.GetSubFolders().Any(f => f.DisplayName == "Inbox"));
        }
    }
}
