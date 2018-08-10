using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests
{
    public static class TestHelper
    {
        public static bool AreEqual(object obj1, object obj2)
        {
            var compare = new CompareLogic();
            var result = compare.Compare(obj1, obj2);
            return result.AreEqual;
        }

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
