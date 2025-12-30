using HyperReps.Domain.Common;
using HyperReps.Domain.Exceptions;
using HyperReps.Domain.ValueObjects;


namespace HyperReps.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string SpotifyId { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string DisplayName { get; private set; } = null!;
        public string AvatarUrl { get; private set; } = null!;

        public SpotifyCredentials Credentials { get; private set; } = null!;

        private readonly List<Mix> _mixes = new();
        public IReadOnlyCollection<Mix> Mixes => _mixes.AsReadOnly();

        private readonly List<Playlist> _playlists = new();
        public IReadOnlyCollection<Playlist> Playlists => _playlists.AsReadOnly();

        private User() : base() { }

        public User(Guid id, string spotifyId, string email, string displayName, string avatarUrl,
                    SpotifyCredentials spotifyCredentials) : base(id)
        {
            if (string.IsNullOrWhiteSpace(spotifyId)) throw UserValidationException.InvalidSpotifyId();

            SpotifyId = spotifyId;
            Email = email;
            DisplayName = displayName;
            AvatarUrl = avatarUrl;
            Credentials = spotifyCredentials ?? throw UserValidationException.CredentialsRequired();
        }


        public void UpdateProfile(string displayName, string avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(displayName)) throw UserValidationException.InvalidDisplayName();

            DisplayName = displayName;
            AvatarUrl = avatarUrl;
        }

        public void UpdateCredentials(SpotifyCredentials spotifyCredentials)
        {
            Credentials = spotifyCredentials ?? throw UserValidationException.CredentialsMustBeDefined();
        }

        public void AddMix(Mix mix)
        {
            if (mix == null) throw new ArgumentNullException(nameof(mix));
            if (_mixes.Contains(mix)) throw new InvalidOperationException("Mix already exists for this user.");
            _mixes.Add(mix);
        }

        public void AddPlaylist(Playlist playlist)
        {
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            if (_playlists.Contains(playlist)) throw new InvalidOperationException("Playlist already exists for this user.");
            _playlists.Add(playlist);
        }
    }
}
