namespace PatentPO;
public class Patent
{
    public Application application { get; set; }
    public List<Check> patentChecks { get; set; } = new List<Check>();
    public Patent(Application application)
    {
        this.application = application;
    }
}
