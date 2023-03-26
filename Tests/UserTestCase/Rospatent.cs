using PatentPO;

namespace Tests
{
    [TestClass]
    public class RospatentUseCase
    {
        //Инициализация
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
            patent = new Patent(application);

            rospatent.patents.Clear();
            rospatent.applications.Clear();
        }        
        [TestMethod]
        public void SendCheckForRegistration() {
            rospatent.SendCheckForRegistration(application);
            Assert.AreEqual(application.checks.Last().summ, (uint)2000);
            Assert.AreEqual(application.checks.Last().checkType, CheckType.RegistrationFee);
            Assert.AreEqual(application.checks.Last().payerClient, client);
            Assert.AreEqual(application.checks.Last().senderRospatent, rospatent);
        }
        [TestMethod]
        public void SendCheckForFirstExpertise() {
            rospatent.SendCheckForFirstExpertise(application);
            Assert.AreEqual(application.checks.Last().summ, (uint)3500);
            Assert.AreEqual(application.checks.Last().checkType, CheckType.FirstExpertiseFee);
            Assert.AreEqual(application.checks.Last().payerClient, client);
            Assert.AreEqual(application.checks.Last().senderRospatent, rospatent);
        }
        [TestMethod]
        public void SendCheckForSecondExpertise() {
            rospatent.SendCheckForSecondExpertise(application);
            Assert.AreEqual(application.checks.Last().summ, (uint)9000);
            Assert.AreEqual(application.checks.Last().checkType, CheckType.SecondExpertiseFee);
            Assert.AreEqual(application.checks.Last().payerClient, client);
            Assert.AreEqual(application.checks.Last().senderRospatent, rospatent);
        }
        [TestMethod]
        public void SendCheckForPatentGrant() {
            rospatent.SendCheckForPatentGrant(patent, patent.application.client);
            Assert.AreEqual(patent.patentChecks.Last().summ, (uint)1500);
            Assert.AreEqual(patent.patentChecks.Last().checkType, CheckType.PatentGrantingFee);
            Assert.AreEqual(patent.patentChecks.Last().payerClient, client);
            Assert.AreEqual(patent.patentChecks.Last().senderRospatent, rospatent);
        }
        [TestMethod]
        public void SendCheckPatentExtention() {
            rospatent.SendCheckPatentExtention(patent);
            Assert.AreEqual(patent.patentChecks.Last().summ, (uint)5000);
            Assert.AreEqual(patent.patentChecks.Last().checkType, CheckType.ExtensionPatentPayment);
            Assert.AreEqual(patent.patentChecks.Last().payerClient, client);
            Assert.AreEqual(patent.patentChecks.Last().senderRospatent, rospatent);
        }
        [TestMethod]
        public void GetFreeExpert() {
            var currentExperts = rospatent.GetFreeExpert(3);
            CollectionAssert.AreEqual(currentExperts, experts);
        }
        [TestMethod]
        public void AllocatePeople() {            
            application.status = ApplicationStatus.AwaitFirstExpertise;
            rospatent.AllocatePeople(application);
            Assert.AreEqual(application.status, ApplicationStatus.FirstExpertise);
            Assert.AreNotEqual(application.expertise, null);

            application.status = ApplicationStatus.AwaitSecondExpertise;
            Assert.AreEqual(rospatent.AllocatePeople(application), false);

            experts[0].expertStatus = ExpertStatus.Available;
            Assert.AreEqual(rospatent.AllocatePeople(application), true);
            Assert.AreEqual(experts[0].expertStatus, ExpertStatus.Busy);   
        }
        [TestMethod]
        public void RegisterApplication() {
            rospatent.RegisterApplication(application);
            Assert.AreEqual(application.status, ApplicationStatus.AwaitFirstExpertisePayment);
            Assert.AreEqual(rospatent.applications.Last(), application);
        }
        [TestMethod]
        public void CreatePatent() {
            rospatent.SendRequestToCreatePatent(application);
            Assert.AreEqual(rospatent.patents.Count, 1);
        }
        [TestMethod]
        public void RelocatePeopleOnExpertiseEnd() {
            var application1 = new Application("Тест 1", "Тест 1", client);
            var application2 = new Application("Тест 2", "Тест 2", client);

            rospatent.RegisterApplication(application1);
            rospatent.RegisterApplication(application2);

            experts.ForEach(experts => experts.expertStatus = ExpertStatus.Busy);
            experts[0].expertStatus = ExpertStatus.Available;

            application1.status = ApplicationStatus.AwaitFirstExpertise;
            application2.status = ApplicationStatus.AwaitFirstExpertise;
            rospatent.AllocatePeople(application1);
            rospatent.AllocatePeople(application2);

            Assert.AreEqual(ApplicationStatus.FirstExpertise, application1.status);
            Assert.AreEqual(ApplicationStatus.AwaitFirstExpertise, application2.status);

            experts[0].expertStatus = ExpertStatus.Available;
            rospatent.RelocatePeopleOnExpertiseEnd();
            Assert.AreEqual(ApplicationStatus.FirstExpertise, application2.status);
        }
        [TestMethod]
        public void SendPatentToClient() {
            var patent = new Patent(application);
            rospatent.GivePatentToClient(patent);
            Assert.AreEqual(patent, client.patents.Last());
        }
        [TestMethod]
        public void GiveRightForPatent() {            
            var patent = new Patent(application);
            Client client1 = new Client("Серебряков Сергей Дмитриевич");
            rospatent.GiveRightForPatent(patent, client1);
            Assert.AreEqual(client1.membershipPatents.Last(), patent);            
        }
        [TestMethod]
        public void ExtendPatent() {
            var dateClass = DateClass.getInstance();

            var patent = new Patent(application);
            Assert.AreEqual(patent.expireDate, new DateOnly(dateClass.date.Year, dateClass.date.Month, dateClass.date.Day).AddYears(1));
            Assert.AreEqual(rospatent.ExtendPatent(patent), true);
            Assert.AreEqual(patent.expireDate, new DateOnly(dateClass.date.Year, dateClass.date.Month, dateClass.date.Day).AddYears(2));
        }
    }
}