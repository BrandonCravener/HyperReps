using HyperReps.Domain.Common;
using HyperReps.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Entities
{
    public class MixSegment : BaseEntity
    {
        public Guid MixId { get; private set; }
        public Guid TrackId { get; private set; }
        public int StartTimeMs { get; private set; }
        public int EndTimeMs { get; private set; }
        public int SequenceOrder { get; private set; }

        public Mix? Mix { get; private set; }
        public Track? Track { get; private set; }

        private MixSegment() : base() { }

        public MixSegment(Guid id, Guid mixId, Guid trackId, int startTimeMs, int endTimeMs, int sequenceOrder)
            : base(id)
        {
            Validate(mixId, trackId, startTimeMs, endTimeMs, sequenceOrder);

            MixId = mixId;
            TrackId = trackId;
            StartTimeMs = startTimeMs;
            EndTimeMs = endTimeMs;
            SequenceOrder = sequenceOrder;
        }

        public void UpdateCrops(int startTimeMs, int endTimeMs)
        {
            Validate(MixId, TrackId, startTimeMs, endTimeMs, SequenceOrder);

            StartTimeMs = startTimeMs;
            EndTimeMs = endTimeMs;
        }

        public void UpdateOrder(int newOrder)
        {
            if (newOrder < 0) throw MixSegmentValidationException.InvalidSequence();
            SequenceOrder = newOrder;
        }

        private void Validate(Guid mixId, Guid trackId, int start, int end, int order)
        {
            if (mixId == Guid.Empty) throw new MixSegmentValidationException("MixId is required.");
            if (trackId == Guid.Empty) throw new MixSegmentValidationException("TrackId is required.");
            if (end <= start) throw MixSegmentValidationException.InvalidTimeRange();
            if (order < 0) throw MixSegmentValidationException.InvalidSequence();
        }
    }
}
