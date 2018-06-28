using System.Collections.Generic;
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

        public static string PopulateTemplate(string template, Dictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                template = template.Replace($"(({token.Key}))", token.Value);
            }

            return template;
        }
    }
}
