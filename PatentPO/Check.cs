using System;

namespace PatentPO;
public class Check
{
    public Client payerClient { get; private set; }
    public Client? senderClient { get; private set; }
    public Rospatent? senderRospatent { get; private set; }
    public ulong summ { get; private set; }
    private Func<Application, bool>? callbackApplication;
    private Func<Patent, Patent>? callbackPatent;
    private Action<Patent, Client>? callbackMembership;
    private Application? application;
    private Patent? patent;
    public CheckStatus status { get; private set; }
    public CheckType checkType { get; private set; }    
    internal Check(Client payer, Rospatent sender,
                   CheckType checkType, ulong summ, Application application, Func<Application, bool> callbackApplication)
    {
        this.payerClient = payer;
        this.senderRospatent = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.application = application;

        status = CheckStatus.PendingPayment;
        this.callbackApplication = callbackApplication;
    }
    internal Check(Client payer, Rospatent sender,
                   CheckType checkType, ulong summ, Patent patent, Func<Patent, Patent> callbackPatent)
    {
        this.payerClient = payer;
        this.senderRospatent = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.patent = patent;

        status = CheckStatus.PendingPayment;
        this.callbackPatent = callbackPatent;
    }    
    internal Check(Client payer, Client sender,
                   CheckType checkType, ulong summ, Patent patent, Action<Patent, Client> callbackMembership)
    {
        this.payerClient = payer;
        this.senderClient = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.patent = patent;

        status = CheckStatus.PendingPayment;
        this.callbackMembership = callbackMembership;
    }
    public bool PayCheck()
    {
        status = CheckStatus.Payed;

        if ((checkType == CheckType.RegistrationFee || checkType == CheckType.FirstExpertiseFee ||
                checkType == CheckType.SecondExpertiseFee) && callbackApplication != null && application != null)
        {
            callbackApplication(application);
            return true;
        }

        if (checkType == CheckType.PatentGrantingFee && callbackPatent != null && patent != null) {
            callbackPatent(patent);
            return true;
        }

        if (checkType == CheckType.MembershipPayment && callbackMembership != null && patent != null) {
            callbackMembership(patent, payerClient);
            return true;
        }

        return false;
    }        
}