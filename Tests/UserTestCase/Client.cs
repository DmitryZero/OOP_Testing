using PatentPO;

namespace Tests
{
    [TestClass]
    public class ClientUseCase
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
        public void SendApplication() {  
            Assert.AreEqual(client.applications.Count, 0);
            client.SendApplication("Тест 1", "Тест 2");            
            Assert.AreEqual(client.applications.Count, 1);
            Assert.AreEqual(ApplicationStatus.AwaitRegistrationPayment, client.applications.Last().status);
        }
        [TestMethod]
        public void SendCheckMemberShip() {  
            Client buyer = new Client("Меньшиков Михаил Андреевич");
            patent = new Patent(application);
            client.SendCheckForMembership(buyer, patent, 30000);
            Assert.AreEqual(buyer.patentChecks.Last().checkType, CheckType.MembershipPayment);
            Assert.AreEqual(buyer.patentChecks.Last().summ, (uint)30000);
            Assert.AreEqual(buyer.patentChecks.Last().senderClient, client);
        }
        [TestMethod]
        public void PayCheck() {  
            Assert.AreEqual(client.applications.Count, 0);
            client.SendApplication("Тест 1", "Тест 2");            
            Assert.AreEqual(client.applications.Count, 1);

            var currentApplication = client.applications.Last();
            var registrationCheck = currentApplication.checks.Last();            

            Assert.AreEqual(CheckType.RegistrationFee, registrationCheck.checkType);
            Assert.AreEqual(CheckStatus.PendingPayment, registrationCheck.status);            
            client.PayFee(registrationCheck);
            Assert.AreEqual(CheckStatus.Payed, registrationCheck.status);
            Assert.AreEqual(currentApplication, rospatent.applications.Last());
        }
    }
}