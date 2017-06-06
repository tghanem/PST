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
        public void ForMessageStore_ShouldFindProperty_PidTagRecordKey()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagRecordKey);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessageStore_ShouldFindProperty_PidTagDisplayName()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagDisplayName);

            //Assert
            Assert.AreEqual("user1@test.lab", Encoding.Unicode.GetString(result.Value.Value));
        }

        [Test]
        public void ForMessageStore_ShouldFindProperty_PidTagIpmSubTreeEntryId()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessageStore_ShouldFindProperty_PidTagIpmWastebasketEntryId()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagIpmWastebasketEntryId);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForMessageStore_ShouldFindProperty_PidTagFinderEntryId()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.MessageStore.GetProperty(MAPIProperties.PidTagFinderEntryId);

            //Assert
            Assert.IsTrue(result.HasValue);
        }
    }
}
