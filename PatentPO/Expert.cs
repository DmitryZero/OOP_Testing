namespace PatentPO;
public class Expert {
    public string fullName {get; private set;} 
    internal  List<Expertise> expertises {get; set;} = new List<Expertise>(); 
    public void executeFirstExpertise() {

    }    

    public Expert(string fullName) {
        this.fullName = fullName;
    }
}