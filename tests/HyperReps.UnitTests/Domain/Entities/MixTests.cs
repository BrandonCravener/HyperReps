using HyperReps.Domain.Entities;
using HyperReps.Domain.Exceptions;

namespace HyperReps.UnitTests.Domain.Entities
{
    public class MixTests
    {
        [Fact]
        public void Constructor_ShouldCreateMix_WhenArgumentsAreValid()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var name = "My Mix";

            var mix = new Mix(id, userId, name, "Desc", "Thumb", true);

            Assert.Equal(id, mix.Id);
            Assert.Equal(userId, mix.UserId);
            Assert.Equal(name, mix.Name);
            Assert.Equal(0, mix.TotalDurationMs);
            Assert.Empty(mix.Segments);
        }

        [Fact]
        public void AddSegment_ShouldAddAndRecalculateDuration()
        {
            var mix = new Mix(Guid.NewGuid(), Guid.NewGuid(), "My Mix", "Desc", "Thumb", true);
            var segment = new MixSegment(Guid.NewGuid(), mix.Id, Guid.NewGuid(), 0, 1000, 0);

            mix.AddSegment(segment);

            Assert.Single(mix.Segments);
            Assert.Equal(1000, mix.TotalDurationMs);
        }

        [Fact]
        public void RemoveSegment_ShouldRemoveAndRecalculateDuration_AndReorder()
        {
            var mix = new Mix(Guid.NewGuid(), Guid.NewGuid(), "My Mix", "Desc", "Thumb", true);
            var seg1 = new MixSegment(Guid.NewGuid(), mix.Id, Guid.NewGuid(), 0, 1000, 0);
            var seg2 = new MixSegment(Guid.NewGuid(), mix.Id, Guid.NewGuid(), 0, 2000, 1);
            var seg3 = new MixSegment(Guid.NewGuid(), mix.Id, Guid.NewGuid(), 0, 3000, 2);

            mix.AddSegment(seg1);
            mix.AddSegment(seg2);
            mix.AddSegment(seg3);

            Assert.Equal(3, mix.Segments.Count);
            Assert.Equal(6000, mix.TotalDurationMs);

            mix.RemoveSegment(seg2.Id);

            Assert.Equal(2, mix.Segments.Count);
            Assert.Equal(4000, mix.TotalDurationMs);

            var segments = mix.Segments.ToList();
            Assert.Equal(seg1.Id, segments[0].Id);
            Assert.Equal(0, segments[0].SequenceOrder);

            Assert.Equal(seg3.Id, segments[1].Id);
            Assert.Equal(1, segments[1].SequenceOrder);
        }
    }
}
