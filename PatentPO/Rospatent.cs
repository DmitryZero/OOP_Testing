using System.Linq;

namespace PatentPO;
public class Rospatent : ICheckParticipant {
    private static Rospatent? instance;
    private static object syncRoot = new Object();
    public static uint registrationFee = 3500;

    private List<Application> applications = new List<Application>(); 
    private List<Expertise> expertises = new List<Expertise>();
    public List<Patent>? patents {get; private set;} 
    public List<Expert> experts {get; set;} = new List<Expert>();
    // public Check[]? checks {get; private set;}
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
    public void SendNotification(Client client) {

    }
    public void SendCheck(Client client, uint summ) {

    }
    internal Check<Application> RegisterApplication(Application application, Client client) {
        applications.Append(application);  

        var check = new Check<Application>(client, this, CheckType.RegistrationFee, Rospatent.registrationFee,
                                           application, AllocatePeopleForFirstExpertise);
        return check;
    }

    private void AllocatePeopleForFirstExpertise(Application application) {
        Expertise expertise = new Expertise(application.client, application);    
        var availableExperts = GetFreeExpert(1);

        if (availableExperts != null) {
            expertise.LaunchFirstExpertise(availableExperts[0]);
        } else expertise.PostponeExpertise();   
    }
    
    private List<Expert>? GetFreeExpert(ulong number) {
        if (experts == null) return null;

        List<Expert> currentExperts = new List<Expert>();

        for (int i = 0; i < experts.Count; i++) {
            var expert = experts[i];
            bool isAvailable = (expert.expertises == null || expert.expertises.All(item => 
                                                item.expertiseStatus != ExpertiseStatus.FirstExpertise &&
                                                item.expertiseStatus != ExpertiseStatus.SecondExpertise &&
                                                item.expertiseStatus != ExpertiseStatus.InThePlans));
            if (isAvailable) currentExperts.Add(experts[i]);
        }

       return (currentExperts.Count == 0 ? null : currentExperts);
    }
}