using PatentPO;

namespace Tests
{   
    // 8) Новая заявка -> Оплата регистрации -> Регистрация -> Оплата первичной экспертизы -> Ожидание первичной экспертизы -> Ожидаем завершения другого проекта -> Первичная Экспертиза -> Одобрено -> Оплата вторичной экспертизы -> Ожидание вторичной экспертизы -> Вторичная экспертиза -> Одобрено
    [TestClass]
    public class PathTest8
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

            rospatent.experts.ForEach(expert => expert.expertStatus = ExpertStatus.Busy);

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
            client.PayFee(firstExpertiseCheck);

            //Ожидание завершения другого проекта - должно быть, так как все сотрудники изначально заняты     
            Assert.AreEqual(ApplicationStatus.AwaitFirstExpertise, currentApplication.status);
            rospatent.experts[0].expertStatus = ExpertStatus.Available; //Освобождаем сотрудника
            rospatent.RelocatePeopleOnExpertiseEnd(); //Данный метод обычно вызывается после закрытия какой-либо экспертизы. Вызовем этот метод напрямую
            Assert.IsNotNull(currentApplication.expertise);

            //Одобрено
            var firstExpert = currentApplication.expertise.firstExpert!;
            firstExpert.ApproveExpertise();
            Assert.AreEqual(ApplicationStatus.AwaitSecondExpertisePayment, currentApplication.status);            

            rospatent.experts.ForEach(expert => expert.expertStatus = ExpertStatus.Available);

            //Оплата вторичной экспертизы
            var secondExpertiseFee = currentApplication.checks.Last();
            Assert.AreEqual(CheckType.SecondExpertiseFee, secondExpertiseFee.checkType);
            client.PayFee(secondExpertiseFee);

            //Ожидание завершения другого проекта - не должно быть
            Assert.AreEqual(ApplicationStatus.SecondExpertise, currentApplication.status);

            //Одобрить
            var expert1 = rospatent.experts[0];
            var expert2 = rospatent.experts[1];
            var expert3 = rospatent.experts[2];

            expert1.ApproveExpertise();
            expert2.ApproveExpertise();
            expert3.ApproveExpertise();

            Assert.AreEqual(ApplicationStatus.Approved, currentApplication.status);
        }
    }
}