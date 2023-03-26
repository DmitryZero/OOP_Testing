using PatentPO;

namespace Tests
{
    [TestClass]
    public class ExpertUseCase
    {
        //Инициализация
        public static Client client;
        public static Rospatent rospatent;
        public static Application application;
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
        }
        //Одобрить заявку
        [TestMethod]
        public void ApproveFirstExpertise()
        {
            var currentExpert = experts[0];

            rospatent.SendCheckForFirstExpertise(application);
            client.PayFee(client.applications.First().checks.Last());

            Assert.IsNotNull(application.expertise);
            Assert.AreEqual(application.expertise!.firstExpert, currentExpert);

            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Busy);
            currentExpert.ApproveExpertise();
            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Available);
            Assert.AreEqual(ApplicationStatus.AwaitSecondExpertisePayment, application.status);
        }
        [TestMethod]
        public void ApproveSecondExpertise()
        {
            var currentExpert = experts[0];

            rospatent.SendCheckForFirstExpertise(application);
            client.PayFee(client.applications.First().checks.Last());

            Assert.AreNotEqual(application.expertise, null);
            Assert.AreEqual(application.expertise!.firstExpert, currentExpert);

            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Busy);
            currentExpert.ApproveExpertise();
            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Available);
            Assert.AreEqual(client.applications.First().checks.Last().checkType, CheckType.SecondExpertiseFee);


            client.PayFee(client.applications.First().checks.Last());
            application.expertise.secondExperts!.ForEach(expert =>
            {
                Assert.AreEqual(expert.expertStatus, ExpertStatus.Busy);
                expert.ApproveExpertise();
            });

            Assert.AreEqual(client.applications.First().status, ApplicationStatus.Approved);

        }
        //Отклонить заявку - первичная экспертиза
        [TestMethod]
        public void RejectFirstExpertise()
        {
            var currentExpert = experts[0];

            rospatent.SendCheckForFirstExpertise(application);
            client.PayFee(client.applications.First().checks.Last());

            Assert.IsNotNull(application.expertise);
            Assert.AreEqual(application.expertise!.firstExpert, currentExpert);

            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Busy);
            currentExpert.RejectExpertise();
            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Available);
            Assert.AreEqual(application.status, ApplicationStatus.Rejected);
        }
        //Отклонить заявку - вторичная экспертиза
        [TestMethod]
        public void RejectSecondExpertise()
        {
            var currentExpert = experts[0];

            rospatent.SendCheckForFirstExpertise(application);
            client.PayFee(client.applications.First().checks.Last());

            Assert.AreNotEqual(application.expertise, null);
            Assert.AreEqual(application.expertise!.firstExpert, currentExpert);

            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Busy);
            currentExpert.ApproveExpertise();
            Assert.AreEqual(currentExpert.expertStatus, ExpertStatus.Available);
            Assert.AreEqual(client.applications.First().checks.Last().checkType, CheckType.SecondExpertiseFee);


            client.PayFee(client.applications.First().checks.Last());

            for (int i = 0; i < application.expertise.secondExperts!.Count; i++) {
                Assert.AreEqual(application.expertise.secondExperts[i].expertStatus, ExpertStatus.Busy);
                
                if (i == 1) {
                    application.expertise.secondExperts[i].RejectExpertise();
                    break;
                } 
                else application.expertise.secondExperts[i].ApproveExpertise();                
            }

            Assert.AreEqual(client.applications.First().status, ApplicationStatus.Rejected);

        }
    }
}