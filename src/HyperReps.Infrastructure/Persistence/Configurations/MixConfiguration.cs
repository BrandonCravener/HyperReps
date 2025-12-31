using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HyperReps.Infrastructure.Persistence.Configurations
{
    public class MixConfiguration : IEntityTypeConfiguration<Mix>
    {
        public void Configure(EntityTypeBuilder<Mix> builder)
        {
            // Configure one-to-many relationship between Mix and MixSegment
            builder.HasMany(m => m.Segments)
                .WithOne(ms => ms.Mix)
                .HasForeignKey(ms => ms.MixId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure concurrency token
            builder.Property<uint>("Version")
                .IsRowVersion();
        }
    }
}
