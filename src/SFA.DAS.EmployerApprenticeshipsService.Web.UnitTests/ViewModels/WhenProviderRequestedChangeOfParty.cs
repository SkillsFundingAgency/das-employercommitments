using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers;
using System;
using System.Collections.Generic;


namespace SFA.DAS.EmployerCommitments.Web.UnitTests.ViewModels
{
    public class WhenProviderRequestedChangeOfParty : ApprenticeshipMapperBase
    {      

        [TestCase(ChangeOfPartyRequestStatus.Pending, false)]
        public void Then_CoE_request_not_yet_approved_dont_show_ChangeProviderLink(ChangeOfPartyRequestStatus changeOfPartyRequestStatus, bool flag)
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
                        ChangeOfPartyType = ChangeOfPartyRequestType.ChangeEmployer
                    }
                }
            };

            //Act
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            //Assert
            viewModel.ShowChangeTrainingProviderLink.Should().Be(flag);
        }

        
        [TestCase(ChangeOfPartyRequestStatus.Approved, false)]
        public void Then_CoE_request_approved_by_parties_apprenticeship_moved_from_me_dont_show_ChangeProviderLink(ChangeOfPartyRequestStatus changeOfPartyRequestStatus, bool flag)
        {
            //Arrange
            var apprenticeship = new Apprenticeship
            {
                PaymentStatus = PaymentStatus.Withdrawn,
                StartDate = new DateTime(2018, 05, 05),
                StopDate = new DateTime(2018, 11, 05),
                ContinuationOfId = null, /* apprenticeship has moved from me to another employer*/
                ChangeOfPartyRequests = new List<ChangeOfPartyRequest>
                {
                    new ChangeOfPartyRequest
                    {
                        Status = changeOfPartyRequestStatus,
                        ChangeOfPartyType = ChangeOfPartyRequestType.ChangeEmployer
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
