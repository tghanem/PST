using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;

namespace pst.tests
{
    [TestFixture]
    public class CachingTests
    {
        [Test]
        public void ShouldCorrectlyCachePropertyFromMessagePropertyContext()
        {
            //Arrange
            var stream = new MemoryStream(Resources.user1_test_lab);

            var sut = GetFolderSut(stream);

            //Act
            var message = sut.GetMessages()[0];

            message.GetProperty(MAPIProperties.PidTagMessageClass);

            ZeroizeStream(stream);

            var result2 = message.GetProperty(MAPIProperties.PidTagMessageClass);

            //Assert
            Assert.AreEqual("IPM.Note", result2.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyCachePropertyFromRecipientTableContext()
        {
            //Arrange
            var stream = new MemoryStream(Resources.user1_test_lab);

            var sut = GetFolderSut(stream);

            //Act
            var recipient = sut.GetMessages()[0].GetRecipients()[0];

            recipient.GetProperty(MAPIProperties.PidTagEmailAddress);

            ZeroizeStream(stream);

            var result2 = recipient.GetProperty(MAPIProperties.PidTagEmailAddress);

            //Assert
            Assert.AreEqual("user1@test.lab", result2.Value.Value.ToUnicode());
        }

        private Folder GetFolderSut(Stream pstStream, string folderName = "FolderWithSingleMessage")
        {
            var sut = PSTFile.Open(pstStream);

            return sut.GetRootMailboxFolder().GetSubFolders().First(f => f.GetDisplayNameUnicode() == folderName);
        }

        private void ZeroizeStream(Stream pstStream)
        {
            pstStream.Seek(0, SeekOrigin.Begin);
            pstStream.Write(new byte[pstStream.Length], 0, (int)pstStream.Length);
        }
    }
}
