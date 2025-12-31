using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HyperReps.Infrastructure.Persistence.Converters;

namespace HyperReps.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure unique index on User's SpotifyId
            builder.HasIndex(u => u.SpotifyId)
                .IsUnique();

            // Configure one-to-many relationship between User and Playlist
            builder.HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between User and Mix
            builder.HasMany(u => u.Mixes)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SpotifyCredentials as an owned type
            builder.OwnsOne(u => u.Credentials, a =>
            {
                a.Property(p => p.AccessToken).HasColumnName("AccessToken");
                a.Property(p => p.RefreshToken).HasColumnName("RefreshToken");
                a.Property(p => p.Expiry).HasColumnName("TokenExpiry");
            });
        }
    }
}
