using PatentPO;

namespace Tests
{
    [TestClass]
    public class ExpertInit
    {
        [TestMethod]
        public void isInit()
        {
            Expert expert = new Expert("Меньшиков Михаил Андреевич");
            Assert.AreEqual(expert, expert);
            Assert.AreEqual(expert.fullName, "Меньшиков Михаил Андреевич");
        }
    }
}