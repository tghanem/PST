using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;
using System.Text;
using pst.utilities;

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

        [Test]
        public void ShouldCorrectlyReadMessageSubjectOfSize4K()
        {
            //Arrange
            var sut = GetMessageSut("FolderWithSampleMessages", "TestWithBodyOf4KCharacters");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidTagBody);

            //Assert
            Assert.AreEqual(new string('a', 4 * 1028), result.Value.Value.ToUnicode().Trim());
        }

        [Test]
        public void ShouldCorrectlyReadMessageProperty_NumericalNamedProperty_PidLidInternetAccountName()
        {
            //Arrange
            var sut = GetMessageSut("FolderWithSingleMessage", "Test1");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidLidInternetAccountName);

            //Assert
            Assert.AreEqual("user1@test.lab", result.Value.Value.ToUnicode().Trim());
        }

        [Test]
        public void ShouldCorrectlyReadMessageProperty_StringNamedProperty_ContentType()
        {
            //Arrange
            var sut = GetMessageSut("FolderWithSingleMessage", "Test1");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidNameContentType);

            //Assert
            Assert.IsTrue(result.Value.Value.ToUnicode().Trim().Contains("multipart/alternative"));
        }

        [Test]
        public void ShouldCorrectlyReadMessageCategories()
        {
            //Arrange
            var sut = GetMessageSut("FolderWithSampleMessages", "TestWithYellowAndGreenCategories");

            //Act
            var result = sut.GetProperty(MAPIProperties.PidNameKeywords).Value.GetMultipleValues();

            //Assert
            var multipleValuesAsStrings = result.Select(d => d.ToUnicode()).ToArray();
            Assert.AreEqual("Yellow category", multipleValuesAsStrings[1]);
            Assert.AreEqual("Green category", multipleValuesAsStrings[0]);
        }

        private Message GetMessageSut(string folderName, string messageSubject)
        {
            var folder = GetFolderSut(folderName);

            return folder.GetMessages().First(m => m.GetSubjectUnicode() == messageSubject);
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
