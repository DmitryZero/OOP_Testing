using PatentPO;

namespace Tests
{
    [TestClass]
    public class DateClassTest
    {        
        [TestMethod]
        public void isInit()
        {
            var dateClass1 = DateClass.getInstance();
            var dateClass2 = DateClass.getInstance();

            Assert.AreEqual(dateClass1, dateClass2);
        }
    }
}