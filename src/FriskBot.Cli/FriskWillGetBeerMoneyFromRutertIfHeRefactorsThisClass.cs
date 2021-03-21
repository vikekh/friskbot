﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FriskBot.Cli
{
    // This is a minimal, bare-bones example of using Discord.Net
    //
    // If writing a bot with commands, we recommend using the Discord.Net.Commands
    // framework, rather than handling commands yourself, like we do in this sample.
    //
    // You can find samples of using the command framework:
    // - Here, under the 02_commands_framework sample
    // - https://github.com/foxbot/DiscordBotBase - a bare-bones bot template
    // - https://github.com/foxbot/patek - a more feature-filled bot, utilizing more aspects of the library
    class FriskWillGetBeerMoneyFromRutertIfHeRefactorsThisClass
    {
        public class RemindMe
        {
            public DateTime Date { get; set; }
            public string Message { get; set; }
            public string User { get; set; }
            public ulong ChannelId { get; set; }
            public ulong GuildId { get; set; }
        }

        private readonly DiscordSocketClient _client;
        private const string _version = "v0.2.0";
        private DateTime _started = DateTime.Now;
        private Random _rnd = new Random();
        private System.Threading.Timer _intervalTimer;
        List<RemindMe> _remindMes = new List<RemindMe>();

        private readonly string AzureComputerVisionApiKey = Environment.GetEnvironmentVariable("AZURE_COMPUTER_VISION_API_KEY");
        private readonly string AzureLuisApiKey = Environment.GetEnvironmentVariable("AZURE_LUIS_API_KEY");

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        //static void Main(string[] args)
        //{
        //    new Program().MainAsync(args).GetAwaiter().GetResult();
        //}

        public FriskWillGetBeerMoneyFromRutertIfHeRefactorsThisClass(DiscordSocketClient discordSocketClient, IServiceProvider services)
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = discordSocketClient;

            //_client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.MessageUpdated += MessageUpdatedAsync;

            try {
                _remindMes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RemindMe>>(System.IO.File.ReadAllText("/data/test.txt"));
            } catch (Exception exc) {
                Console.WriteLine("kunde inte läsa filfan: " + exc.Message);
            }

            TimeSpan tsInterval = new TimeSpan(0, 1, 0);
            _intervalTimer = new System.Threading.Timer(
                new System.Threading.TimerCallback(IntervalTimer_Elapsed)
                , null, tsInterval, tsInterval);

            Console.WriteLine($"AZURE_COMPUTER_VISION_API_KEY={AzureComputerVisionApiKey}");
            Console.WriteLine($"AZURE_LUIS_API_KEY={AzureLuisApiKey}");
        }

        //public async Task MainAsync(string[] args)
        //{
        //    string token = "";

        //    try
        //    {
        //        token = args[0];
        //    }
        //    catch
        //    {
        //        throw new ApplicationException($"Auth token is not valid, \"{token}\".");
        //    }

        //    // Tokens should be considered secret data, and never hard-coded.
        //    await _client.LoginAsync(TokenType.Bot, token);
        //    await _client.StartAsync();
        //    await _client.SetGameAsync("Sekiro 2: Electric Boogaloo");

        //    // Block the program until it is closed.
        //    await Task.Delay(-1);
        //}

        //private Task LogAsync(LogMessage log)
        //{
        //    Console.WriteLine(log.ToString());
        //    return Task.CompletedTask;
        //}

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            //TODO: Only display message when bot is redeployed
            //await (_client.GetChannel(Convert.ToUInt64("503278200064049152")) as ISocketMessageChannel).SendMessageAsync($"Hello, my name is FriskBot {_version}!");

            //return Task.CompletedTask;
        }

        private async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (arg2.EditedTimestamp != null) {
                sortedEdits.Add(history[arg2.Id]);
		       // await arg2.Channel.SendMessageAsync("revisionism!! (han skrev egentligen " + history[arg2.Id] + ")");
                history[arg2.Id] = arg2.Content;
            }
        }

        private void IntervalTimer_Elapsed(object state)
        {
            try {
                List<RemindMe> removies = new List<RemindMe>();

                foreach (var remindMe in _remindMes.Where(p => DateTime.Now > p.Date)) {
                    var res = _client.GetGuild(remindMe.GuildId).GetTextChannel(remindMe.ChannelId).SendMessageAsync("Hey " + remindMe.User + " " + remindMe.Message + " is happening").Result;
                    
                    removies.Add(remindMe);
                }

                foreach (var removie in removies) {
                    _remindMes.Remove(removie);
                }

                if (removies.Any()) {
                    System.IO.File.WriteAllText("/data/test.txt", Newtonsoft.Json.JsonConvert.SerializeObject(_remindMes));
                }
            } catch(Exception exc) {
                Console.WriteLine("something went horribly wrong, contact the police: " + exc.Message);
            }
        }

        private bool isfriendochannel(SocketGuildChannel channel)
        {
            if (channel.Name.Any(p => p > 255)) {
                return false;
            }

            string formattedlikemad = channel.Name.Replace(" ", "").Replace("-", "").Replace(",", "").Replace(".", "").ToUpper();

            if (formattedlikemad.Contains("FRISK") || formattedlikemad.Contains("MATTIAS") || formattedlikemad.Contains("HUGO") || formattedlikemad.Contains("FAGGOT") ||
                formattedlikemad.Contains("AKALI") || formattedlikemad.Contains("CATPCHA")) {
                return false;
            }

            return true;

        }

        Dictionary<ulong, string> history = new Dictionary<ulong, string>();

        List<string> sortedEdits = new List<string>();
        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content.StartsWith("!version")) {
                await message.Channel.SendMessageAsync(_version);
            }

            if (message.Content == "!id") {
                await message.Channel.SendMessageAsync(message.Channel.Id + "," + string.Join(";", _client.Guilds.Select(p => p.Id)));
            }

            if (message.Content == "!kodändring") {
                await message.Channel.SendMessageAsync("lol");
            }

            if (message.Content.StartsWith("!remindme")) {
                try {
                    var split = message.Content.Split(" ");
                    var date = DateTime.ParseExact(split[1] + " " + split[2], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    var remindMessage = string.Join(" ", split.Skip(2));
                    var chnl = message.Channel as SocketGuildChannel;

                    _remindMes.Add(new RemindMe() { Date = date, Message = remindMessage, ChannelId = message.Channel.Id, User = message.Author.Mention, GuildId =  chnl.Guild.Id});

                    System.IO.File.WriteAllText("/data/test.txt", Newtonsoft.Json.JsonConvert.SerializeObject(_remindMes));

                    await message.Channel.SendMessageAsync("jag kommer påminna dig då jag är en duktig båt");
                } catch (Exception exc) {
                    await message.Channel.SendMessageAsync("du formaterade skiten fel, fråga frisk hur man gör: " + exc.Message);
                }
            }

            if(message.Content.StartsWith("!strlen ")) {
                await message.Channel.SendMessageAsync(message.Content.Substring(8).Length.ToString() + " tecken lång, jävla dålig stil att viktor inte fixade detta");
            }

            if (message.Content.StartsWith("!strlen2 ")) {
                await message.Channel.SendMessageAsync(System.Text.Encoding.UTF8.GetBytes(message.Content.Substring(9)).Length.ToString() + ", " + Convert.ToString(System.Text.Encoding.UTF8.GetBytes(message.Content.Substring(9)).Length, 16) + " tecken lång, jävla dålig stil att viktor inte fixade detta");
            }

            if (message.Author.Id == 297436465565007872) {
                // stolen from https://stackoverflow.com/questions/10576686/c-sharp-regex-pattern-to-extract-urls-from-given-string-not-full-html-urls-but
                var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                foreach (Match m in linkParser.Matches(message.Content).Take(1)) {
                    var tags = await Services.ImageTagListerService.GetImageTags(m.Value);

                    if (tags.Any(p => p == "monkey")) {
                        await message.Channel.SendMessageAsync("sluta stål");
                        await message.DeleteAsync();
                    }
                }

                foreach (string url in message.Attachments.Select(p => p.Url)) {
                    var tags = await Services.ImageTagListerService.GetImageTags(url);

                    if (tags.Any(p => p.ToUpper() == "MONKEY")) {

                        await message.Channel.SendMessageAsync("sluta stål");
                        await message.DeleteAsync();
                    }
                }
            }

            if(message.Content.ToUpper().Contains("BÅT")) {
                var sentiment = await Services.SentimentService.GetSentiment(message.Content);

                if(sentiment > 0.70) {
                    await message.Channel.SendMessageAsync("tack :3");
                } else if(sentiment < 0.30) {
                    await message.Channel.SendMessageAsync("sluta mobbas");
                }
            }

            if ((message.Content.StartsWith("!exterminatus") || message.Content.StartsWith("!purge")) && message.Channel.Id == 84660308882239488) {
                var guild = _client.Guilds.FirstOrDefault(p => p.Id == 84660308882239488);

                if (guild != null) {
                    var channels = guild.Channels.Where(p => isfriendochannel(p));

                    if (channels.Count() > 12) {
                        channels = channels.OrderBy(p => p.CreatedAt).Reverse().Take(12);
                    }

                    var survivors = new HashSet<ulong>(channels.Select(p => p.Id));

                    //foreach (var channel in guild.Channels.Where(p => !survivors.Contains(p.Id)))
                    //{
                    //    await channel.DeleteAsync();
                    //}

                    //await message.Channel.SendMessageAsync(string.Join(" ", guild.Channels.Where(p => !survivors.Contains(p.Id)).Select(p => p.Name)));
                }
            }

            history.Add(message.Id, message.Content);

            if (message.Content.StartsWith("!help")) {
                if (message.Content.ToLower() == "!help nilaus") {
                    await message.Channel.SendMessageAsync("HEY MUFFIN! HELP NILAUS BULLY BUM");
                } else if (message.Content.ToLower() == "!help viktor") {
                    await message.Channel.SendMessageAsync("böghög");
                } else if (message.Content.Length > 5) {
                    await message.Channel.SendMessageAsync("HEY! DONT BULLY" + message.Content.Substring(5).ToUpper());
                } else {
                    await message.Channel.SendMessageAsync("HEY! DONT BULLY FRISK");
                }
            }

            if (message.Content.StartsWith("!clown")) {
                await message.Channel.SendMessageAsync("If frisk is the clown wolf does that make me a clown bot? :(");
            }

            if (message.Content.StartsWith("!edit ") && message.Channel.Id == 503278200064049152) {
                int index = -1;

                if (int.TryParse(message.Content.Substring(6), out index) && index < sortedEdits.Count) {
                    await message.Channel.SendMessageAsync(sortedEdits.Skip(sortedEdits.Count - 1 - index).First());
                }
            }

            if (message.Content.StartsWith("!uptime")) {
                await message.Channel.SendMessageAsync("I've been alive for " + (DateTime.Now - _started).TotalHours.ToString("#.##") + " hours");
            }

            if (message.Content.StartsWith("!tupptid")) {
                DateTime now = DateTime.Now;
                TimeSpan toEarly = new TimeSpan(13, 0, 0);
                TimeSpan toLate = new TimeSpan(18, 0, 0);

                if (now.TimeOfDay > toEarly && now.TimeOfDay < toLate) {
                    await message.Channel.SendMessageAsync("Perfekt tid för en tupplur!");
                }
                else if (now.TimeOfDay < toEarly) {
                    await message.Channel.SendMessageAsync("För tidigt, jobba istället din slacker!");
                } else if (now.TimeOfDay > toLate){
                    await message.Channel.SendMessageAsync("Nu är det kväll, ingen tupplur då!");
                }
            }

            if (message.Content.StartsWith("!calc") && message.Content.Substring(5).Replace(" ", "").ToUpper() == "KATTEN+MUSEN") {
                await message.Channel.SendMessageAsync("tiotusen");
            } else if(message.Content.StartsWith("!calc") && message.Content.Substring(5).Replace(" ", "").ToUpper() == "LÄNGDPÅVIKTORSVADERIM") {
                await message.Channel.SendMessageAsync("0.5");
            } else if(message.Content.StartsWith("!calc") && message.Content.Substring(5).Replace(" ", "").ToUpper() == "LÄNGDPÅSTÅLSVADERIM") {
                await message.Channel.SendMessageAsync("0.2");
            } else if(message.Content.StartsWith("!calc") && message.Content.Substring(5).Replace(" ", "").ToUpper() == "LÄNGDPÅFRISKSVADERIM") {
                await message.Channel.SendMessageAsync("0.1");
            } else if(message.Content.StartsWith("!calc") && message.Content.Substring(5).Replace(" ", "").ToUpper() == "STÅL") {
                await message.Channel.SendMessageAsync("venne, inte implementerat än");
            } else if (message.Content.StartsWith("!calc")) {
                try {
                    SuperHappyScript.SuperHappyScript shs = new SuperHappyScript.SuperHappyScript(message.Content.Substring(5).Replace("kotlett", "1").Replace("kottvå", "2").Replace("kottre", "3"));
                    var bla = new Dictionary<string, double>();

                    await message.Channel.SendMessageAsync(shs.Eval(bla).ToString());
                } catch (Exception exc) {
                    await message.Channel.SendMessageAsync("Felis: " + exc.Message);
                }
            }

            if (message.Content.Replace(" ", "") == "<a:glenoeoeogif:567000592979853312><a:glenoeoeogif:567000592979853312>")
                await message.Channel.SendMessageAsync("<a:glenoeoeogif:567000592979853312>");

            if (message.Content == "!vecka")
                await message.Channel.SendMessageAsync(System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString());

            if (message.Content == "!datum") {
                DateTime date = DateTime.Now;

                if (date > new DateTime(date.Year, 10, 1)) {
                    var days = (date - new DateTime(date.Year, 10, 1)).Days + 1;

                    await message.Channel.SendMessageAsync("It's the " + days + "st of October, " + date.Year);
                } else {
                    var days = (date - new DateTime(date.Year - 1, 10, 1)).Days + 1;

                    await message.Channel.SendMessageAsync("It's the " + days + "st of October, " + (date.Year - 1));
                }
            }

            if (message.Content.StartsWith("!cat ")) {
                await message.Channel.SendMessageAsync("https://cataas.com/cat/says/" + Uri.EscapeDataString(message.Content.Substring(5)) + "?" + _rnd.Next());
            } else if (message.Content == "!cat") {
                await message.Channel.SendMessageAsync("https://cataas.com/cat?" + _rnd.Next());
            }

            if (message.Content == "!dog") {
                try {
                    HttpClient dogFetcher = new HttpClient();
                    var temp = await dogFetcher.GetStringAsync("https://dog.ceo/api/breeds/image/random");

                    dynamic dogguJson = JObject.Parse(temp);

                    await message.Channel.SendMessageAsync((string)dogguJson.message);
                } catch (Exception exc) {
                    await message.Channel.SendMessageAsync("Något gick jättefel :( " + exc.Message);
                }
            }

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");
        }
    }
}
