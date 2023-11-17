using MarkethuntOTC.Domain.Roots.Listing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkethuntOTC.Infrastructure.Mappings;

public class ListingMapping : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("listing");
        builder.Property(x => x.ItemId).HasColumnName("item_id");
        builder.Property(x => x.IsSelling).HasColumnName("is_selling");
        builder.Property(x => x.SbPrice).HasColumnName("sb_price");
        builder.Property(x => x.ListingType).HasColumnName("listing_type");
        builder.Property(x => x.MessageId).HasColumnName("message_id");
        builder.Property(x => x.ParseRuleId).HasColumnName("parse_rule_id");
        builder.Property(x => x.Timestamp).HasColumnName("timestamp");
    }
}