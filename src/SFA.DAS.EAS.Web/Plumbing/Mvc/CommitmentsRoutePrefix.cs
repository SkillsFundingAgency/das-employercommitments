using Microsoft.Azure;
using System;
using System.Web.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc
{
    public class CommitmentsRoutePrefixAttribute : RoutePrefixAttribute
    {
        public CommitmentsRoutePrefixAttribute() : base()
        {
        }

        public CommitmentsRoutePrefixAttribute(string prefix) : base(GetPrefix(prefix))
        {
        }

        private static string GetPrefix(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix), "Cannot be null");

            var commitmentsPrefix = CloudConfigurationManager.GetSetting("CommitmentsRoutePrefix");

            if (!string.IsNullOrWhiteSpace(commitmentsPrefix))
                return $"{commitmentsPrefix}/{prefix}";
            else 
                return prefix;
        }
    }
}