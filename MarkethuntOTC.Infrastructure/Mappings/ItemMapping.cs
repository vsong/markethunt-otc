using MarkethuntOTC.Domain.Roots.Item;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkethuntOTC.Infrastructure.Mappings;

public class ItemMapping : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("item_meta");
        builder.Property(x => x.Id).HasColumnName("item_id");
        builder.OwnsMany(x => x.DailyValues, ownedBuilder =>
        {
            ownedBuilder.ToTable("v_sb_prices");
            ownedBuilder.WithOwner().HasForeignKey("item_id");
            ownedBuilder.Property(x => x.Timestamp).HasColumnName("date");
            ownedBuilder.Property(x => x.SbPrice).HasColumnName("sb_price");
        });
    }
}