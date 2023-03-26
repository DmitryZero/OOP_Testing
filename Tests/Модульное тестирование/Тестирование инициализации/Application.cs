using PatentPO;

namespace Tests
{
    [TestClass]
    public class ApplicationInit
    {
        public static Client client;
        [TestInitialize]
        public void InitializeTests()
        {
            string fio = "Никифоров Ярослав Николаевич";

            client = new Client(fio);

            client.applications.Clear();
            client.patents.Clear();
            client.patentChecks.Clear();
        }
        [TestMethod]
        public void isInit()
        {
            var applicationNew = new Application("Тест 1", "Тест 2", client);
            Assert.AreEqual(applicationNew.inventionName, "Тест 1");
            Assert.AreEqual(applicationNew.essay, "Тест 2");
            Assert.AreEqual(applicationNew.client, client);
            Assert.AreEqual(applicationNew.status, ApplicationStatus.NewApplication);
            Assert.AreEqual(client.applications.Last(), applicationNew);
        }
    }
}