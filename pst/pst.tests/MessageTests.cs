using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;

namespace pst.tests
{
    [TestFixture]
    public class MessageTests
    {
        [Test]
        public void ShouldCorrectlyReadMessageClass()
        {
            //Arrange
            var sut = GetFolderSut();

            //Act
            var result = sut.GetMessages()[0].GetProperty(MAPIProperties.PidTagMessageClass);

            //Assert
            Assert.AreEqual("IPM.Note", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyReadMessageSubject()
        {
            //Arrange
            var sut = GetFolderSut();

            //Act
            var result = sut.GetMessages()[0].GetProperty(MAPIProperties.PidTagSubject);

            //Assert
            Assert.AreEqual("Test1", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyReadMessageSenderEmailAddress()
        {
            //Arrange
            var sut = GetFolderSut();

            //Act
            var result = sut.GetMessages()[0].GetProperty(MAPIProperties.PidTagSenderEmailAddress);

            //Assert
            Assert.AreEqual("user1@test.lab", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyReadMessageBody()
        {
            //Arrange
            var sut = GetFolderSut();

            //Act
            var result = sut.GetMessages()[0].GetProperty(MAPIProperties.PidTagBody);

            //Assert
            Assert.AreEqual("Test1", result.Value.Value.ToUnicode().Trim());
        }

        private Folder GetFolderSut()
        {
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            return sut.GetRootMailboxFolder().GetSubFolders().First(f => f.GetDisplayNameUnicode() == "FolderWithSingleMessage");
        }
    }
}
