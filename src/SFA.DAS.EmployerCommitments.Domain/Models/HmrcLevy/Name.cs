using Newtonsoft.Json;

namespace SFA.DAS.EmployerCommitments.Domain.Models.HmrcLevy
{
    public class Name
    {
        [JsonProperty("nameLine1")]
        public string EmprefAssociatedName { get; set; }
    }
}