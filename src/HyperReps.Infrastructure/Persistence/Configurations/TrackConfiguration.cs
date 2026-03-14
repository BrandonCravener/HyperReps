using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HyperReps.Infrastructure.Persistence.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            // Configure unique index on Track's SpotifyTrackId
            builder.HasIndex(t => t.SpotifyTrackId)
                .IsUnique();
        }
    }
}
