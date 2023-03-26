using PatentPO;

namespace Tests
{
    [TestClass]
    public class ExtendPatentProcess
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
            Application application = new Application(inventionName, essay, client);

            Patent patent = new Patent(application);
            application.patent = patent;

            var dateClass = DateClass.getInstance();

            Assert.AreEqual(patent.expireDate, new DateOnly(dateClass.date.Year, dateClass.date.Month, dateClass.date.Day).AddYears(1));
            
            Assert.AreEqual(client.SendExtendPatentRequest(patent), true);
            var extendPatentCheck = client.patentChecks.Last();
            Assert.AreEqual(CheckStatus.PendingPayment, extendPatentCheck.status);
            Assert.AreEqual(CheckType.ExtensionPatentPayment, extendPatentCheck.checkType);
            client.PayFee(extendPatentCheck);

            Assert.AreEqual(patent.expireDate, new DateOnly(dateClass.date.Year, dateClass.date.Month, dateClass.date.Day).AddYears(2));
        }
    }
}