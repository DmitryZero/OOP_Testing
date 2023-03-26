using PatentPO;

namespace Tests
{
    [TestClass]
    public class PatentInit
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
        }
        //Тестирование инициализации
        [TestMethod]
        public void testInit()
        {
            Assert.AreEqual(patent.application, application);            
            Assert.AreEqual(patent.conclusionDate, new DateOnly(2001, 1, 1));
            Assert.AreEqual(patent.expireDate, new DateOnly(2001, 1, 1).AddYears(1));
            Assert.AreEqual(patent.maxPatentDuration, new DateOnly(2001, 1, 1).AddYears(20));

            Assert.AreNotEqual(patent.patentChecks, null);
            Assert.AreNotEqual(patent.members, null);
        }
        [TestMethod]
        //Тестирование методов
        public void MethodsTest()
        {
            Assert.AreEqual(patent.IsExpired(), false);

            for (int i = 0; i < 19; i++) {
                Assert.AreEqual(patent.IsExtendPossible(), true);
                patent.ExtendPatent();
            }
            Assert.AreEqual(patent.IsExtendPossible(), false);
            patent.ExtendPatent();
            Assert.AreEqual(patent.IsExpired(), true);
        }
    }
}