using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HyperReps.Infrastructure.Persistence.Configurations
{
    public class MixSegmentConfiguration : IEntityTypeConfiguration<MixSegment>
    {
        public void Configure(EntityTypeBuilder<MixSegment> builder)
        {
            // Configure MixSegment relationships
            builder.HasOne(ms => ms.Track)
                .WithMany(t => t.ReferencedBySegments)
                .HasForeignKey(ms => ms.TrackId);
        }
    }
}
