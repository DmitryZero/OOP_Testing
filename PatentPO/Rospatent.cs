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
    internal Check<Application> RegisterApplication(Application application, Client client) {
        applications.Append(application);  

        var check = new Check<Application>(client, this, CheckType.RegistrationFee, Rospatent.registrationFee,
                                           application, AllocatePeopleForFirstExpertise);
        return check;
    }

    internal void AllocatePeopleForFirstExpertise(Application application) {
        Expertise expertise = new Expertise(application.client, application); 
        var availableExperts = GetFreeExpert(1);

        if (availableExperts != null) {
            expertise.LaunchFirstExpertise(availableExperts[0]);
        } else expertise.PostponeExpertise();   
    }

    internal void AllocatePeopleForSecondExpertise(Expertise expertise) {        
        var availableExperts = GetFreeExpert(3);

        if (availableExperts != null && availableExperts.Count == 3) {
            expertise.LaunchSecondExpertise(availableExperts);
        } else expertise.PostponeExpertise();   
    }    
    private List<Expert>? GetFreeExpert(ulong number) {
        if (experts == null) return null;

        List<Expert> currentExperts = new List<Expert>();

        for (int i = 0; i < experts.Count; i++) {
            var expert = experts[i];
            if (expert.expertStatus == ExpertStatus.Available) currentExperts.Add(experts[i]);
        }

       return (currentExperts.Count == 0 ? null : currentExperts);
    }
    public void AllocatePeopleUpdate() {
        
    }
}