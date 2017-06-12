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

        [Test]
        public void ShouldCorrectlyDetectThatTheMessageHasAttachmentsUsingPidTagMessageFlagsProperty()
        {
            //Arrange
            var sut = GetFolderSut("FolderWithMessagesWithAttachments");

            //Act
            var result = sut.GetMessages()[0].GetProperty(MAPIProperties.PidTagMessageFlags);

            //Assert
            Assert.AreEqual(MAPIProperties.mfHasAttach, result.Value.Value.ToInt32() & MAPIProperties.mfHasAttach);
        }

        private Folder GetFolderSut()
        {
            return GetFolderSut("FolderWithSingleMessage");
        }

        private Folder GetFolderSut(string folderName)
        {
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            return sut.GetRootMailboxFolder().GetSubFolders().First(f => f.GetDisplayNameUnicode() == folderName);
        }
    }
}
