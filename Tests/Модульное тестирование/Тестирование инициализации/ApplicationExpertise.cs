using PatentPO;

namespace Tests
{
    [TestClass]
    public class ExpertiseInit
    {
        public static Client client;
        public static Rospatent rospatent;
        public static Application application;
        [TestInitialize]
        public void InitializeTests()
        {
            DateClass dateClass = DateClass.getInstance();
            dateClass.date = new DateOnly(2001, 1, 1);

            rospatent = Rospatent.getInstance();            

            string inventionName = "Название 1";
            string essay = "Реферат 1";
            string fio = "Никифоров Ярослав Николаевич";

            client = new Client(fio);
            application = new Application(inventionName, essay, client);

            client.applications.Clear();
            client.patents.Clear();
            client.patentChecks.Clear();
        }
        [TestMethod]
        public void isInit()
        {
            var expertise = new Application.Expertise(application);
            Assert.AreEqual(application, expertise.application);
            Assert.AreEqual(null, expertise.firstExpert);
            Assert.AreEqual(null, expertise.secondExperts);
            Assert.AreNotEqual(new List<bool>(), expertise.approvalList);
        }
    }
}