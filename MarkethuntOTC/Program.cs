using MarkethuntOTC;
using MarkethuntOTC.Domain;
using MarkethuntOTC.Services.QueryServices;
using MarkethuntOTC.TextProcessing;
using System.Linq;
using System.Text.Json;

Console.WriteLine("Hello, World!");

var serializerOpts = new JsonSerializerOptions { WriteIndented = true };