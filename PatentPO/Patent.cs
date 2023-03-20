namespace PatentPO;
public class Patent {
    public Client? owner {get; private set;}
    public DateOnly conclusionDate {get; private set;}
}