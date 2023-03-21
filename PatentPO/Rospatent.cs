using System.Linq;

namespace PatentPO;
//Singleton
public class Rospatent : ICheckParticipant
{
    private static Rospatent? instance;
    private static object syncRoot = new Object();
    public static uint registrationFee = 2000;
    public static uint firstExpertiseFee = 3500;
    public static uint secondExpertiseFee = 9000;
    public static uint patentGrant = 1500;
    public static ushort secondExpertiseLength = 3;

    private List<Application> applications = new List<Application>();
    internal Patent? patent { get; private set; }
    public List<Expert> experts { get; set; } = new List<Expert>();
    private Rospatent()
    { }

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
    internal void SendCheckForRegistration(Application application)
    {
        application.status = ApplicationStatus.awaitRegistrationPayment;
        application.registrationCheck = new Check(application.client, this, CheckType.RegistrationFee, Rospatent.registrationFee,
                                           application, RegisterApplication);
    }
    internal void SendCheckForFirstExpertise(Application application)
    {
        application.status = ApplicationStatus.awaitFirstExpertisePayment;
        application.firstExpertiseCheck = new Check(application.client, this, CheckType.FirstExpertiseFee, Rospatent.firstExpertiseFee,
                                           application, AllocatePeople);
    }
    internal void SendCheckForSecondExpertise(Application application)
    {
        application.status = ApplicationStatus.AwaitSecondExpertisePayment;
        application.secondExpertiseCheck = new Check(application.client, this, CheckType.SecondExpertiseFee, Rospatent.secondExpertiseFee,
                                           application, AllocatePeople);
    }
    internal void SendCheckForPatent(Patent patent)
    {
        patent.patentChecks.Add(new Check(patent.application.client, this, CheckType.RegistrationFee, Rospatent.registrationFee,
                                           patent, GivePatent));
    }
    internal bool RegisterApplication(Application application)
    {
        applications.Add(application);
        application.status = ApplicationStatus.awaitFirstExpertisePayment;

        SendCheckForFirstExpertise(application);

        return true;
    }

    internal bool AllocatePeople(Application application)
    {
        if (application.expertise == null && application.status == ApplicationStatus.awaitFirstExpertise)
        {
            var availableExperts = GetFreeExpert(1);
            if (availableExperts != null && availableExperts.Count == 1)
            {
                application.status = ApplicationStatus.FirstExpertise;
                Application.Expertise expertise = new Application.Expertise(application.client, application);
                application.expertise = expertise;


                availableExperts[0].currentExpertise = expertise;
                availableExperts[0].expertStatus = ExpertStatus.Busy;
                expertise.firstExpert = availableExperts[0];

                return true;
            }
        }

        if (application.expertise != null && application.status == ApplicationStatus.AwaitSecondExpertise)
        {
            var availableExperts = GetFreeExpert(secondExpertiseLength);

            if (availableExperts != null && availableExperts.Count == secondExpertiseLength)
            {
                application.status = ApplicationStatus.SecondExpertise;
                experts.ForEach(expert => expert.expertStatus = ExpertStatus.Busy);
                application.expertise.secondExperts = availableExperts;

                return true;
            }
        }

        return false;
    }
    private List<Expert>? GetFreeExpert(ulong number)
    {
        if (experts == null) return null;

        List<Expert> currentExperts = new List<Expert>();

        for (int i = 0; i < experts.Count; i++)
        {
            var expert = experts[i];
            if (expert.expertStatus == ExpertStatus.Available) currentExperts.Add(experts[i]);
        }

        return (currentExperts.Count == 0 ? null : currentExperts);
    }
    internal void RelocatePeopleOnExpertiseEnd()
    {
        for (int i = 0; i < applications.Count; i++)
        {
            var currentApplication = applications[i];

            if (currentApplication.status == ApplicationStatus.awaitFirstExpertise ||
                currentApplication.status == ApplicationStatus.AwaitSecondExpertise)
            {
                var success = AllocatePeople(currentApplication);
                if (!success) return;
            }
        }
    }
    internal void CreatePatent(Application application)
    {
        this.patent = new Patent(application);
        SendCheckForPatent(this.patent);
    }
    internal Patent GivePatent(Patent patent)
    {
        patent.application.patent = patent;
        return patent;
    }
}