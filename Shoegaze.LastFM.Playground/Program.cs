// See https://aka.ms/new-console-template for more information
using Shoegaze.LastFM;
using Shoegaze.LastFM.Authentication;

Console.WriteLine("Hello, World!");

var http = new HttpClient();
var auth = new LastfmAuthService(http, "69fbfa5fdc2cc1a158ec3bffab4be7a7", "30a6ed8a75dad2aa6758fa607c53adb5", "");
var session = await auth.AuthenticateAsync();

Console.WriteLine($"Hello, {session.Username}. Session key: {session.SessionKey}");

var client = new LastfmClient("69fbfa5fdc2cc1a158ec3bffab4be7a7", "30a6ed8a75dad2aa6758fa607c53adb5", http);
client.SetSessionKey(session.SessionKey);
var ui = await client.User.GetInfoAsync();
var friends = await client.User.GetFriendsAsync();
var loves = await client.User.GetLovedTracksAsync();
var tt = await client.User.GetTopTracksAsync();
var rt = await client.User.GetRecentTracksAsync();
int i = 0;