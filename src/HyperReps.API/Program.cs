using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Application.Common.Interfaces.Services;
using HyperReps.Application.Common.Middleware;
using HyperReps.Infrastructure.Persistence;
using HyperReps.Infrastructure.Persistence.Repositories;
using HyperReps.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Client.AspNetCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAntiforgery();

builder.Services.AddRouting(opts => opts.LowercaseUrls = true);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add PostgreSQL EF Core support
var connectionString = builder.Configuration.GetConnectionString("HyperRepsDatabase");
builder.Services.AddDbContextPool<HyperRepsContext>(opt =>
{
    opt.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            // Use split query for related collections to avoid Cartesian explosion
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    );
    opt.UseOpenIddict();
});

builder
    .Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIddictClientAspNetCoreDefaults.AuthenticationScheme;
    })
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        opts =>
        {
            opts.LoginPath = "/api/auth/login";
            opts.LogoutPath = "/auth/auth/logout";
        }
    );

// Configure OpenIddict for Spotify authentication
var spotifyConfiguration = builder.Configuration.GetRequiredSection("Spotify");
builder
    .Services.AddOpenIddict()
    .AddCore(opts =>
    {
        opts.UseEntityFrameworkCore().UseDbContext<HyperRepsContext>();
    })
    .AddClient(opts =>
    {
        opts.AllowAuthorizationCodeFlow();

        if (builder.Environment.IsDevelopment())
        {
            opts.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        }

        opts.UseAspNetCore().EnableRedirectionEndpointPassthrough();

        opts.UseSystemNetHttp();

        opts.UseWebProviders()
            .AddSpotify(opts =>
            {
                opts.SetClientId(spotifyConfiguration["ClientId"]!)
                    .SetClientSecret(spotifyConfiguration["ClientSecret"]!)
                    .SetRedirectUri("api/auth/callback")
                    .AddScopes(["user-read-email", "playlist-read-private", "streaming"]);
            });
    });

// Register repositories for dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMixRepository, MixRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();

builder.Services.AddScoped<ISpotifyAuthService, SpotifyAuthService>();

builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(IUserRepository).Assembly);
    opts.PersistMessagesWithPostgresql(connectionString!, "wolverine");
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.AutoApplyTransactions();
    opts.Policies.AddMiddleware(typeof(LoggingMiddleware));
    opts.UseFluentValidation();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<HyperRepsContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            throw;
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
