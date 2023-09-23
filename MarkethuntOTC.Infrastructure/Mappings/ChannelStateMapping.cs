using MarkethuntOTC.DataTransferObjects.Agent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkethuntOTC.Infrastructure.Mappings;

public class ChannelStateMapping : IEntityTypeConfiguration<ChannelState>
{
    public void Configure(EntityTypeBuilder<ChannelState> builder)
    {
        builder.ToTable("channel_state");
        builder.HasKey(x => x.ChannelId);
        builder.Property(x => x.ChannelId).HasColumnName("channel_id");
        builder.Property(x => x.ServerId).HasColumnName("server_id");
        builder.Property(x => x.ChannelType).HasColumnName("channel_type");
        builder.Property(x => x.LatestMessageId).HasColumnName("latest_message_id");
    }
}