using MediatR;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Commands.PublishGenericEvent
{
    public class PublishGenericEventCommand : IAsyncRequest<PublishGenericEventCommandResponse>
    {
        public GenericEvent Event { get; set; }
    }
}
