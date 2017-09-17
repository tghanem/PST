using NUnit.Framework;
using pst.tests.Properties;
using pst.utilities;
using System.IO;
using System.Text;

namespace pst.tests
{
    [TestFixture]
    public class MessageStoreTests
    {
        [Test]
        public void ShouldCorrectlyReadMessageStoreDisplayName()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagDisplayName);

            //Assert
            Assert.AreEqual("user1@test.lab", Encoding.Unicode.GetString(result.Value.Value));
        }

        [Test]
        public void ShouldCorrectlyReadMessageStorePassword()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagPstPassword);

            //Assert

            //Although the PST is a Unicode PST. However, the password is encoded as ASCII.
            //TODO: Get some documentation on this.
            var encodedPassword = Encoding.ASCII.GetBytes("user1");

            Assert.AreEqual(Crc32.ComputeCrc32(encodedPassword), result.Value.Value.ToInt32());
        }
    }
}
