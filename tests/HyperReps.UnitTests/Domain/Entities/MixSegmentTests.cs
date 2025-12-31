using HyperReps.Domain.Entities;
using HyperReps.Domain.Exceptions;

namespace HyperReps.UnitTests.Domain.Entities
{
    public class MixSegmentTests
    {
        [Fact]
        public void Constructor_ShouldCreateSegment_WhenValid()
        {

            var id = Guid.NewGuid();
            var mixId = Guid.NewGuid();
            var trackId = Guid.NewGuid();


            var segment = new MixSegment(id, mixId, trackId, 1000, 2000, 0);


            Assert.Equal(1000, segment.StartTimeMs);
            Assert.Equal(2000, segment.EndTimeMs);
            Assert.Equal(0, segment.SequenceOrder);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenTimeRangeInvalid()
        {


            Assert.Throws<MixSegmentValidationException>(() =>
                new MixSegment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2000, 1000, 0));
        }

        [Fact]
        public void UpdateCrops_ShouldUpdateTimes_WhenValid()
        {

            var segment = new MixSegment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1000, 2000, 0);


            segment.UpdateCrops(500, 2500);


            Assert.Equal(500, segment.StartTimeMs);
            Assert.Equal(2500, segment.EndTimeMs);
        }
    }
}
