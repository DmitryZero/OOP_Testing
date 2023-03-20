namespace PatentPO;
public class Client : ICheckParticipant {
    public string? fullName {get; private set;} 
    public List<Application> applicationOnExpertise {get; private set;} = new List<Application>();

    public Client(string fullName) {
        this.fullName = fullName;        
    }
    public List<Patent>? patents {get; private set;} 

    public Application SendApplication(string inventionName, string essay) {
        var application = new Application(inventionName, essay, this);
        applicationOnExpertise.Add(application);
        
        Rospatent rospatent = Rospatent.getInstance();
        var check = rospatent.RegisterApplication(application, this);
        application.checkApplication = check;

        return application;
    }
    public void PayFee(Check<Application> check) {
        check.PayCheck();
    }
}