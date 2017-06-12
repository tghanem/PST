using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Linq;

namespace pst.tests
{
    [TestFixture]
    public class RecipientTests
    {
        [Test]
        public void ShouldCorrectlyReadRecipientEmailAddress()
        {
            //Arrange
            var sut = GetFolderSut();

            //Act
            var result = sut.GetMessages()[0].GetRecipients()[0].GetProperty(MAPIProperties.PidTagEmailAddress);

            //Assert
            Assert.AreEqual("user1@test.lab", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ShouldCorrectlyReadRecipientType()
        {
            //Arrange
            var sut = GetFolderSut();

            //Act
            var result = sut.GetMessages()[0].GetRecipients()[0].GetProperty(MAPIProperties.PidTagRecipientType);

            //Assert
            Assert.AreEqual(MAPIProperties.PidTagRecipientTypePrimaryRecipient, result.Value.Value.ToInt32());
        }

        private Folder GetFolderSut()
        {
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            return sut.GetRootMailboxFolder().GetSubFolders().First(f => f.GetDisplayNameUnicode() == "FolderWithSingleMessage");
        }
    }
}
