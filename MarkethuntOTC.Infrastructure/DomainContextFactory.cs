
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.Infrastructure;

public class DomainContextFactory : IDomainContextFactory
{
    private readonly DbContextOptions<DomainContext> _options;
    
    public DomainContextFactory(DbContextOptions<DomainContext> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public DomainContext Create()
    {
        var context = new DomainContext(_options);

        // Test db connection. Will throw exception if connection fails.
        context.Database.OpenConnection();
        
        return context;
    }
}