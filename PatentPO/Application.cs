namespace PatentPO;
class Application {
    public string inventionName {get; private set;}
    public string essay {get; private set;}
    public ApplicationStatus status {get; private set;}    

    public Application(string inventionName, string essay) {
        this.inventionName = inventionName;        
        this.essay = essay;
        status = ApplicationStatus.New;
    }
}