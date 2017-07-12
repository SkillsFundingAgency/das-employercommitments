using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetUserAccountRoleQueryTests
{
    public class WhenIGetUserAccountDetails : QueryBaseTest<GetUserAccountRoleQueryHandler, GetUserAccountRoleQuery, GetUserAccountRoleResponse>
    {
        private Mock<IEmployerAccountService> _accountService;
        public override GetUserAccountRoleQuery Query { get; set; }
        public override GetUserAccountRoleQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserAccountRoleQuery>> RequestValidator { get; set; }

        private const string ExpectedAccountId = "123RFV";
        private const string ExpectedUserId = "56TGB";


        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetUserAccountRoleQuery
            {
                HashedAccountId = ExpectedAccountId,
                UserId = ExpectedUserId
            };

            _accountService = new Mock<IEmployerAccountService>();
            _accountService.Setup(x => x.GetUserRoleOnAccount(ExpectedAccountId, ExpectedUserId))
                .ReturnsAsync(new TeamMember());

            RequestHandler = new GetUserAccountRoleQueryHandler(RequestValidator.Object, _accountService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _accountService.Verify(x=>x.GetUserRoleOnAccount(ExpectedAccountId,ExpectedUserId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.User);
        }

        [Test]
        public async Task ThenIfTheServiceReturnsNullThenNullIsReturned()
        {
            //Arrange
            var query = new GetUserAccountRoleQuery {HashedAccountId = "123RFV",UserId = "45rfv"};

            //Act
            var actual = await RequestHandler.Handle(query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNull(actual.User);
        }
    }
}
