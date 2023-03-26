using PatentPO;

namespace Tests
{
    // 3) Новая заявка -> Оплата регистрации -> Регистрация -> Оплата первичной экспертизы -> Ожидание первичной экспертизы -> Первичная Экспертиза -> Одобрено -> Оплата вторичной экспертизы -> Ожидание вторичной экспертизы -> Вторичная экспертиза -> Отклонено
    [TestClass]
    public class PathTest3
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

            //Одобрено
            var firstExpert = currentApplication.expertise.firstExpert!;
            firstExpert.ApproveExpertise();
            Assert.AreEqual(ApplicationStatus.AwaitSecondExpertisePayment, currentApplication.status);

            //Оплата вторичной экспертизы
            var secondExpertiseFee = currentApplication.checks.Last();
            Assert.AreEqual(CheckType.SecondExpertiseFee, secondExpertiseFee.checkType);
            client.PayFee(secondExpertiseFee);

            //Ожидание завершения другого проекта - не должно быть, так как все сотрудники были свободны     
            //Вторичная Экспертиза
            Assert.IsNotNull(currentApplication.expertise);
            Assert.AreEqual(ApplicationStatus.SecondExpertise, currentApplication.status);   

            //Отклонить
            var expert1 = rospatent.experts[0];
            var expert2 = rospatent.experts[1];
            var expert3 = rospatent.experts[2];

            expert1.ApproveExpertise();
            expert2.RejectExpertise();
            expert3.ApproveExpertise();

            Assert.AreEqual(ApplicationStatus.Rejected, currentApplication.status);
        }
    }
}