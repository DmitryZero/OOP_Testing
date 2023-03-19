namespace PatentPO;
public class Client {
    public string? fullName {get; private set;} 
    private Application[]? applications {get; set;} 

    public Client(string fullName) {
        this.fullName = fullName;        
    }
    public Patent[]? patents {get; private set;} 
    public Check[]? checks {get; private set;}

    internal void SendApplication() {
        
    }
    public void PayCheck(Check check) {

    }
    public void SendCheck(Check check, Client client) {

    }
}