using PatentPO;

namespace Tests
{
    [TestClass]
    public class MembershipProcess
    {
        
        [TestMethod]
        public void SendCheckMemberShip() {  
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

            var dateClass = DateClass.getInstance();

            Client buyer = new Client("Меньшиков Михаил Андреевич");
            patent = new Patent(application);
            Assert.AreEqual(client.SendCheckForMembership(buyer, patent, 30000), true);
            Assert.AreEqual(buyer.patentChecks.Last().checkType, CheckType.MembershipPayment);
            Assert.AreEqual(buyer.patentChecks.Last().summ, (uint)30000);
            Assert.AreEqual(buyer.patentChecks.Last().senderClient, client);

            Assert.AreEqual(buyer.patents.Count, 0);
            buyer.PayFee(buyer.patentChecks.Last());
            Assert.AreEqual(patent, buyer.membershipPatents.Last());
        }
    }
}