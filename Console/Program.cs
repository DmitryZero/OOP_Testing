using PatentPO;

namespace Programm
{
    class Program
    {
        static void Main(string[] args)
        {   
            Expert expert = new Expert("Михайлов Андрей Виктороич");
            var rospatent = Rospatent.getInstance();
            rospatent.experts.Add(expert);

            Client client = new Client("Никитин Дмитрий Алексеевич");       
            var check = client.SendApplication("Тест 1", "Тест 2");     
            client.PayFee(check);

            // client.PayCheck(check);

        }
    }
}