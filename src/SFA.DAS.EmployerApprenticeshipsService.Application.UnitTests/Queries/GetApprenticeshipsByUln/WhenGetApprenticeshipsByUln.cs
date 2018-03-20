using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipsByUln;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetApprenticeshipsByUln
{
    public class WhenGetApprenticeshipsByUln
    {
        private GetApprenticeshipsByUlnHandler _handler;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;

        private const long AccountId = 1L;
        private const string Uln = "Uln";

        [SetUp]
        public void Setup()
        {
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _commitmentsApi.Setup(x => x.GetActiveApprenticeshipsForUln(It.IsAny<long>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<Apprenticeship>() { });

            _handler = new GetApprenticeshipsByUlnHandler(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalledCorrectly()
        {
            var request = new GetApprenticeshipsByUlnRequest
            {
                AccountId = AccountId,
                Uln = Uln
            };

            await _handler.Handle(request);

            _commitmentsApi.Verify(
                x => x.GetActiveApprenticeshipsForUln(
                    It.Is<long>(c => c.Equals(AccountId)),
                    It.Is<string>(c => c.Equals(Uln))),
                    Times.Once);
        }

        [Test]
        public async Task ThenShouldOnlyReturnActiveRecords()
        {
            var request = new GetApprenticeshipsByUlnRequest
            {
                AccountId = 1L,
                Uln = ""
            };

            var apprenticeship1 = new Apprenticeship() { Id = 1L, PaymentStatus = PaymentStatus.Active };
            var apprenticeship2 = new Apprenticeship() { Id = 2L, PaymentStatus = PaymentStatus.Completed };
            var apprenticeship3 = new Apprenticeship() { Id = 3L, PaymentStatus = PaymentStatus.PendingApproval };
            var apprenticeship4 = new Apprenticeship() { Id = 4L, PaymentStatus = PaymentStatus.Withdrawn };
            var apprenticeship5 = new Apprenticeship() { Id = 5L, PaymentStatus = PaymentStatus.Deleted };

            _commitmentsApi.Setup(x => x.GetActiveApprenticeshipsForUln(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Apprenticeship>() { apprenticeship1, apprenticeship2, apprenticeship3, apprenticeship4, apprenticeship5 });

            var response = await _handler.Handle(request);

            response.Apprenticeships.Count.Should().Be(1);
        }
    }
}
