using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Domain.Entities;
using HyperReps.Domain.ValueObjects;

namespace HyperReps.Application.Users.Commands
{
    public record UpsertUserCommand(
        string SpotifyId,
        string Email,
        string DisplayName,
        string AvatarUrl,
        string AccessToken,
        string RefreshToken,
        DateTimeOffset Expiry
    );

    public class UpsertUserHandler
    {
        public async Task<Guid> Handle(UpsertUserCommand command, IUserRepository repository)
        {
            var user = await repository.GetBySpotifyIdAsync(command.SpotifyId);
            var credentials = new SpotifyCredentials(
                command.AccessToken,
                command.RefreshToken,
                command.Expiry
            );

            if (user == null)
            {
                user = new User(
                    Guid.NewGuid(),
                    command.SpotifyId,
                    command.Email,
                    command.DisplayName,
                    command.AvatarUrl,
                    credentials
                );
                await repository.AddAsync(user);
            }
            else
            {
                user.UpdateProfile(command.DisplayName, command.Email, command.AvatarUrl);
                user.UpdateCredentials(credentials);
                await repository.UpdateAsync(user);
            }

            return user.Id;
        }
    }
}
