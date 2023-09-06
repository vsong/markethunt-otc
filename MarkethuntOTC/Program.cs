using MarkethuntOTC;
using MarkethuntOTC.Domain;
using MarkethuntOTC.Services.QueryServices;
using MarkethuntOTC.TextProcessing;
using System.Linq;
using System.Text.Json;

Console.WriteLine("Hello, World!");

var serializerOpts = new JsonSerializerOptions { WriteIndented = true };

var testStrings = new string[]
{
    """
    Rare Empyrean Sky Palace RESP
    Lf 1 closer 920 2 leecher 940
    L2M LP
    """,
    """
    S> Fresh BB 560
    S> Fresh ESP 500
    """
};
    
var lexer = LexerFactory.Create(ChannelType.SellMapsChests);
var parser = new Parser(new ParseRuleQueryService());

// TODO contents of foreach loop should be inside parse class
foreach (var testString in testStrings)
{
    Console.WriteLine();
    Console.WriteLine(testString);
    Console.WriteLine();

    var tokens = lexer.Tokenize(testString);
    Console.WriteLine("Tokens:");
    Console.WriteLine(JsonSerializer.Serialize(tokens, serializerOpts));

    var results = tokens.Select(x => x.Accept(parser));
    Console.WriteLine("Parser Output:");
    Console.WriteLine(JsonSerializer.Serialize(results, serializerOpts));

    var finalTxns =
        from result in results
        where result.Successful
        select new Transaction(0, 1234567890, result.MatchedParseRule.ItemId, result.TransactionType, result.IsSelling, result.SbPrice, null);

    Console.WriteLine("Parsed Transactions:");
    Console.WriteLine(JsonSerializer.Serialize(finalTxns, serializerOpts));
}