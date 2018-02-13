using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetAccountTransferConnectionsTests
{
    public class WhenIGetAccountTransferringEntities : QueryBaseTest<GetAccountTransferConnectionsHandler, GetAccountTransferConnectionsRequest, GetAccountTransferConnectionsResponse>
    {
        private Mock<IEmployerAccountService> _employerAccountService;
        public override GetAccountTransferConnectionsRequest Query { get; set; }
        public override GetAccountTransferConnectionsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTransferConnectionsRequest>> RequestValidator { get; set; }

        private const string ExpectedAccountId = "AFV45TGB";
        private const string ExpectedUserId = "LKJ678UYT";

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountService = new Mock<IEmployerAccountService>();
            _employerAccountService.Setup(x => x.GetTransferConnectionsForAccount(It.IsAny<string>()))
                .ReturnsAsync(new List<TransferConnection> {new TransferConnection()});

            Query = new GetAccountTransferConnectionsRequest {HashedAccountId =  ExpectedAccountId, UserId = ExpectedUserId};

            RequestHandler = new GetAccountTransferConnectionsHandler(RequestValidator.Object, _employerAccountService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAccountService.Verify(x=>x.GetTransferConnectionsForAccount(ExpectedAccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.TransferConnections.Count);
        }

        [Test]
        public void ThenIfTheValidationResultReturnsErrorsAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetAccountTransferConnectionsRequest>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        { "Key", "value" }
                    }
                });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await RequestHandler.Handle(Query));

            // Assert
            RequestValidator.Verify(x=>x.Validate(Query), Times.Once);
        }
    }
}
