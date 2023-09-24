namespace MarkethuntOTC.DataTransferObjects.Events;

public abstract class Event
{
    protected Event(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}