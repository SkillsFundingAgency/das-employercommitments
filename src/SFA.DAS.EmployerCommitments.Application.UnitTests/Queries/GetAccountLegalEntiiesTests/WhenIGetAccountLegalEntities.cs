using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetAccountLegalEntiiesTests
{
    public class WhenIGetAccountLegalEntities : QueryBaseTest<GetAccountLegalEntitiestHandler, GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        private Mock<IEmployerAccountService> _employerAccountService;
        public override GetAccountLegalEntitiesRequest Query { get; set; }
        public override GetAccountLegalEntitiestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntitiesRequest>> RequestValidator { get; set; }

        private const string ExpectedAccountId = "AFV45TGB";
        private const string ExpectedUserId = "LKJ678UYT";

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountService = new Mock<IEmployerAccountService>();
            _employerAccountService.Setup(x => x.GetLegalEntitiesForAccount(ExpectedAccountId))
                .ReturnsAsync(new List<LegalEntity> {new LegalEntity()});

            Query = new GetAccountLegalEntitiesRequest {HashedAccountId =  ExpectedAccountId, UserId = ExpectedUserId};

            RequestHandler = new GetAccountLegalEntitiestHandler(RequestValidator.Object, _employerAccountService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAccountService.Verify(x=>x.GetLegalEntitiesForAccount(ExpectedAccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.LegalEntities.Count);
        }

        [Test]
        public void ThenIfTheValidationResultReturnsUnauthorizedAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new ValidationResult
                {
                    IsUnauthorized = true,
                    ValidationDictionary = new Dictionary<string, string>()
                });

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async() => await RequestHandler.Handle(Query));
        }
    }
}
