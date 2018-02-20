using System.Collections.Generic;
using System.Threading.Tasks;
//using AutoFixture.NUnit3;
using NUnit.Framework;
using Moq;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingAcknowledgementModelForExistingCommitment : OrchestratorTestBase
    {
        // test won't run like this, may be an issue with having a base class
        //[Test, AutoData]
        //public async Task ThenIsTransferShouldBeSetWhenTransferSenderIdHasValue(GetCommitmentQueryResponse getCommitmentQueryResponse)
        //{
        //    getCommitmentQueryResponse.Commitment.TransferSenderId = 1;

        [Test]
        public async Task ThenIsTransferShouldBeSetWhenTransferSenderIdHasValue()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        TransferSenderId = 1,
                        Messages = new List<MessageView>()
                    }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetAcknowledgementModelForExistingCommitment("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.IsTrue(result.Data.IsTransfer);
        }

        [Test]
        public async Task ThenIsTransferShouldntBeSetWhenTransferSenderIdHasNoValue()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        TransferSenderId = null,
                        Messages = new List<MessageView>()
                    }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetAcknowledgementModelForExistingCommitment("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.IsFalse(result.Data.IsTransfer);
        }
    }
}