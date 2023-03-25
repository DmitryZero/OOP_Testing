using PatentPO;

namespace Tests
{
    [TestClass]
    public class PatentInit
    {
        [TestMethod]
        public void testInit()
        {
            string inventionName = "Название 1";
            string essay = "Реферат 1";
            string fio = "Никифоров Ярослав Николаевич";

            DateClass dateClass = DateClass.getInstance();
            dateClass.date = new DateOnly(2001, 1, 4);

            Client client = new Client(fio);
            Application application = new Application(inventionName, essay, client);
            Patent patent = new Patent(application);

            Assert.AreEqual(patent.application, application);
            Assert.AreEqual(patent.GetIsExpired(), false);
            Assert.AreEqual(patent.conclusionDate, new DateOnly(2001, 1, 4));
            Assert.AreEqual(patent.expireDate, new DateOnly(2001, 1, 4).AddYears(1));
            Assert.AreEqual(patent.maxPatentDuration, new DateOnly(2001, 1, 4).AddYears(20));

            Assert.AreNotEqual(patent.patentChecks, null);
            Assert.AreNotEqual(patent.members, null);
        }
        [TestMethod]
        public void IsExtendPossible()
        {
            string inventionName = "Название 1";
            string essay = "Реферат 1";
            string fio = "Никифоров Ярослав Николаевич";

            DateClass dateClass = DateClass.getInstance();
            dateClass.date = new DateOnly(2001, 1, 4);

            Client client = new Client(fio);
            Application application = new Application(inventionName, essay, client);
            Patent patent = new Patent(application);

            for (int i = 0; i < 19; i++) {
                Assert.AreEqual(patent.IsExtendPossible(), true);
                patent.ExtendPatent();
            }
            Assert.AreEqual(patent.IsExtendPossible(), false);
        }
    }
}