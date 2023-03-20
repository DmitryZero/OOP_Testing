namespace PatentPO;
public class Client : ICheckParticipant {
    public string? fullName {get; private set;} 
    private List<Application> applications = new List<Application>();

    public Client(string fullName) {
        this.fullName = fullName;        
    }
    public List<Patent>? patents {get; private set;} 
    // public List<Check<Application>>? checksForApplication {get; private set;}

    public Check<Application> SendApplication(string inventionName, string essay) {
        var application = new Application(inventionName, essay, this);
        applications.Add(application);
        
        Rospatent rospatent = Rospatent.getInstance();
        var check = rospatent.RegisterApplication(application, this);

        return check;
    }
    public void PayFee(Check<Application> check) {
        check.PayCheck();
    }
}