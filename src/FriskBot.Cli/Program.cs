using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    class Program
    {
        private readonly DiscordSocketClient _client;
        private const string _version = "v0.1.2";
        private DateTime _started = DateTime.Now;

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }

        public Program()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.MessageUpdated += MessageUpdatedAsync;
        }

        public async Task MainAsync(string[] args)
        {
            string token = "";

            try {
                token = args[0];
            } catch {
                throw new ApplicationException($"Auth token is not valid, \"{token}\".");
            }

            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _client.SetGameAsync("Sekiro 2: Electric Boogaloo");

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            await (_client.GetChannel(Convert.ToUInt64("503278200064049152")) as ISocketMessageChannel).SendMessageAsync($"Hello, my name is FriskBot {_version}!");

            //return Task.CompletedTask;
        }

        private async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (arg2.EditedTimestamp != null) {
                sortedEdits.Add(history[arg2.Id]);

                await arg2.Channel.SendMessageAsync("revisionism!! (han skrev egentligen " + history[arg2.Id] + ")");
                history[arg2.Id] = arg2.Content;
            }
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

            if (message.Content.StartsWith("!version"))
            {
                await message.Channel.SendMessageAsync(_version);
            }
          
            history.Add(message.Id, message.Content);

            if (message.Content.StartsWith("!help")) {
                if (message.Content.ToLower() == "!help nilaus")
                {
                    await message.Channel.SendMessageAsync("HEY MUFFIN! HELP NILAUS BULLY BUM");
                } else if (message.Content.ToLower() == "!help viktor")
                {
                    await message.Channel.SendMessageAsync("böghög");
                } else if(message.Content.Length > 5) {
                    await message.Channel.SendMessageAsync("HEY! DONT BULLY" + message.Content.Substring(5).ToUpper());
                } else {
                    await message.Channel.SendMessageAsync("HEY! DONT BULLY FRISK");
                }
            }

            if (message.Content.StartsWith("!clown")) {
                await message.Channel.SendMessageAsync("If frisk is the clown wolf does that make me a clown bot? :(");
            }

            if(message.Content.StartsWith("!edit ")) {
                int index = -1;

                if(int.TryParse(message.Content.Substring(6), out index) && index < sortedEdits.Count) {
                    await message.Channel.SendMessageAsync(sortedEdits.Skip(sortedEdits.Count - 1 - index).First());
                }
            }

            if(message.Content.StartsWith("!uptime")) {
                await message.Channel.SendMessageAsync("I've been alive for " + (DateTime.UtcNow - _started).TotalHours);
            }

            if (message.Content.StartsWith("!calc") && message.Content.Substring(5).Replace(" ", "").ToUpper() == "KATTEN+MUSEN") {
                await message.Channel.SendMessageAsync("tiotusen");
            } else if (message.Content.StartsWith("!calc")) {
                try {
                    SuperHappyScript.SuperHappyScript shs = new SuperHappyScript.SuperHappyScript(message.Content.Substring(5));
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

                if(date > new DateTime(date.Year, 10, 1)) {
                    var days = (date - new DateTime(date.Year, 10, 1)).Days + 1;

                    await message.Channel.SendMessageAsync("It's the " + days + "st of October, " + date.Year);
                } else {
                    var days = (date - new DateTime(date.Year - 1, 10, 1)).Days + 1;

                    await message.Channel.SendMessageAsync("It's the " + days + "st of October, " + (date.Year - 1));
                }
            }

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");
        }
    }
}
