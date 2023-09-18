using MarkethuntOTC.Domain.Roots.ParseRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkethuntOTC.Infrastructure.Mappings;

public class ParseRuleMapping : IEntityTypeConfiguration<ParseRule>
{
    public void Configure(EntityTypeBuilder<ParseRule> builder)
    {
        builder.ToTable("parse_rule");
        builder.Property(x => x.ItemId).HasColumnName("item_id");
        builder.Property(x => x.ParseItemCategory).HasColumnName("parse_item_category");
        builder.Property(x => x.StartDate).HasColumnName("start_date");
        builder.Property(x => x.EndDate).HasColumnName("end_date");
    }
}