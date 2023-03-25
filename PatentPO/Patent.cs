namespace PatentPO;

public class Patent
{
    public Application application { get; set; }
    public List<Check> patentChecks { get; set; } = new List<Check>();
    public List<Client> members { get; set; } = new List<Client>();
    public DateOnly conclusionDate { get; private set; }
    public DateOnly expireDate { get; private set; }
    public DateOnly maxPatentDuration;
    private bool isExpired = false;
    public bool GetIsExpired() {return isExpired;}
    public void ExtendPatent() {        
        expireDate = new DateOnly(expireDate.Year, expireDate.Month, expireDate.Day).AddYears(1);
    }
    public bool IsExtendPossible() {
        if (isExpired) return false;

        DateClass dateClass = DateClass.getInstance();
        var nextExpireDate = new DateOnly(expireDate.Year, expireDate.Month, expireDate.Day).AddYears(1);
        return (!(nextExpireDate > maxPatentDuration));
    }
    public Patent(Application application)
    {
        this.application = application;

        DateClass dateClass = DateClass.getInstance();
        conclusionDate = dateClass.date;
        expireDate = new DateOnly(conclusionDate.Year, conclusionDate.Month, conclusionDate.Day).AddYears(1);
        maxPatentDuration = new DateOnly(conclusionDate.Year, conclusionDate.Month, conclusionDate.Day).AddYears(20);

        isExpired = false;
    }
}
