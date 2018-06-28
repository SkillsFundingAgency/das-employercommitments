using Newtonsoft.Json;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests
{
    public static class TestHelpers
    {
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
