using System.Text.Json;
using Lamar;
using MarkethuntOTC.Agent;
using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.TextProcessing;

var registry = new ApplicationRegistry();
var container = new Container(registry);

var textProcessor = container.GetInstance<IMessageProcessor>();

var listings = textProcessor.ExtractListings(new Message(0, ChannelType.SellMapsChests, "S> Fresh ESP 450\nS> Unopened BB 20", new DateTime()));

var jsonOpts = new JsonSerializerOptions {WriteIndented = true};

Console.WriteLine(JsonSerializer.Serialize(listings, jsonOpts));