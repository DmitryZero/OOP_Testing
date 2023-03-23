namespace PatentPO;
public partial class Application
{
    internal class Expertise
    {
        public Expert? firstExpert { get; set; }
        public List<Expert>? secondExperts { get; set; }
        public List<bool> approvalList { get; set; } = new List<bool>();
        public Application application { get; private set; }
        public Expertise(Application application)
        {
            this.application = application;       
        }
    }
}
