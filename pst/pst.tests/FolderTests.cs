using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;

namespace pst.tests
{
    [TestFixture]
    public class FolderTests
    {
        [Test]
        public void ShouldCorrectlyReadFolderDisplayName()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetProperty(MAPIProperties.PidTagDisplayName);

            //Assert
            Assert.AreEqual("Top of Outlook data file", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyReadFolderContentCount()
        {
            //Arrange
            var sut = GetFolderSut("FolderWithSingleMessage");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidTagContentCount);

            //Assert
            Assert.AreEqual(1, result.Value.Value.ToInt32());
        }

        [Test]
        public void ShouldCorrectlyReadFolderContentUnreadCount()
        {
            //Arrange
            var sut = GetFolderSut("FolderWithSingleUnreadMessage");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidTagContentUnreadCount);

            //Assert
            Assert.AreEqual(1, result.Value.Value.ToInt32());
        }

        [Test]
        public void ShouldCorrectlyDetectIfFolderHasSubfolders()
        {
            //Arrange
            var sut = GetFolderSut("FolderWithTwoSubFolders");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidTagSubfolders);

            //Assert
            Assert.IsTrue(result.Value.Value.ToBoolean());
        }

        [Test]
        [Ignore("Not Yet Implemented")]
        public void ShouldCorrectlyDetectNewlyAddedFolder()
        {
            //Arrange
            var memoryStream = new MemoryStream(Resources.user1_test_lab);

            var sut = PSTFile.Open(memoryStream);

            //Act
            sut.GetRootMailboxFolder().NewFolder("NewFolder");

            sut.Save();

            sut = PSTFile.Open(memoryStream);

            //Assert
            Assert.IsTrue(sut.GetRootMailboxFolder().GetSubFolders().Any(f => f.GetDisplayNameUnicode() == "NewFolder"));
        }

        private Folder GetFolderSut(string folderName)
        {
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            return sut.GetRootMailboxFolder().GetSubFolders().First(f => f.GetDisplayNameUnicode() == folderName);
        }
    }
}
