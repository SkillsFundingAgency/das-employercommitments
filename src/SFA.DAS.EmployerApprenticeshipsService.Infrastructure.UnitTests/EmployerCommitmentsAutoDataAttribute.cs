using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests
{
    public class EmployerCommitmentsAutoDataAttribute : AutoDataAttribute
    {
        public EmployerCommitmentsAutoDataAttribute() : base(FixtureBuilder.FixtureFactory)
        { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EmployerCommitmentsInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public EmployerCommitmentsInlineAutoDataAttribute(params object[] arguments)
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