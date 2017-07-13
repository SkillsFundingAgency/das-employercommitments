using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetAccountLegalEntiiesTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountLegalEntitiesValidator _validator;
        private Mock<IEmployerAccountService> _employerAccountApi;

        [SetUp]
        public void Arrange()
        {
            _employerAccountApi = new Mock<IEmployerAccountService>();

            _employerAccountApi.Setup(x => x.GetUserRoleOnAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TeamMember {UserRef = "ref"});

            _validator = new GetAccountLegalEntitiesValidator(_employerAccountApi.Object);
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountLegalEntitiesRequest { HashedAccountId = "123ABD", UserId = "123HGB", SignedOnly = false});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheRequestIsNotPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountLegalEntitiesRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
            _employerAccountApi.Verify(x => x.GetUserRoleOnAccount(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenIfTheUSerIsNotConnectedtoTheAccountTheValidationResultIsMarkedAsNotAuthorized()
        {
            //Arrange
            var expectedAccountId = "123ABD";
            var expectedUserId = "123HGB";
            _employerAccountApi.Setup(x => x.GetUserRoleOnAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountLegalEntitiesRequest { HashedAccountId = expectedAccountId, UserId = expectedUserId, SignedOnly = false });

            //Assert
            _employerAccountApi.Verify(x => x.GetUserRoleOnAccount(expectedAccountId, expectedUserId), Times.Once);
            Assert.IsTrue(actual.IsUnauthorized);
        }

    }
}
