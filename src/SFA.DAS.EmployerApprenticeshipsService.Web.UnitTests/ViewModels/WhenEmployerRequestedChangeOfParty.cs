using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers;
using System;
using System.Collections.Generic;


namespace SFA.DAS.EmployerCommitments.Web.UnitTests.ViewModels
{
    public class WhenEmployerRequestedChangeOfParty : ApprenticeshipMapperBase
    {
        [TestCase(ChangeOfPartyRequestStatus.Approved, true)]
        [TestCase(ChangeOfPartyRequestStatus.Pending, false)]
        [TestCase(ChangeOfPartyRequestStatus.Rejected, true)]
        [TestCase(ChangeOfPartyRequestStatus.Withdrawn, true)]
        public void ThenShowChangePrviderLinkCorrectly(ChangeOfPartyRequestStatus changeOfPartyRequestStatus, bool flag)
        {
            //Arrange
            var apprenticeship = new Apprenticeship
            {
                PaymentStatus = PaymentStatus.Withdrawn,
                StartDate = new DateTime(2018, 05, 05),
                StopDate = new DateTime(2018, 11, 05),
                ChangeOfPartyRequests = new List<ChangeOfPartyRequest>
                {
                    new ChangeOfPartyRequest
                    {
                        Status = changeOfPartyRequestStatus,
                        ChangeOfPartyType = ChangeOfPartyRequestType.ChangeProvider
                    }
                }
            };

            //Act
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            //Assert
            viewModel.ShowChangeTrainingProviderLink.Should().Be(flag);
        }
    }
}
