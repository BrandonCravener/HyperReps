using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HyperReps.Infrastructure.Persistence.Configurations
{
    public class PlaylistTrackConfiguration : IEntityTypeConfiguration<PlaylistTrack>
    {
        public void Configure(EntityTypeBuilder<PlaylistTrack> builder)
        {
            // Configure many-to-many relationship between Playlist and Track via PlaylistTrack
            builder.HasKey(pt => new { pt.PlaylistId, pt.TrackId });

            // Configure PlaylistTrack relationships
            builder.HasOne(pt => pt.Playlist)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(pt => pt.PlaylistId);

            // Configure Track relationship
            builder.HasOne(pt => pt.Track)
                .WithMany(t => t.IncludedInPlaylists)
                .HasForeignKey(pt => pt.TrackId);
        }
    }
}
