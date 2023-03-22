namespace PatentPO;
public class Expert
{
    public string fullName { get; private set; }
    internal Application.Expertise? currentExpertise { get; set; }
    internal ExpertStatus expertStatus { get; set; } = ExpertStatus.Available;
    public Expert(string fullName)
    {
        this.fullName = fullName;
    }
    public void ApproveExpertise()
    {
        if (currentExpertise != null && currentExpertise.application.status == ApplicationStatus.FirstExpertise)
        {
            var expertise = currentExpertise;
            currentExpertise = null;

            expertStatus = ExpertStatus.Available;

            var application = expertise.application;
            application.client.notificationList.Add(application.inventionName + " был одобрен на первой экспертизе!");

            var rospatent = Rospatent.getInstance();
            rospatent.SendCheckForSecondExpertise(expertise.application);
            rospatent.RelocatePeopleOnExpertiseEnd();
        }
        if (currentExpertise != null && currentExpertise.application.status == ApplicationStatus.SecondExpertise)
        {
            currentExpertise.approvalList.Add(true);

            var rospatent = Rospatent.getInstance();
            if (currentExpertise.approvalList.All(item => item == true) && currentExpertise.approvalList.Count == Rospatent.secondExpertiseLength)
            {
                var expertise = currentExpertise;

                expertise.application.status = ApplicationStatus.Approved;

                var application = expertise.application;
                application.client.notificationList.Add(application.inventionName + " был одобрен на второй экспертизе!");

                expertise?.secondExperts?.ForEach(expert =>
                {
                    expert.expertStatus = ExpertStatus.Available;
                    expert.currentExpertise = null;
                });

                if (expertise != null) rospatent.CreatePatent(expertise.application);

                return;
            }
            else currentExpertise.application.status = ApplicationStatus.Rejected;

            rospatent.RelocatePeopleOnExpertiseEnd();
        }
    }
    public void RejectExpertise()
    {
        if (currentExpertise != null && currentExpertise.application.status == ApplicationStatus.FirstExpertise)
        {
            currentExpertise.application.status = ApplicationStatus.Rejected;
            currentExpertise = null;
            expertStatus = ExpertStatus.Available;

            var application = currentExpertise?.application;
            application?.client.notificationList.Add(application.inventionName + " был отклонён на первой экспертизе!");   

            var rospatent = Rospatent.getInstance();
            rospatent.RelocatePeopleOnExpertiseEnd();
        }
        if (currentExpertise != null && currentExpertise.application.status == ApplicationStatus.SecondExpertise)
        {
            currentExpertise.application.status = ApplicationStatus.Rejected;

            var application = currentExpertise.application;
            application?.client.notificationList.Add(application.inventionName + " был отклонён на второй экспертизе!");   

            if (currentExpertise.secondExperts != null) currentExpertise.secondExperts.ForEach(item =>
            {
                item.expertStatus = ExpertStatus.Available;
                item.currentExpertise = null;
            });

            var rospatent = Rospatent.getInstance();
            rospatent.RelocatePeopleOnExpertiseEnd();
        }
    }
}