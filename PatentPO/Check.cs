using System;

namespace PatentPO;
public class Check
{
    public Client payerClient { get; private set; }
    public Client? senderClient { get; private set; }
    public Rospatent? senderRospatent { get; private set; }
    public uint summ { get; private set; }
    private Func<Application, bool>? callbackApplication;
    private Func<Patent, Patent>? callbackPatentSend;
    private Action<Patent, Client>? callbackMembership;
    private Func<Patent, bool>? callbackPatentExtension;
    public Application? application;
    public Patent? patent;
    public CheckStatus status { get; private set; }
    public CheckType checkType { get; private set; }          
    public Check(Client payer, Rospatent sender,
                   CheckType checkType, uint summ, Application application, Func<Application, bool> callbackApplication)
    {
        this.payerClient = payer;
        this.senderRospatent = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.application = application;

        status = CheckStatus.PendingPayment;
        this.callbackApplication = callbackApplication;
    }
    public Check(Client payer, Rospatent sender,
                   CheckType checkType, uint summ, Patent patent, Func<Patent, Patent> callbackPatentSend)
    {
        this.payerClient = payer;
        this.senderRospatent = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.patent = patent;

        status = CheckStatus.PendingPayment;
        this.callbackPatentSend = callbackPatentSend;
    }    
    public Check(Client payer, Client sender,
                   CheckType checkType, uint summ, Patent patent, Action<Patent, Client> callbackMembership)
    {
        this.payerClient = payer;
        this.senderClient = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.patent = patent;

        status = CheckStatus.PendingPayment;
        this.callbackMembership = callbackMembership;
    }    
    public Check(Client payer, Rospatent sender,
                   CheckType checkType, uint summ, Patent patent, Func<Patent, bool> callbackPatentExtension)
    {
        this.payerClient = payer;
        this.senderRospatent = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.patent = patent;

        status = CheckStatus.PendingPayment;
        this.callbackPatentExtension = callbackPatentExtension;
    }  
    public bool PayCheck()
    {
        status = CheckStatus.Payed;

        if ((checkType == CheckType.RegistrationFee || checkType == CheckType.FirstExpertiseFee ||
                checkType == CheckType.SecondExpertiseFee) && callbackApplication != null && application != null)
        {
            if (application.status == ApplicationStatus.AwaitFirstExpertisePayment) application.status = ApplicationStatus.AwaitFirstExpertise;            
            if (application.status == ApplicationStatus.AwaitSecondExpertisePayment) application.status = ApplicationStatus.AwaitSecondExpertise;            
            callbackApplication(application);
            return true;
        }

        if (checkType == CheckType.PatentGrantingFee && callbackPatentSend != null && patent != null) {
            callbackPatentSend(patent);
            return true;
        }

        if (checkType == CheckType.MembershipPayment && callbackMembership != null && patent != null) {
            callbackMembership(patent, payerClient);
            return true;
        }

        if (checkType == CheckType.ExtensionPatentPayment && callbackPatentExtension != null && patent != null) {
            callbackPatentExtension(patent);
            return true;
        }

        return false;
    }        
}