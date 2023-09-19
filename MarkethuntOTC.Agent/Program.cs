// See https://aka.ms/new-console-template for more information

// DI

using System.Text.Json;
using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.TextProcessing;
using MarkethuntOTC.TextProcessing.Lexer;
using Microsoft.EntityFrameworkCore;

var connectionString = "server=localhost;port=8990;user=appdbuser;password=7e49cd3db4;database=markethunt";
var optionsBuilder = new DbContextOptionsBuilder<DomainContext>();
optionsBuilder.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 5, 0)));

var lexerFactory = new LexerFactory();
var parser = new Parser(new DomainContextFactory(optionsBuilder.Options));
var textProcessor = new MessageProcessor(lexerFactory, parser);

var listings = textProcessor.ExtractListings(new Message(0, ChannelType.SellMapsChests, "S> Fresh ESP 450\nS> Unopened BB 20", new DateTime()));

var jsonOpts = new JsonSerializerOptions {WriteIndented = true};

Console.WriteLine(JsonSerializer.Serialize(listings, jsonOpts));