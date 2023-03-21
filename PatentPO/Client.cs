namespace PatentPO;
public class Client : ICheckParticipant {
    public string? fullName {get; private set;} 
    public List<Application> applications {get; private set;} = new List<Application>();

    public Client(string fullName) {
        this.fullName = fullName;        
    }
    internal List<Patent>? patents {get; private set;} 

    public Application SendApplication(string inventionName, string essay) {
        var application = new Application(inventionName, essay, this);
        applications.Add(application); //to
        
        Rospatent rospatent = Rospatent.getInstance();
        rospatent.SendCheckForRegistration(application);        

        return application;
    }
    public void PayFee(Check check) {
        check.PayCheck();
    }
}