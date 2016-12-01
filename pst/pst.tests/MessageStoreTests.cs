using Microsoft.VisualStudio.TestTools.UnitTesting;
using pst.tests.Properties;
using System.IO;

namespace pst.tests
{
    [TestClass]
    public class MessageStoreTests
    {
        [TestMethod]
        public void ShouldCorrectlyReadThePSTDisplayName()
        {
            //Arrange
            var sut = new PSTFile(new MemoryStream(Resources.PST));

            //Act
            var store = sut.GetMessageStore();

            //Assert
            Assert.AreEqual("Test", store.DisplayName);
        }
    }
}
