using HyperReps.Domain.Common;
using HyperReps.Domain.Entities;
using HyperReps.Infrastructure.Persistence.Configurations;
using HyperReps.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace HyperReps.Infrastructure.Persistence
{
    public class HyperRepsContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public HyperRepsContext(DbContextOptions<HyperRepsContext> dbContextOptions, IConfiguration configuration) : base(dbContextOptions) 
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Mix> Mixes { get; set; }
        public DbSet<MixSegment> MixSegments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var encryptionKey = _configuration["Spotify:EncryptionKey"];
            if (!string.IsNullOrEmpty(encryptionKey))
            {
                var encryptionConverter = new EncryptionValueConverter(encryptionKey);
                modelBuilder.Entity<User>()
                    .OwnsOne(u => u.Credentials, a =>
                    {
                        a.Property(p => p.AccessToken).HasConversion(encryptionConverter);
                        a.Property(p => p.RefreshToken).HasConversion(encryptionConverter);
                    });
            }

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();   
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker
                .Entries<AuditableEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var now = DateTimeOffset.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }

                entry.Entity.ModifiedAt = now;
            }
        }

    }
}
