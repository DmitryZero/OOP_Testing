using PatentPO;

namespace Tests
{
    [TestClass]
    public class RospatentInit
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
        //Тестирование Singleton
        [TestMethod]
        public void testSingleton()
        {
            Rospatent rospatent1 = Rospatent.getInstance();
            Rospatent rospatent2 = Rospatent.getInstance();

            Assert.AreEqual(rospatent1, rospatent2);
        }
        //Тестрование свойств при инициализации
        [TestMethod]
        public void testProperties()
        {
            Rospatent rospatent = Rospatent.getInstance();

            Assert.AreEqual(Rospatent.registrationFee, (uint)2000);
            Assert.AreEqual(Rospatent.firstExpertiseFee, (uint)3500);
            Assert.AreEqual(Rospatent.secondExpertiseFee, (uint)9000);
            Assert.AreEqual(Rospatent.patentGrantFee, (uint)1500);
            Assert.AreEqual(Rospatent.patentExtentiosFee, (uint)5000);
            Assert.AreEqual(Rospatent.secondExpertiseLength, (ushort)3);

            CollectionAssert.AreEqual(rospatent.experts, experts);
            CollectionAssert.AreEqual(rospatent.patents, new List<Patent>());
            CollectionAssert.AreEqual(rospatent.applications, new List<Application>());
        }
        //Тестирование методов по выдаче чека
        [TestMethod]
        public void ChecksMethods()
        {
            rospatent.SendCheckForRegistration(application);
            Assert.AreEqual(application.checks.Last().summ, (uint)2000);
            Assert.AreEqual(application.checks.Last().checkType, CheckType.RegistrationFee);
            Assert.AreEqual(application.checks.Last().payerClient, client);
            Assert.AreEqual(application.checks.Last().senderRospatent, rospatent);

            rospatent.SendCheckForFirstExpertise(application);
            Assert.AreEqual(application.checks.Last().summ, (uint)3500);
            Assert.AreEqual(application.checks.Last().checkType, CheckType.FirstExpertiseFee);
            Assert.AreEqual(application.checks.Last().payerClient, client);
            Assert.AreEqual(application.checks.Last().senderRospatent, rospatent);

            rospatent.SendCheckForSecondExpertise(application);
            Assert.AreEqual(application.checks.Last().summ, (uint)9000);
            Assert.AreEqual(application.checks.Last().checkType, CheckType.SecondExpertiseFee);
            Assert.AreEqual(application.checks.Last().payerClient, client);
            Assert.AreEqual(application.checks.Last().senderRospatent, rospatent);

            rospatent.SendCheckForPatentGrant(patent, patent.application.client);
            Assert.AreEqual(patent.patentChecks.Last().summ, (uint)1500);
            Assert.AreEqual(patent.patentChecks.Last().checkType, CheckType.PatentGrantingFee);
            Assert.AreEqual(patent.patentChecks.Last().payerClient, client);
            Assert.AreEqual(patent.patentChecks.Last().senderRospatent, rospatent);

            rospatent.SendCheckPatentExtention(patent);
            Assert.AreEqual(patent.patentChecks.Last().summ, (uint)5000);
            Assert.AreEqual(patent.patentChecks.Last().checkType, CheckType.ExtensionPatentPayment);
            Assert.AreEqual(patent.patentChecks.Last().payerClient, client);
            Assert.AreEqual(patent.patentChecks.Last().senderRospatent, rospatent);
        }
        //Тестирование метода по регистрации заявки
        [TestMethod]
        public void RegisterMethod()
        {
            rospatent.RegisterApplication(application);
            Assert.AreEqual(application.status, ApplicationStatus.AwaitFirstExpertisePayment);
            Assert.AreEqual(rospatent.applications.Last(), application);
        }
        //Тестирование метода по выделению экспертов на проект
        [TestMethod]
        public void PeopleAllocation()
        {
            var currentExperts = rospatent.GetFreeExpert(3);
            CollectionAssert.AreEqual(currentExperts, experts);

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
        //Тестирование методов по работе с патентами
        [TestMethod]
        public void PatentMethods()
        {
            application.patent = null;
            rospatent.SendRequestToCreatePatent(application);
            Assert.AreEqual(rospatent.patents.Last().patentChecks.Last().checkType, CheckType.PatentGrantingFee);
            
            application.patent = rospatent.patents.Last();

            rospatent.GivePatentToClient(application.patent!);
            Assert.AreEqual(client.patents.Last(), application.patent);

            Client client1 = new Client("Серебряков Сергей Дмитриевич");
            rospatent.GiveRightForPatent(application.patent!, client1);
            Assert.AreEqual(client1.membershipPatents.Last(), application.patent);

            Assert.AreEqual(rospatent.ExtendPatent(application.patent!), true);
        }
    }
}