using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers;
using System;
using System.Collections.Generic;


namespace SFA.DAS.EmployerCommitments.Web.UnitTests.ViewModels
{
    public class WhenMappingShowChangeTrainingProviderLink : ApprenticeshipMapperBase
    {

        [TestCase(PaymentStatus.Active, true)]
        [TestCase(PaymentStatus.Withdrawn, true)]
        public void Then_Given_No_Change_Of_Parity_ShowChangeProviderLink(PaymentStatus paymentStatus, bool expected)
        {
            //Arrange
            var apprenticeship = new Apprenticeship
            {

                PaymentStatus = paymentStatus,
                StartDate = new DateTime(2021, 01, 01),
                ChangeOfPartyRequests = new List<ChangeOfPartyRequest>()
            };
            
            //Act
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            //Assert
            viewModel.ShowChangeTrainingProviderLink.Should().Be(expected);
        }


        [TestCase(PaymentStatus.Active, false)]
        [TestCase(PaymentStatus.Withdrawn, false)]
        public void Then_Given_ThereIsChangeOfParity_DoNotShowChangeProviderLink(PaymentStatus paymentStatus, bool expected)
        {
            //Arrange
            var apprenticeship = new Apprenticeship
            {

                PaymentStatus = paymentStatus,
                StartDate = new DateTime(2021, 01, 01),
                ChangeOfPartyRequests = new List<ChangeOfPartyRequest>
                {
                    new ChangeOfPartyRequest
                    {
                        Status =ChangeOfPartyRequestStatus.Pending ,
                        ChangeOfPartyType = ChangeOfPartyRequestType.ChangeEmployer
                    }
                }
            };

            //Act
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            //Assert
            viewModel.ShowChangeTrainingProviderLink.Should().Be(expected);
        }


        [TestCase(PaymentStatus.Paused, false)]
        [TestCase(PaymentStatus.Completed, false)]
        public void Then_BasedOnApprenticeStatus_DoNotShowChangeProviderLink(PaymentStatus paymentStatus, bool expected)
        {
            //Arrange
            var apprenticeship = new Apprenticeship
            {

                PaymentStatus = paymentStatus,
                StartDate = new DateTime(2021, 01, 01),
                ChangeOfPartyRequests = new List<ChangeOfPartyRequest>()
            };

            //Act
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);

            //Assert
            viewModel.ShowChangeTrainingProviderLink.Should().Be(expected);
        }


    }
}
