namespace MarkethuntOTC.Infrastructure;

public interface IDomainContextFactory
{
    DomainContext Create();
}