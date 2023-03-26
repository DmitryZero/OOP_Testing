using PatentPO;

namespace Tests
{
    [TestClass]
    public class ClientInit
    {
        public static Client client;
        public static Rospatent rospatent;
        public static Application application;
        public static Patent patent;
        public static List<Expert> experts;
        [TestInitialize]
        public void InitializeTests()
        {
            DateClass dateClass = DateClass.getInstance();
            dateClass.date = new DateOnly(2001, 1, 1);

            rospatent = Rospatent.getInstance();

            experts = new List<Expert>();
            experts.AddRange(new List<Expert>(){
                new Expert("Макаров Андрей Викторович"),
                new Expert("Ложкин Ярослав Андреевич"),
                new Expert("Смирнов Кирилл Сергеевич"),
            });
            rospatent.experts.Clear();
            rospatent.experts.AddRange(experts);

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
            Client client1 = new Client("Меньшиков Михаил Андреевич");
            Assert.AreEqual(client1.fullName, "Меньшиков Михаил Андреевич");
            CollectionAssert.AreEqual(client1.applications, new List<Application>());
            CollectionAssert.AreEqual(client1.patents, new List<Patent>());
            CollectionAssert.AreEqual(client1.membershipPatents, new List<Patent>());
            CollectionAssert.AreEqual(client1.patentChecks, new List<Check>());
            CollectionAssert.AreEqual(client1.notificationList, new List<string>());
        }
        [TestMethod]
        public void MethodTest() {            
            Assert.AreEqual(client.applications.Count, 0);
            client.SendApplication("Тест 1", "Тест 2");            
            Assert.AreEqual(client.applications.Count, 1);

            Client buyer = new Client("Меньшиков Михаил Андреевич");
            patent = new Patent(application);
            client.SendCheckForMembership(buyer, patent, 30000);
            Assert.AreEqual(buyer.patentChecks.Last().checkType, CheckType.MembershipPayment);
            Assert.AreEqual(buyer.patentChecks.Last().summ, (uint)30000);
            Assert.AreEqual(buyer.patentChecks.Last().senderClient, client);

            Assert.AreEqual(buyer.patentChecks.Last().status, CheckStatus.PendingPayment);
            buyer.PayFee(buyer.patentChecks.Last());
            Assert.AreEqual(buyer.patentChecks.Last().status, CheckStatus.Payed);

            client.SendExtendPatentRequest(patent);
            Assert.AreEqual(patent.patentChecks.Last().checkType, CheckType.ExtensionPatentPayment);
        }
    }
}