namespace PatentPO;
public class Patent
{
    public Application application { get; set; }
    public List<Check> patentChecks { get; set; } = new List<Check>();
    public List<Client> members {get; set;} = new List<Client>();
    public Patent(Application application)
    {
        this.application = application;
    }
}
