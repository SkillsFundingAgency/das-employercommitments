using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferringEntities;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetAccountTransferringEntitiesTests
{
    public class WhenIGetAccountTransferringEntities : QueryBaseTest<GetAccountTransferringEntitiesHandler, GetAccountTransferringEntitiesRequest, GetAccountTransferringEntitiesResponse>
    {
        private Mock<IEmployerAccountService> _employerAccountService;
        public override GetAccountTransferringEntitiesRequest Query { get; set; }
        public override GetAccountTransferringEntitiesHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTransferringEntitiesRequest>> RequestValidator { get; set; }

        private const string ExpectedAccountId = "AFV45TGB";
        private const string ExpectedUserId = "LKJ678UYT";

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountService = new Mock<IEmployerAccountService>();
            _employerAccountService.Setup(x => x.GetTransferConnectionsForAccount(It.IsAny<string>()))
                .ReturnsAsync(new List<TransferringEntity> {new TransferringEntity()});

            Query = new GetAccountTransferringEntitiesRequest {HashedAccountId =  ExpectedAccountId, UserId = ExpectedUserId};

            RequestHandler = new GetAccountTransferringEntitiesHandler(RequestValidator.Object, _employerAccountService.Object);
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
            Assert.AreEqual(1, actual.TransferringEntities.Count);
        }

        [Test]
        public void ThenIfTheValidationResultReturnsErrorsAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetAccountTransferringEntitiesRequest>()))
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
