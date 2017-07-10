using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Factories
{
    public interface IGenericEventFactory
    {
        GenericEvent Create<T>(T value);
    }
}
