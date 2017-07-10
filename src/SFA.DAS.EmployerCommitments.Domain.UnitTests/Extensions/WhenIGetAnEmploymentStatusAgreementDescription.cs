using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EmployerCommitments.Domain.UnitTests.Extensions
{
    class WhenIGetAnEmploymentStatusAgreementDescription
    {
        [Test]
        public void ThenIShouldGetTheCorrectDescription()
        {
            Assert.AreEqual("Not signed", EmployerAgreementStatus.Pending.GetDescription());
            Assert.AreEqual("Signed", EmployerAgreementStatus.Signed.GetDescription());
            Assert.AreEqual("Expired", EmployerAgreementStatus.Expired.GetDescription());
            Assert.AreEqual("Superseded", EmployerAgreementStatus.Superseded.GetDescription());
        }
    }
}
