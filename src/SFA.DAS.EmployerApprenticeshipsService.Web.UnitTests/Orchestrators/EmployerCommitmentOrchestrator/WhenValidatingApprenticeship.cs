using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenValidatingApprenticeship : OrchestratorTestBase
    {
        [Test]
        public async Task ThenEndDateShouldntBeValidatedIfNotSupplied()
        {
            MockApprenticeshipCoreValidator.Setup(v => v.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());

            var apprenticeship = new ApprenticeshipViewModel { EndDate = new DateTimeViewModel() };

            var result = await EmployerCommitmentOrchestrator.ValidateApprenticeship(apprenticeship);

            CollectionAssert.AreEqual(new Dictionary<string, string>(), result);
        }
    }
}
