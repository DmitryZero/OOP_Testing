namespace PatentPO;
public class Expertise {
    public Expert? firstExpert {get; private set;}
    public Expert[]? secondExperts {get; private set;}
    public List<bool> approvalList {get; set;} = new List<bool>();
    public ExpertiseStatus expertiseStatus {get; set;}
    public Client client {get; private set;}
    public Application application {get; private set;}
    public Expertise(Client client, Application application) {
        this.client = client;
        this.application = application;

        application.expertise = this;
    }
    public void PostponeExpertise() {
        expertiseStatus = ExpertiseStatus.InThePlans;
    }
    public void LaunchFirstExpertise(Expert expert) {        
        var rospatent = Rospatent.getInstance();                
        expertiseStatus = ExpertiseStatus.FirstExpertise;    

        expert.expertises.Append(this);
        expert.currentExpertise = this;
        expert.expertStatus = ExpertStatus.Busy;
        firstExpert = expert;            
    }
    public void LaunchSecondExpertise(List<Expert> experts) {
        secondExperts = secondExperts;
        expertiseStatus = ExpertiseStatus.SecondExpertise;
    }
}