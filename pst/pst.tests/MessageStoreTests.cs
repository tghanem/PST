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
            var sut = PSTFile.Open(new MemoryStream(Resources.PST));

            //Act
            var store = sut.MessageStore;

            //Assert
            Assert.AreEqual("Test", store.DisplayName);
        }

        [Test]
        public void ShouldDetectInboxFolder()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.PST));

            //Act
            var topOfPSTDataFile = sut.RootFolder.GetSubFolders().First(f => f.DisplayName == "Top of Outlook data file");

            //Assert
            Assert.IsTrue(topOfPSTDataFile.GetSubFolders().Any(f => f.DisplayName == "Inbox"));
        }
    }
}
