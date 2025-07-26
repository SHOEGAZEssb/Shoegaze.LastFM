// See https://aka.ms/new-console-template for more information
using Shoegaze.LastFM.Authentication;

Console.WriteLine("Hello, World!");

var http = new HttpClient();
var auth = new LastfmAuthService(http, "69fbfa5fdc2cc1a158ec3bffab4be7a7", "30a6ed8a75dad2aa6758fa607c53adb5", "");
var session = await auth.AuthenticateAsync();

Console.WriteLine($"Hello, {session.Username}. Session key: {session.SessionKey}");
