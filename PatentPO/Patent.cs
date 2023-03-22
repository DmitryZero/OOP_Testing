namespace PatentPO;

public class Patent
{
    public Application application { get; set; }
    public List<Check> patentChecks { get; set; } = new List<Check>();
    public List<Client> members {get; set;} = new List<Client>();
    public DateOnly conclusionDate {get; private set;}
    public DateOnly expireDate {get; private set;}
    public DateOnly maxPatentDuration;
    public bool isExpired {
        get {
            DateClass dateClass = DateClass.getInstance();
            var currentDate = dateClass.date;
            return currentDate >= expireDate;
        }
        set {
            isExpired = value;
        }
    }
    public Patent(Application application)
    {
        this.application = application;

        DateClass dateClass = DateClass.getInstance();
        conclusionDate = dateClass.date;
        expireDate = new DateOnly(conclusionDate.Day, conclusionDate.Month, conclusionDate.Year).AddYears(1);
        maxPatentDuration = new DateOnly(conclusionDate.Day, conclusionDate.Month, conclusionDate.Year).AddYears(20);

        isExpired = false;
    }    
}
