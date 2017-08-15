using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Plumbing
{
    [TestFixture]
    public class CommitmentRoutePrefixTests
    {
        [Test]
        public void ShouldPrefixRouteWithConfigurationValue()
        {
            var attribute = new CommitmentsRoutePrefixAttribute("apprenitceships/name/");

            attribute.Prefix.Should().Be("commitmentstest/apprenitceships/name/");
        }
    }
}
