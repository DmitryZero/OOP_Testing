namespace PatentPO;
public class Rospatent {
    private static Rospatent? instance;
    private static object syncRoot = new Object();

    private Rospatent()
    {}

    public static Rospatent getInstance()
    {
        if (instance == null)
        {
            lock (syncRoot)
            {
                if (instance == null)
                    instance = new Rospatent();
            }
        }
        return instance;
    }

    private Application[]? applications {get; set;} 
    public Patent[]? patents {get; private set;} 
    public Expert[]? experts {get; private set;}
    public Check[]? checks {get; private set;}
    public void SendNotification(Client client) {

    }
    public void SendCheck(Check check, Client client) {

    }
}