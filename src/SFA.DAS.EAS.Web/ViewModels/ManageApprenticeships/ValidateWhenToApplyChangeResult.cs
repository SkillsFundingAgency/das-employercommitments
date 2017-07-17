using System;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public sealed class ValidateWhenToApplyChangeResult
    {
        public ValidationResult ValidationResult { get; set; }
        public DateTime DateOfChange { get; internal set; }
    }
}