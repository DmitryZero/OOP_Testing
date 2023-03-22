namespace PatentPO;
public partial class Application {
    public string inventionName {get; private set;}
    public string essay {get; private set;}
    public ApplicationStatus status {get; set;}        
    public Client client {get; private set;}
    public Check? registrationCheck {get; set;}
    public Check? firstExpertiseCheck {get; set;}
    public Check? secondExpertiseCheck {get; set;} 
    public Patent? patent {get; set;}       
    internal Application.Expertise? expertise {get; set;}

    public Application(string inventionName, string essay, Client client) {
        this.inventionName = inventionName;        
        this.essay = essay;
        status = ApplicationStatus.newApplication;

        this.client = client;
    }
    public Expert? GetFirstExpertiseExpert() {
        if (expertise != null && expertise.firstExpert != null) {
            return expertise.firstExpert;
        }

        return null;
    }
    public List<Expert>? GetSecondExpertiseExperts() {
        if (expertise != null && expertise.secondExperts != null) {
            return expertise.secondExperts;
        }

        return null;
    }
}