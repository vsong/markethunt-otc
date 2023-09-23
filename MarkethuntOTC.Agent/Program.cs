using Lamar;
using MarkethuntOTC.Agent;
using MarkethuntOTC.ApplicationServices;
using MarkethuntOTC.DataTransferObjects.Configuration;
using Microsoft.Extensions.Configuration;

#region Config options setup
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .Build();
    
var databaseConnectionOptions = configuration
    .GetRequiredSection(nameof(DatabaseConnectionOptions))
    .Get<DatabaseConnectionOptions>(x => x.BindNonPublicProperties = true)!;

var discordBotOptions = configuration
    .GetRequiredSection(nameof(DiscordBotOptions))
    .Get<DiscordBotOptions>(x => x.BindNonPublicProperties = true)!;
#endregion

#region Container setup
var registry = new ApplicationRegistry(databaseConnectionOptions, discordBotOptions);
var container = new Container(registry);
#endregion

container.GetInstance<IMessageCollectionService>();

Console.ReadKey();