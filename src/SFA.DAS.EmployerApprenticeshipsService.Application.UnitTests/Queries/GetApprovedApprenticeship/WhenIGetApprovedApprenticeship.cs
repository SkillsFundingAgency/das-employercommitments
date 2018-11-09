using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprovedApprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetApprovedApprenticeship
{
    [TestFixture]
    public class WhenIGetApprovedApprenticeship
    {
        private GetApprovedApprenticeshipQueryHandler _handler;
        private GetApprovedApprenticeshipQueryRequest _validRequest;
        private ApprovedApprenticeship _apiResult;
        private Mock<IEmployerCommitmentApi> _commitmentApi;

        [SetUp]
        public void Arrange()
        {
            _apiResult = new ApprovedApprenticeship();

            _commitmentApi = new Mock<IEmployerCommitmentApi>();
            _commitmentApi.Setup(x => x.GetApprovedApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_apiResult);

            _handler = new GetApprovedApprenticeshipQueryHandler(_commitmentApi.Object);

            _validRequest = new GetApprovedApprenticeshipQueryRequest
            {
                ApprovedApprenticeshipId = 1
            };
        }

        [Test]
        public async Task ThenTheApprovedApprenticeshipReturnedByTheApiIsRetrieved()
        {
            var result = await _handler.Handle(TestHelper.Clone(_validRequest));

            Assert.AreEqual(_apiResult, result.ApprovedApprenticeship);
        }
    }
}