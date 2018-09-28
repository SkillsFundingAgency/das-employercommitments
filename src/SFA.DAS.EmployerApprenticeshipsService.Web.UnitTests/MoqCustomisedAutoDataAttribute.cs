using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests
{
    public class MoqCustomisedAutoData : AutoDataAttribute
    {
        public MoqCustomisedAutoData() : base(FixtureBuilder.FixtureFactory)
        { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MoqCustomisedInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public MoqCustomisedInlineAutoDataAttribute(params object[] arguments)
            : base(FixtureBuilder.FixtureFactory, arguments)
        {
        }
    }

    internal static class FixtureBuilder
    {
        public static IFixture FixtureFactory()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            return fixture;
        }
    }
}