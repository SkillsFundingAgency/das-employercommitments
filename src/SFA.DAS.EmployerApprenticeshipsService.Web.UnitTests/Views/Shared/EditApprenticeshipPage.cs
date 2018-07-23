using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.Shared;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.Shared
{
    [TestFixture]
    public class EditApprenticeshipPage : ViewModelTestingBase<ExtendedApprenticeshipViewModel, EditApprenticeship>
    {
        private const string Uln = "IAMAULN";

        [Test]
        public void ShouldDisplayUln()
        {
            var model = new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = new ApprenticeshipViewModel { ULN = Uln },
                ApprenticeshipProgrammes = new List<ITrainingProgramme>(),
                ValidationErrors = new Dictionary<string, string>()
            };

            AssertParsedContent(model, "#uln", Uln);
        }

        [Test]
        public void ShouldNotDisplayUln()
        {
            var model = new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = new ApprenticeshipViewModel { ULN = string.Empty },
                ApprenticeshipProgrammes = new List<ITrainingProgramme>(),
                ValidationErrors = new Dictionary<string, string>()
            };

            AssertParsedContent(model, "#uln", string.Empty);
        }
    }
}