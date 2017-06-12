using NUnit.Framework;
using pst.tests.Properties;
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
    }
}
