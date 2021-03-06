﻿using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests
{
    public static class TestHelper
    {
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool EnumerablesAreEqual(IEnumerable<object> expected, IEnumerable<object> actual)
        {
            return new CompareLogic(new ComparisonConfig { IgnoreObjectTypes = true })
                .Compare(expected, actual).AreEqual;
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
