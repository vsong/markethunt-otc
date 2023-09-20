using Lamar;
using MarkethuntOTC.Agent.Configuration;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.TextProcessing;
using MarkethuntOTC.TextProcessing.Lexer;
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.Agent;

public class ApplicationRegistry : ServiceRegistry
{
    public ApplicationRegistry(DatabaseConnectionOptions databaseConnectionOptions)
    {
        Scan(x =>
        {
            x.WithDefaultConventions();
            
            x.AssemblyContainingType(typeof(IMessageProcessor));
        });

        ForSingletonOf<ILexerFactory>().Use<LexerFactory>();
        
        var optionsBuilder = new DbContextOptionsBuilder<DomainContext>();
        optionsBuilder.UseMySql(databaseConnectionOptions.MarkethuntDatabase, new MariaDbServerVersion(new Version(10, 5, 0)));

        ForSingletonOf<DbContextOptions<DomainContext>>().Use(optionsBuilder.Options);
        ForSingletonOf<IDomainContextFactory>().Use<DomainContextFactory>();
    }
}