namespace PatentPO;
public class Client {
    public string? fullName {get; private set;} 
    public List<Application> applications {get; set;} = new List<Application>();
    public List<Patent> patents {get; set;} = new List<Patent>();
    public List<Patent> membershipPatents {get; set;} = new List<Patent>();
    public List<Check> patentChecks {get; set;} = new List<Check>();
    public List<string> notificationList {get; set;} = new List<string>();

    public Client(string fullName) {
        this.fullName = fullName;        
    }

    public Application SendApplication(string inventionName, string essay) {
        var application = new Application(inventionName, essay, this);
        
        Rospatent rospatent = Rospatent.getInstance();
        rospatent.SendCheckForRegistration(application);        

        return application;
    }
    public bool SendCheckForMembership(Client member, Patent patent, uint summ) {
        Rospatent rospatent = Rospatent.getInstance();

        if (!patent.IsExtendPossible()) {            
            patent.application.client.notificationList.Add(patent.application.inventionName + " истёк и из-за этого на него могут быть переданы права!");   
            return false;
        }

        member.patentChecks.Add(new Check(member, this, CheckType.MembershipPayment, summ, patent, rospatent.GiveRightForPatent));
        return true;
    }
    public void PayFee(Check check) {
        check.PayCheck();
    }  
    public bool SendExtendPatentRequest(Patent patent) {
        Rospatent rospatent = Rospatent.getInstance();
        
        return rospatent.SendCheckPatentExtention(patent);
    }  
}