using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetCommitment
{
    public class WhenIGetACommitment
    {
        private Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        private CommitmentView _commitmentView;
        public GetCommitmentQueryRequest _query { get; set; }
        public GetCommitmentQueryHandler _requestHandler { get; set; }

        private const long ExpectedAccountId = 123;
        private const long ExpectedCommitmentId = 879;

        [SetUp]
        public void Arrange()
        {
            _commitmentView = new CommitmentView();
            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _employerCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitmentView);

            _employerCommitmentApi.Setup(x => x.GetTransferSenderCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitmentView);

            _query = new GetCommitmentQueryRequest { AccountId =  ExpectedAccountId, CommitmentId = ExpectedCommitmentId};

            _requestHandler = new GetCommitmentQueryHandler(_employerCommitmentApi.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledForEmployer()
        {
            //Act
            var result = await _requestHandler.Handle(_query);

            //Assert
            _employerCommitmentApi.Verify(x=>x.GetEmployerCommitment(ExpectedAccountId, ExpectedCommitmentId));
            Assert.AreSame(_commitmentView, result.Commitment);
        }

        [Test]
        public async Task ThenTheApiIsCalledForEmployerExplicitly()
        {
            // Arrange
            _query.CallType = CallType.Employer;

            //Act
            var result = await _requestHandler.Handle(_query);

            //Assert
            _employerCommitmentApi.Verify(x => x.GetEmployerCommitment(ExpectedAccountId, ExpectedCommitmentId));
            Assert.AreSame(_commitmentView, result.Commitment);
        }

        [Test]
        public async Task ThenTheApiIsCalledForTransferSenderExplicitly()
        {
            // Arrange
            _query.CallType = CallType.TransferSender;

            //Act
            var result = await _requestHandler.Handle(_query);

            //Assert
            _employerCommitmentApi.Verify(x => x.GetTransferSenderCommitment(ExpectedAccountId, ExpectedCommitmentId));
            Assert.AreSame(_commitmentView, result.Commitment);
        }
    }
}
