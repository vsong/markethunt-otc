using Lamar;
using MarkethuntOTC.ApplicationServices;
using MarkethuntOTC.ApplicationServices.Discord;
using MarkethuntOTC.DataTransferObjects.Configuration;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.TextProcessing;
using MarkethuntOTC.TextProcessing.Lexer;
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.Agent;

public class ApplicationRegistry : ServiceRegistry
{
    public ApplicationRegistry(DatabaseConnectionOptions databaseConnectionOptions, DiscordBotOptions discordBotOptions)
    {
        ForSingletonOf<DiscordBotOptions>().Use(discordBotOptions);
        
        Scan(x =>
        {
            x.WithDefaultConventions();
            
            x.AssemblyContainingType(typeof(IMessageProcessor));
        });

        ForSingletonOf<IDiscordService>().Use(_ => new DiscordService(discordBotOptions.Token));
        ForSingletonOf<IMessageCollectionService>().Use(x => new MessageCollectionService(
            x.GetInstance<IDiscordService>(),
            discordBotOptions.MessageCollectionInterval,
            x.GetInstance<IDomainContextFactory>()));

        ForSingletonOf<ILexerFactory>().Use<LexerFactory>();
        
        var optionsBuilder = new DbContextOptionsBuilder<DomainContext>();
        optionsBuilder.UseMySql(databaseConnectionOptions.MarkethuntDatabase, new MariaDbServerVersion(new Version(10, 5, 0)));

        ForSingletonOf<DbContextOptions<DomainContext>>().Use(optionsBuilder.Options);
        ForSingletonOf<IDomainContextFactory>().Use<DomainContextFactory>();
    }
}