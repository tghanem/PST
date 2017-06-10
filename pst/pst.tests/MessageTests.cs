using NUnit.Framework;
using pst.tests.Properties;
using System.IO;
using System.Text;

namespace pst.tests
{
    [TestFixture]
    public class MessageTests
    {
        [Test]
        public void ForMessage_ShouldFindProperty_PidTagMessageClass()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0].GetProperty(MAPIProperties.PidTagMessageClass);

            //Assert
            Assert.AreEqual("IPM.Note", result.Value.Value.ToUnicode());
        }

        [Test]
        public void ForMessage_ShouldFindProperty_PidTagMessageFlags()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0].GetProperty(MAPIProperties.PidTagMessageFlags);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessage_ShouldFindProperty_PidTagMessageSize()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0].GetProperty(MAPIProperties.PidTagMessageSize);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessage_ShouldCorrectlyReadProperty_PidTagEmailAddress()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0];

            //Assert
            var emailAddressPropertyValue = result.Recipients[0].GetProperty(MAPIProperties.PidTagEmailAddress).Value;

            Assert.AreEqual("user1@test.lab", Encoding.Unicode.GetString(emailAddressPropertyValue.Value));
        }

        [Test]
        public void ForMessage_ShouldFindProperty_PidTagCreationTime()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0].GetProperty(MAPIProperties.PidTagCreationTime);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessage_ShouldFindProperty_PidTagLastModificationTime()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0].GetProperty(MAPIProperties.PidTagLastModificationTime);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessage_ShouldFindProperty_PidTagSearchKey()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.GetRootMailboxFolder().GetMessages()[0].GetProperty(MAPIProperties.PidTagSearchKey);

            //Assert
            Assert.IsTrue(result.HasValue);
        }
    }
}
