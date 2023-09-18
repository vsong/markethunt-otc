namespace MarkethuntOTC.Domain;

public abstract class Entity<TIdentifier>
{
    protected Entity()
    {
    }

    public TIdentifier Id { get; protected set; }
}