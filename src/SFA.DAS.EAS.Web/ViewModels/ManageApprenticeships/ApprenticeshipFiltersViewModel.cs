using System.Collections.Generic;
using Microsoft.Ajax.Utilities;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class ApprenticeshipFiltersViewModel
    {
        public ApprenticeshipFiltersViewModel()
        {
            Status = new List<string>();
            RecordStatus = new List<string>();
            Provider = new List<string>();
            Course = new List<string>();
            FundingStatus = new List<string>();

            ApprenticeshipStatusOptions = new List<KeyValuePair<string, string>>();
            TrainingCourseOptions = new List<KeyValuePair<string, string>>();
            RecordStatusOptions = new List<KeyValuePair<string, string>>();
            ProviderOrganisationOptions = new List<KeyValuePair<string, string>>();
            FundingStatusOptions = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ApprenticeshipStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> TrainingCourseOptions { get; set; }
        public List<KeyValuePair<string, string>> RecordStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> ProviderOrganisationOptions { get; set; }
        public List<KeyValuePair<string, string>> FundingStatusOptions { get; set; }

        public List<string> Status { get; set; }
        public List<string> RecordStatus { get; set; }
        public List<string> Provider { get; set; }
        public List<string> Course { get; set; }
        public List<string> FundingStatus { get; set; }

        public int PageNumber { get; set; }
        public string SearchInput { get; set; }
        public bool ResetFilter { get; set; }
        public bool CheckCookie { get; set; }

        public bool HasValues()
        {
            return ApprenticeshipStatusOptions.Count > 0
                || FundingStatusOptions.Count > 0
                || ProviderOrganisationOptions.Count > 0
                || RecordStatusOptions.Count > 0
                || TrainingCourseOptions.Count > 0
                || Status.Count > 0
                || RecordStatus.Count > 0
                || Provider.Count > 0
                || Course.Count > 0
                || FundingStatus.Count > 0
                || !SearchInput.IsNullOrWhiteSpace()
                || PageNumber > 0;
        }
    }
}