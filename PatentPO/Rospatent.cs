using System.Linq;

namespace PatentPO;
//Singleton
public class Rospatent
{
    private static Rospatent? instance;
    private static object syncRoot = new Object();
    public static uint registrationFee = 2000;
    public static uint firstExpertiseFee = 3500;
    public static uint secondExpertiseFee = 9000;
    public static uint patentGrantFee = 1500;
    public static uint patentExtentiosFee = 5000;
    public static ushort secondExpertiseLength = 3;

    private List<Application> applications = new List<Application>();
    internal List<Patent> patents { get; private set; } = new List<Patent>();
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
        application.status = ApplicationStatus.AwaitRegistrationPayment;
        application.checks.Add(new Check(application.client, this, CheckType.RegistrationFee, Rospatent.registrationFee,
                                           application, RegisterApplication));
    }
    internal void SendCheckForFirstExpertise(Application application)
    {
        application.status = ApplicationStatus.AwaitFirstExpertisePayment;
        application.checks.Add(new Check(application.client, this, CheckType.FirstExpertiseFee, Rospatent.firstExpertiseFee,
                                           application, AllocatePeople));
    }
    internal void SendCheckForSecondExpertise(Application application)
    {
        application.status = ApplicationStatus.AwaitSecondExpertisePayment;
        application.checks.Add(new Check(application.client, this, CheckType.SecondExpertiseFee, Rospatent.secondExpertiseFee,
                                           application, AllocatePeople));
    }
    internal void SendCheckForPatentGrant(Patent patent)
    {
        patent.patentChecks.Add(new Check(patent.application.client, this, CheckType.PatentGrantingFee, Rospatent.patentGrantFee,
                                           patent, GivePatentToClient));
    }
    internal void SendCheckPatentExtention(Patent patent)
    {
        patent.application.client.patentChecks.Add(new Check(patent.application.client, this, CheckType.ExtensionPatentPayment, Rospatent.patentExtentiosFee, 
            patent, ExtendPatent));
    }
    internal bool RegisterApplication(Application application)
    {
        application.status = ApplicationStatus.Registration;
        applications.Add(application);        

        SendCheckForFirstExpertise(application);

        return true;
    }

    internal bool AllocatePeople(Application application)
    {
        if (application.status == ApplicationStatus.AwaitFirstExpertisePayment) application.status = ApplicationStatus.AwaitFirstExpertise;

        if (application.expertise == null && application.status == ApplicationStatus.AwaitFirstExpertise)
        {
            var availableExperts = GetFreeExpert(1);
            if (availableExperts != null && availableExperts.Count == 1)
            {
                application.status = ApplicationStatus.FirstExpertise;
                Application.Expertise expertise = new Application.Expertise(application);
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

            if (currentApplication.status == ApplicationStatus.AwaitFirstExpertise ||
                currentApplication.status == ApplicationStatus.AwaitSecondExpertise)
            {
                var success = AllocatePeople(currentApplication);
                if (!success) return;
            }
        }
    }
    internal void CreatePatent(Application application)
    {
        if (application.patent != null) {
            application.client.notificationList.Add(application.inventionName + " - уже есть патент!");    
        }
        this.patents.Add(new Patent(application));
        SendCheckForPatentGrant(this.patents.Last());
    }
    internal Patent GivePatentToClient(Patent patent)
    {
        var application = patent.application;
        var client = application.client;        

        application.patent = patent;
        client.patents.Add(patent);
        return patent;
    }
    internal void GiveRightForPatent(Patent patent, Client payerClient) {    
        patent.members.Add(payerClient);
        payerClient.membershipPatents.Add(patent);
    }
    public bool ExtendPatent(Patent patent) {
        if (patent.isExpired) return false;

        patent.expireDate.AddYears(1);
        if (patent.expireDate >= patent.maxPatentDuration) {
            patent.isExpired = true;
            return false;
        }

        patent.application.client.notificationList.Add(patent.application.inventionName + " был продлён");        

        return true;
    }
}