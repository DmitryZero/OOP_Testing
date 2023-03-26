using PatentPO;

namespace Tests
{
    [TestClass]
    public class ChecksInit
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
            var check1 = new Check(application.client, rospatent, CheckType.RegistrationFee, Rospatent.registrationFee,
                                                   application, rospatent.RegisterApplication);
            var check2 = new Check(application.client, rospatent, CheckType.PatentGrantingFee, Rospatent.patentGrantFee,
                       patent, rospatent.GivePatentToClient);
            
            Assert.AreEqual(application, check1.application);
            Assert.AreEqual(CheckType.RegistrationFee, check1.checkType);
            Assert.AreEqual(application.client, check1.payerClient);
            Assert.AreEqual(rospatent, check1.senderRospatent);

            Assert.AreEqual(patent, check2.patent);
            Assert.AreEqual(CheckType.PatentGrantingFee, check2.checkType);
            Assert.AreEqual(application.client, check2.payerClient);
            Assert.AreEqual(rospatent, check2.senderRospatent);
        }
        [TestMethod]
        public void PayCheck()
        {
            var check1 = new Check(application.client, rospatent, CheckType.RegistrationFee, Rospatent.registrationFee,
                                                   application, rospatent.RegisterApplication);
            Assert.AreEqual(check1.status, CheckStatus.PendingPayment);
            check1.PayCheck();
            Assert.AreEqual(check1.status, CheckStatus.Payed);
        }
    }
}