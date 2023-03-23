using PatentPO;

namespace Tests
{
    [TestClass]
    public class RospatentInit
    {
        [TestMethod]
        public void initTwice()
        {
            Rospatent rospatent1 = Rospatent.getInstance();
            Rospatent rospatent2 = Rospatent.getInstance();

            Assert.AreEqual(rospatent1, rospatent2);
            
        }
    }
}