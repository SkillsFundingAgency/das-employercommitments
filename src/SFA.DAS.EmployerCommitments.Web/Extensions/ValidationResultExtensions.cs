using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Web.Extensions
{
    public static class ValidationResultExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            if (!result.IsValid())
            {
                foreach (var error in result.ValidationDictionary)
                {
                    modelState.AddModelError(error.Key, error.Value);
                }
            }
        }
    }
}