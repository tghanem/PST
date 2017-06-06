using NUnit.Framework;
using pst.tests.Properties;
using System.IO;

namespace pst.tests
{
    [TestFixture]
    public class FolderTests
    {
        [Test]
        public void ForFolder_ShouldFindProperty_PidTagDisplayName()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.RootFolder.GetProperty(MAPIProperties.PidTagDisplayName);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForFolder_ShouldFindProperty_PidTagContentCount()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.RootFolder.GetProperty(MAPIProperties.PidTagContentCount);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForFolder_ShouldFindProperty_PidTagContentUnreadCount()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.RootFolder.GetProperty(MAPIProperties.PidTagContentUnreadCount);

            //Assert
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void ForFolder_ShouldFindProperty_PidTagSubfolders()
        {
            //Arrange
            var sut = PSTFile.Open(new MemoryStream(Resources.user1_test_lab));

            //Act
            var result = sut.RootFolder.GetProperty(MAPIProperties.PidTagSubfolders);

            //Assert
            Assert.IsTrue(result.HasValue);
        }
    }
}
