using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace pst.tests
{
    [TestClass]
    public class MessageStoreTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            var sut = new PSTFile(@"c:\users\timothy\Desktop\Test.pst");

            //Act
            var store = sut.GetMessageStore();

            //Assert

        }
    }
}
