using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;

namespace pst.tests
{
    [TestFixture]
    public class AttachmentTests
    {
        [Test]
        public void ShouldCorrectlyReadAttachmentFileName()
        {
            //Arrange
            var sut = GetMessageSut();

            //Act
            var result = sut.GetAttachments()[0].GetProperty(MAPIProperties.PidTagAttachLongFilename);

            //Assert
            Assert.AreEqual("TextFile.txt", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyReadAttachmentContent()
        {
            //Arrange
            var sut = GetMessageSut();

            //Act
            var result = sut.GetAttachments()[0].GetProperty(MAPIProperties.PidTagAttachDataBinary);

            //Assert
            //TODO: The .txt file (i.e. attachment) was generated as ANSI. Should be changed to Unicode.
            Assert.AreEqual("Test", result.Value.Value.ToAnsi());
        }

        private Message GetMessageSut()
        {
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            return
                sut
                .GetRootMailboxFolder()
                .GetSubFolders()
                .First(f => f.GetDisplayNameUnicode() == "FolderWithMessagesWithAttachments")
                .GetMessages()
                .First(m => m.GetSubjectUnicode() == "TestWithTextFileAttachment");
        }
    }
}
