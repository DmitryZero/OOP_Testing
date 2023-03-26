namespace PatentPO;
public partial class Application {
    public string inventionName {get; private set;}
    public string essay {get; private set;}
    public ApplicationStatus status {get; set;}        
    public Client client {get; private set;}
    public List<Check> checks {get; set;} = new List<Check>();
    public Patent? patent {get; set;}       
    public Application.Expertise? expertise {get; set;}

    public Application(string inventionName, string essay, Client client) {
        this.inventionName = inventionName;        
        this.essay = essay;
        status = ApplicationStatus.NewApplication;

        this.client = client;
        client.applications.Add(this);
    }
}