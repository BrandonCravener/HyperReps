using HyperReps.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.ValueObjects
{
    public record SpotifyCredentials
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public DateTime Expiry { get; }

        public SpotifyCredentials(string accessToken, string refreshToken, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) throw UserValidationException.InvalidAccessToken();
            if (string.IsNullOrWhiteSpace(refreshToken)) throw UserValidationException.InvalidRefreshToken();

            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expiry = expiry;
        }

        public bool IsExpired() => DateTime.UtcNow >= Expiry.AddMinutes(-5);
    }
}
