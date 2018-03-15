using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingCommitmentViewModelForTransferSender : OrchestratorTestBase
    {
        private CommitmentView _commitmentView;
        private TransferCommitmentViewModel _transferCommitmentViewModel;

        [SetUp]
        public void Arrange()
        {

            _commitmentView = new CommitmentView();
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = _commitmentView
                });

            _transferCommitmentViewModel = new TransferCommitmentViewModel();
            MockCommitmentMapper.Setup(x => x.MapToTransferCommitmentViewModel(It.IsAny<CommitmentView>()))
                .Returns(_transferCommitmentViewModel);

            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123);
            MockHashingService.Setup(x => x.DecodeValue("XYZ789")).Returns(789);
        }

        [Test]
        public async Task ShouldCallMediatorWithExpectedQuery()
        {
            //Act
            await EmployerCommitmentOrchestrator.GetCommitmentDetailsForTransfer("ABC123", "XYZ789", "UserA");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetCommitmentQueryRequest>(c =>
                c.AccountId == 123 && c.CommitmentId == 789 && c.CallerType == CallerType.TransferSender)));
        }

        [Test]
        public async Task ThenItShouldCallCommitmentMapperWithCommitmentViewObject()
        {
            //Arrange 

            //Act
            await EmployerCommitmentOrchestrator.GetCommitmentDetailsForTransfer("ABC123", "XYZ789", "UserA");

            //Assert
            MockCommitmentMapper.Verify(x=>x.MapToTransferCommitmentViewModel(_commitmentView));
        }

        [Test]
        public async Task ThenItShouldReturnTheMappedTransferCommitmentViewModel()
        {
            //Arrange 

            //Act
            var result = await EmployerCommitmentOrchestrator.GetCommitmentDetailsForTransfer("ABC123", "XYZ789", "UserA");

            //Assert
            Assert.AreSame(_transferCommitmentViewModel, result.Data);
        }
    }
}
