using System;

namespace PatentPO;
public class Check {
    public Client payer {get; private set;}
    public Client sender {get; private set;}
    public ulong summ {get; private set;}
    public DateOnly dueDate {get; private set;}
}