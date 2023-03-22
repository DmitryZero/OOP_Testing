namespace PatentPO;
public partial class Application
{
    internal class Expertise
    {
        public Expert? firstExpert { get; set; }
        public List<Expert>? secondExperts { get; set; }
        public List<bool> approvalList { get; set; } = new List<bool>();
        public Client client { get; private set; }
        public Application application { get; private set; }
        public Expertise(Client client, Application application)
        {
            this.client = client;
            this.application = application;       
        }
    }
}
