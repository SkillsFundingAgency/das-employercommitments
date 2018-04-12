using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTransferRequest;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using CallerType = SFA.DAS.EmployerCommitments.Application.Queries.GetTransferRequest.CallerType;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingtransferRequestViewModelForTransferSender : OrchestratorTestBase
    {
        private TransferRequest _transferRequest;
        private TransferRequestViewModel _transferRequestViewModel;

        [SetUp]
        public void Arrange()
        {

            _transferRequest = new TransferRequest();
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetTransferRequestQueryRequest>()))
                .ReturnsAsync(new GetTransferRequestQueryResponse
                {
                    TransferRequest = _transferRequest
                });

            _transferRequestViewModel = new TransferRequestViewModel();
            MockCommitmentMapper.Setup(x => x.MapToTransferRequestViewModel(It.IsAny<TransferRequest>()))
                .Returns(_transferRequestViewModel);

            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123);
            MockHashingService.Setup(x => x.DecodeValue("XYZ789")).Returns(789);
        }

        [Test]
        public async Task ShouldCallMediatorWithExpectedQuery()
        {
            //Act
            await EmployerCommitmentOrchestrator.GetTransferRequestDetails("ABC123", "XYZ789", "UserA");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetTransferRequestQueryRequest>(c =>
                c.AccountId == 123 && c.TransferRequestId == 789 && c.CallerType == CallerType.TransferSender)));
        }

        [Test]
        public async Task ThenItShouldCallCommitmentMapperWithTransferRequestObject()
        {
            //Arrange 

            //Act
            await EmployerCommitmentOrchestrator.GetTransferRequestDetails("ABC123", "XYZ789", "UserA");

            //Assert
            MockCommitmentMapper.Verify(x=>x.MapToTransferRequestViewModel(_transferRequest));
        }

        [Test]
        public async Task ThenItShouldReturnTheMappedTransferRequestViewModel()
        {
            //Arrange 

            //Act
            var result = await EmployerCommitmentOrchestrator.GetTransferRequestDetails("ABC123", "XYZ789", "UserA");

            //Assert
            Assert.AreSame(_transferRequestViewModel, result.Data);
        }
    }
}
