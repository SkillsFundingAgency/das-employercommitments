using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetAccountTransferringEntitiesTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountTransferConnectionsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountTransferConnectionsValidator();
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTransferConnectionsRequest { HashedAccountId = "123ABD", UserId = "123HGB", SignedOnly = false});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheRequestIsNotPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTransferConnectionsRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
        }
    }
}
