namespace SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggle
{
    public class FeatureToggleItem
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string[] WhiteList { get; set; }
    }
}