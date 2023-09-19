using Lamar;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.TextProcessing;
using MarkethuntOTC.TextProcessing.Lexer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MarkethuntOTC.Agent;

public class ApplicationRegistry : ServiceRegistry
{
    public ApplicationRegistry()
    {
        Scan(x =>
        {
            x.WithDefaultConventions();
            
            x.AssemblyContainingType(typeof(IMessageProcessor));
        });

        ForSingletonOf<ILexerFactory>().Use<LexerFactory>();
        
        var connectionString = "server=localhost;port=8990;user=appdbuser;password=7e49cd3db4;database=markethunt";
        
        var optionsBuilder = new DbContextOptionsBuilder<DomainContext>();
        optionsBuilder.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 5, 0)));

        ForSingletonOf<DbContextOptions<DomainContext>>().Use(optionsBuilder.Options);
        ForSingletonOf<IDomainContextFactory>().Use<DomainContextFactory>();
    }
}