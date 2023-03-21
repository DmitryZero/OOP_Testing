using System;

namespace PatentPO;
public class Check {
    internal ICheckParticipant payer {get; private set;}
    internal ICheckParticipant sender {get; private set;}
    public ulong summ {get; private set;}
    private Func<Application, bool>? callbackApplication;
    private Func<Patent, Patent>? callbackPatent;
    private Application? application;
    private Patent? patent;
    public CheckStatus status {get; private set;}
    public CheckType checkType {get; private set;}

    internal Check(ICheckParticipant payer, ICheckParticipant sender, 
                   CheckType checkType, ulong summ, Patent patent, Func<Patent, Patent> callback) {
        this.payer = payer;
        this.sender = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.patent = patent;      

        status = CheckStatus.PendingPayment;
        this.callbackPatent = callback;
    }
    internal Check(ICheckParticipant payer, ICheckParticipant sender, 
                   CheckType checkType, ulong summ, Application application, Func<Application, bool> callback) {
        this.payer = payer;
        this.sender = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.application = application;      

        status = CheckStatus.PendingPayment;
        this.callbackApplication = callback;
    }    
    internal void PayCheck() {
        status = CheckStatus.Payed;
        if (callbackApplication != null && application != null) callbackApplication(application);  
        if (callbackPatent != null && patent != null) callbackPatent(patent);  
    }
}