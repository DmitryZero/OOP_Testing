using PatentPO;

namespace Tests
{
    // 1)Новая заявка -> Оплата регистрации -> Регистрация -> Оплата первичной экспертизы -> Ожидание первичной экспертизы -> Первичная Экспертиза -> Отклонено
    [TestClass]
    public class PathTest1
    {        
        [TestMethod]
        public void Run()
        {
            Rospatent rospatent = Rospatent.getInstance();
            rospatent.applications.Clear();
            rospatent.patents.Clear();
            rospatent.experts.Clear();

            rospatent.experts.AddRange(new List<Expert>(){
                new Expert("Эксперт 1"),
                new Expert("Эксперт 2"),
                new Expert("Эксперт 3")
            });


            string inventionName = "Название 1";
            string essay = "Реферат 1";
            string fio = "Никифоров Ярослав Николаевич";

            Client client = new Client(fio);

            //Новая заявка
            client.SendApplication(inventionName, essay);
            var currentApplication = client.applications.Last();
            var registrationCheck = currentApplication.checks.Last();
            Assert.AreEqual(CheckType.RegistrationFee, registrationCheck.checkType);

            //Оплата регистрации
            Assert.AreEqual(registrationCheck.status, CheckStatus.PendingPayment);
            client.PayFee(registrationCheck);
            Assert.AreEqual(registrationCheck.status, CheckStatus.Payed);

            //Регистрация
            Assert.AreEqual(rospatent.applications.Last(), currentApplication);

            //Оплата первичной экспертизы
            var firstExpertiseCheck = currentApplication.checks.Last();
            Assert.AreEqual(CheckType.FirstExpertiseFee, firstExpertiseCheck.checkType);
            Assert.AreEqual(true, rospatent.experts.All(expert => expert.expertStatus == ExpertStatus.Available));
            client.PayFee(firstExpertiseCheck);

            //Ожидание завершения другого проекта - не должно быть, так как все сотрудники были свободны     
            //Первичная Экспертиза
            Assert.IsNotNull(currentApplication.expertise);
            Assert.AreEqual(ApplicationStatus.FirstExpertise, currentApplication.status);            

            //Отклонено       
            var firstExpert = currentApplication.expertise.firstExpert!;
            firstExpert.RejectExpertise();
            Assert.AreEqual(ApplicationStatus.Rejected, currentApplication.status);
        }
    }
}