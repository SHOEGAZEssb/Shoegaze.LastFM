# Shoegaze.LastFM
A modern C# wrapper for the LastFM API.

[![NuGet](https://img.shields.io/nuget/v/Shoegaze.LastFM.svg)](https://www.nuget.org/packages/Shoegaze.LastFM/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Shoegaze.LastFM.svg)](https://www.nuget.org/packages/Shoegaze.LastFM/)

[![CI](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/SHOEGAZEssb/Shoegaze.LastFM/branch/main/graph/badge.svg)](https://codecov.io/gh/SHOEGAZEssb/Shoegaze.LastFM)
![CodeQL](https://img.shields.io/github/actions/workflow/status/SHOEGAZEssb/Shoegaze.LastFM/codeql.yml?label=CodeQL)
![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=SHOEGAZEssb_Shoegaze.LastFM&metric=alert_status)

## API Implementation Status

| Category       | Progress |
|----------------|----------|
| Album          | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Album/IAlbumApi.cs) |
| Artist         | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Artist/IArtistApi.cs) |
| Chart          | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Chart/IChartApi.cs) |
| Geo            | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Geo/IGeoApi.cs) |
| Library        | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Library/ILibraryApi.cs) |
| Tag            | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Tag/ITagApi.cs) |
| Track          | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/Track/ITrackApi.cs) |
| User           | [![100%](https://img.shields.io/badge/Progress-100%25-brightgreen)](https://github.com/SHOEGAZEssb/Shoegaze.LastFM/blob/main/Shoegaze.LastFM/User/IUserApi.cs) |

## Quickstart

### Creating a Client

To do any API requests at all you need to instanciate a `LastfmClient`. You will need to supply your **API key** and your **API secret** that you can obtain by [creating an API account](https://www.last.fm/api/account/create).

```csharp
var client = new LastfmClient(apiKey, apiSecret, httpClient);

// you can then call any method that does not require user authentication
var response = await client.user.GetRecentTracksAsync("testUser");
var tracks = response.Data.Items;
```

All API responses contain error information in case something went wrong.

### Getting a Session Key

To be able to do authenticated requests (like scrobbling or adding tracks) you will need a session key granted by the user.

There are two ways to obtain a session key:

#### 1. Automatic Desktop Flow

Use `AuthenticateAsync()`. This opens the user’s default browser, listens on a temporary `http://localhost:{port}/` URL, and automatically exchanges the returned token for a session key.

```csharp
var auth = new LastfmAuthService(apiKey, apiSecret, httpClient);
var session = await auth.AuthenticateAsync();
Console.WriteLine($"Logged in as {session.User} with key {session.Key}");
```

#### 2. Manual Flow

Use `GetAuthorizationUrlAsync(callbackUrl)` with your own callback URL (e.g. a webserver endpoint or custom URI scheme). Redirect the user there, capture the token from Last.fm’s redirect, and then call `GetSessionAsync(token)` to complete authentication and get a session key.

```csharp
var auth = new LastfmAuthService(http, apiKey, apiSecret);
var authUrl = await auth.GetAuthorizationUrlAsync("https://myapp.com/callback");

// redirect the user to authUrl and receive the ?token=... in your callback handler
var token = "...";
var session = await auth.GetSessionAsync(token);
Console.WriteLine($"Logged in as {session.User} with key {session.Key}");
```

### Authenticated Requests

After creating a `LastfmClient` and getting your session key, you can 'authenticate' the client.

```csharp
var auth = new LastfmAuthService(apiKey, apiSecret, httpClient);
var session = await auth.AuthenticateAsync();

var client = new LastfmClient(apiKey, apiSecret, httpClient);
client.SetSessionKey(session.Key);

// you can then call any method on the client that requires authentication
var response = await client.Artist.AddTagsAsync("My Bloody Valentine", "shoegaze");
```

### Scrobbling

You can scrobble batches of up to 50 scrobbles at once.
Scrobbling requires an **authenticated client**.

```csharp
var scrobble = new ScrobbleData(artistName: "My Bloody Valentine", trackName: "Loomer", playedAt: DateTime.UtcNow);

var response = await client.Track.ScrobbleAsync(scrobble);
```
