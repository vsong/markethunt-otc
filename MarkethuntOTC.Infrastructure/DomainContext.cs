using MarkethuntOTC.Domain.Roots.DiscordMessage;
using MarkethuntOTC.Domain.Roots.Item;
using MarkethuntOTC.Domain.Roots.Listing;
using MarkethuntOTC.Domain.Roots.ParseRule;
using MarkethuntOTC.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.Infrastructure;

public class DomainContext : DbContext
{
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ParseRule> ParseRules => Set<ParseRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemMapping).Assembly);
    }
}