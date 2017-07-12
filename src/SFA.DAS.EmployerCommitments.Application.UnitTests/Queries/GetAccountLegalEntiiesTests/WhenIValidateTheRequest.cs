using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetAccountLegalEntiiesTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountLegalEntitiesValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountLegalEntitiesValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountLegalEntitiesRequest { HashedAccountId = "123ABD", UserId = "123HGB", SignedOnly = false});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheRequestIsNotPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountLegalEntitiesRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
        }

    }
}
