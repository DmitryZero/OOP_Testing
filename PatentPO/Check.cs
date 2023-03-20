using System;

namespace PatentPO;
public class Check<T> where T: class {
    internal ICheckParticipant payer {get; private set;}
    internal ICheckParticipant sender {get; private set;}
    public ulong summ {get; private set;}
    private Action<T>? callback;
    private T checkSubject;
    public CheckStatus status {get; private set;}
    public CheckType checkType {get; private set;}
    internal Check(ICheckParticipant payer, ICheckParticipant sender, 
                   CheckType checkType, ulong summ, T checkSubject, Action<T> callback) {
        this.payer = payer;
        this.sender = sender;
        this.summ = summ;
        this.checkType = checkType;

        this.checkSubject = checkSubject;      

        status = CheckStatus.PendingPayment;

        this.callback = callback;
    }
    internal void PayCheck() {
        if (checkType == CheckType.RegistrationFee) PayRegistrationFee();
    }
    private void PayRegistrationFee() {
        status = CheckStatus.Payed;
        if (callback != null) callback(checkSubject);        
    }

}