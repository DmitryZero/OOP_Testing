using PatentPO;

namespace Programm
{
    class Program
    {
        static void Main(string[] args)
        {
            var experts = new List<Expert> {
                new Expert("Михайлов Андрей Виктороич"),
                new Expert("Чернышев Сергей Дмитриевич"),
                new Expert("Большаков Виктор Владимирович")
            };


            var rospatent = Rospatent.getInstance();
            rospatent.experts.AddRange(experts);

            Client client = new Client("Никитин Дмитрий Алексеевич");
            var application = client.SendApplication("Тест 1", "Тест 2");
            if (application.checkApplication != null) client.PayFee(application.checkApplication);

            var currentExpert = application.expertise?.firstExpert;
            if (currentExpert != null)
            {
                currentExpert.ApproveExpertise();
            }

        }
    }
}