using MarkethuntOTC.DataTransferObjects.Events;
using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MediatR;

namespace MarkethuntOTC.ApplicationServices;

public class AgentCommandService : IAgentCommandService
{
    private readonly IDiscordService _discordService;
    private readonly IMediator _mediator;

    public AgentCommandService(IDiscordService discordService, IMediator mediator)
    {
        _discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        _discordService.CommandReceived += OnCommandReceived;
    }

    public void OnCommandReceived(object sender, IEnumerable<string> args)
    {
        var argList = args.ToList();
        if (!argList.Any()) return;

        switch (argList[0])
        {
            case "reprocess":
                if (argList.Count != 2
                    || !int.TryParse(argList[1], out var channelTypeInt)
                    || !Enum.IsDefined(typeof(ChannelType), channelTypeInt))
                {
                    return;
                }

                _mediator.Publish(new ReprocessMessagesRequestedEvent(Guid.NewGuid(), (ChannelType)channelTypeInt));
                break;
        }
    }
}