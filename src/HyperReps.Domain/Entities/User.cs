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
            if (mix == null) throw UserValidationException.NullEntity(nameof(Mix));
            if (_mixes.Any(m => m.Id == mix.Id)) throw UserValidationException.MixAlreadyExists(mix.Id);

            _mixes.Add(mix);
        }

        public void AddPlaylist(Playlist playlist)
        {
            if (playlist == null) throw UserValidationException.NullEntity(nameof(Playlist));
            if (_playlists.Any(p => p.Id == playlist.Id)) throw UserValidationException.PlaylistAlreadyExists(playlist.Id);

            _playlists.Add(playlist);
        }
    }
}
