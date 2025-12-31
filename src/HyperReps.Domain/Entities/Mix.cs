using HyperReps.Domain.Common;
using HyperReps.Domain.Exceptions;

namespace HyperReps.Domain.Entities
{
    public class Mix : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? ThumbnailUrl { get; private set; }
        public string? Description { get; private set; }
        public bool IsPublic { get; private set; }
        public int TotalDurationMs { get; private set; }

        public User? User { get; private set; }

        private readonly List<MixSegment> _segments = new();
        public IReadOnlyCollection<MixSegment> Segments => _segments.OrderBy(s => s.SequenceOrder).ToList().AsReadOnly();

        private Mix() : base() { }

        public Mix(Guid id, Guid userId, string name, string? description, string? thumbnailUrl, bool isPublic)
            : base(id)
        {
            if (userId == Guid.Empty) throw MixValidationException.InvalidUserId();
            if (string.IsNullOrWhiteSpace(name)) throw MixValidationException.InvalidName();

            UserId = userId;
            Name = name;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            IsPublic = isPublic;
            TotalDurationMs = 0;
        }

        public void UpdateDetails(string name, string? description, string? thumbnailUrl, bool isPublic)
        {
            if (string.IsNullOrWhiteSpace(name)) throw MixValidationException.InvalidName();

            Name = name;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            IsPublic = isPublic;
        }

        public void AddSegment(MixSegment segment)
        {
            if (segment == null) throw new MixValidationException("Segment cannot be null.");

            _segments.Add(segment);
            CalculateTotalDuration();
        }

        public void RemoveSegment(Guid segmentId)
        {
            var segment = _segments.FirstOrDefault(s => s.Id == segmentId);
            if (segment != null)
            {
                _segments.Remove(segment);
                ReorderSegments();
                CalculateTotalDuration();
            }
        }

        private void ReorderSegments()
        {
            var sorted = _segments.OrderBy(s => s.SequenceOrder).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].UpdateOrder(i);
            }
        }

        private void CalculateTotalDuration()
        {
            int total = 0;
            var sorted = _segments.OrderBy(s => s.SequenceOrder).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                total += (sorted[i].EndTimeMs - sorted[i].StartTimeMs);
            }

            TotalDurationMs = Math.Max(0, total);
        }
    }
}
