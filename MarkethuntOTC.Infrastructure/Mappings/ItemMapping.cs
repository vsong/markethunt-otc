using MarkethuntOTC.Domain.Roots.Item;
using MarkethuntOTC.Domain.Roots.ParseRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkethuntOTC.Infrastructure.Mappings;

public class ItemMapping : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("item_meta");
        builder.Property(x => x.Id).HasColumnName("item_id");
    }
}