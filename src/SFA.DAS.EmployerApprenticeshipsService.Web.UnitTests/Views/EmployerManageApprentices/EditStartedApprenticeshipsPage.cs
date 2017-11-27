using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerManageApprentices
{
    [TestFixture]
    public class EditStartedApprenticeshipPage : ViewModelTestingBase<ExtendedApprenticeshipViewModel, EditStartedApprenticeship>
    {
        private const string Uln = "IAMAULN";
        
        [Test]
        public void ShouldDisplayUln()
        {
            var model = new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = new ApprenticeshipViewModel {ULN = Uln},
                ApprenticeshipProgrammes = new List<ITrainingProgramme>(),
                ValidationErrors = new Dictionary<string, string>()
            };

            AssertParsedContent(model, "#uln", Uln);
        }
    }
}