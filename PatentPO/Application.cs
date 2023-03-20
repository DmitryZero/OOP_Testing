namespace PatentPO;
public class Application {
    public string inventionName {get; private set;}
    public string essay {get; private set;}
    public ApplicationStatus status {get; set;}    
    public Expertise? expertise {get; set;}
    public Client client {get; private set;}
    public Check<Application>? checkApplication {get; set;}

    public Application(string inventionName, string essay, Client client) {
        this.inventionName = inventionName;        
        this.essay = essay;
        status = ApplicationStatus.Registration;

        this.client = client;
    }
}