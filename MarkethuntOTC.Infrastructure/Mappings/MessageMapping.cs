using MarkethuntOTC.Domain.Roots.DiscordMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkethuntOTC.Infrastructure.Mappings;

public class MessageMapping : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("message");
        builder.Property(x => x.OriginatingChannelType).HasColumnName("originating_channel_type");
        builder.Property(x => x.OriginationChannelId).HasColumnName("originating_channel_id");
        builder.Property(x => x.CreatedOn).HasColumnName("created_on");
    }
}