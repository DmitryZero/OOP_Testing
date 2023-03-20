namespace PatentPO;
class Expertise {
    public Expert? firstExpert {get; private set;}
    public Expert[]? secondExperts {get; private set;}
    public ExpertiseStatus expertiseStatus {get; private set;}
    public Client client {get; private set;}
    public Application application {get; private set;}
    public Expertise(Client client, Application application) {
        this.client = client;
        this.application = application;
    }
    public void PostponeExpertise() {
        expertiseStatus = ExpertiseStatus.InThePlans;
    }
    public void LaunchFirstExpertise(Expert expert) {
        firstExpert = expert;
        expert.expertises.Append(this);

        expertiseStatus = ExpertiseStatus.FirstExpertise;

        var rospatent = Rospatent.getInstance();        
        firstExpert.executeFirstExpertise();
    }
    public void LaunchSecondExpertise(Expert[] experts) {
        secondExperts = secondExperts;
        expertiseStatus = ExpertiseStatus.SecondExpertise;
    }
}