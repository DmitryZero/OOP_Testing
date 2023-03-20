namespace PatentPO;
public class Expert
{
    public string fullName { get; private set; }
    internal List<Expertise> expertises { get; set; } = new List<Expertise>();
    internal Expertise? currentExpertise { get; set; }
    internal ExpertStatus expertStatus { get; set; } = ExpertStatus.Available;
    public Expert(string fullName)
    {
        this.fullName = fullName;
    }
    public void ApproveExpertise()
    {
        if (currentExpertise != null && currentExpertise.expertiseStatus == ExpertiseStatus.FirstExpertise)
        {
            var expertise = currentExpertise;
            currentExpertise.expertiseStatus = ExpertiseStatus.SecondExpertise;            
            currentExpertise = null;
            expertStatus = ExpertStatus.Available;

            var rospatent = Rospatent.getInstance();
            rospatent.AllocatePeopleForSecondExpertise(expertise);
        }
        if (currentExpertise != null && currentExpertise.expertiseStatus == ExpertiseStatus.SecondExpertise) {
            currentExpertise.expertiseStatus = ExpertiseStatus.Approved;
            currentExpertise.approvalList.Add(true);
            expertStatus = ExpertStatus.Available;
            
        } 

    }
    public void RejectExpertise()
    {
        if (currentExpertise != null && currentExpertise.expertiseStatus == ExpertiseStatus.FirstExpertise) {
            currentExpertise.expertiseStatus = ExpertiseStatus.Rejected;
            currentExpertise.approvalList.Add(false);
            expertStatus = ExpertStatus.Available;   
        }
        if (currentExpertise != null && currentExpertise.expertiseStatus == ExpertiseStatus.SecondExpertise) {
            currentExpertise.expertiseStatus = ExpertiseStatus.Rejected;
            currentExpertise.approvalList.Add(false);
            expertStatus = ExpertStatus.Available;            
        } 
    }
}