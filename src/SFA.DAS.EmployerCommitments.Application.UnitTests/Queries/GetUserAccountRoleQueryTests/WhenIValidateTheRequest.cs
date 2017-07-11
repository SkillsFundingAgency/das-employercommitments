using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetUserAccountRoleQueryTests
{
    public class WhenIValidateTheRequest
    {
        private GetUserAccountRoleValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetUserAccountRoleValidator();
        }

        [Test]
        public void ThenWhenAllFieldsArePopulatedTheResultIsValid()
        {
            //Act
            var actual = _validator.Validate(new GetUserAccountRoleQuery {HashedAccountId = "123RFV",UserId="IYTP123"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenNoFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetUserAccountRoleQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("UserId","UserId has not been supplied"),actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"),actual.ValidationDictionary);

        }

    }
}
