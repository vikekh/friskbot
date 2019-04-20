using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ConsoleApp2
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

		// Discord.Net heavily utilizes TAP for async, so we create
		// an asynchronous context from the beginning.
		static void Main(string[] args)
		{
			new Program().MainAsync().GetAwaiter().GetResult();
		}

		public Program()
		{
			// It is recommended to Dispose of a client when you are finished
			// using it, at the end of your app's lifetime.
			_client = new DiscordSocketClient();

			_client.Log += LogAsync;
			_client.Ready += ReadyAsync;
			_client.MessageReceived += MessageReceivedAsync;
		}

		public async Task MainAsync()
		{
			// Tokens should be considered secret data, and never hard-coded.
			await _client.LoginAsync(TokenType.Bot, "NTY4NTgzNDIwNDYxMTg3MTAy.XLkOvw.JXeiv_T8OJfmMSJz3H7TiyNvq3g");
			await _client.StartAsync();

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
		private Task ReadyAsync()
		{
			Console.WriteLine($"{_client.CurrentUser} is connected!");

			return Task.CompletedTask;
		}

		// This is not the recommended way to write a bot - consider
		// reading over the Commands Framework sample.
		private async Task MessageReceivedAsync(SocketMessage message)
		{
			// The bot should never respond to itself.
			if (message.Author.Id == _client.CurrentUser.Id)
				return;

			if (message.Content.StartsWith("!calc")) {
				try {
					SuperHappyScript.SuperHappyScript shs = new SuperHappyScript.SuperHappyScript(message.Content.Substring(5));
					var bla = new Dictionary<string, double>();

					await message.Channel.SendMessageAsync(shs.Eval(bla).ToString());
				} catch(Exception exc) {
					await message.Channel.SendMessageAsync("Felis: " + exc.Message);
				}
			}

			if (message.Content.Replace(" ", "") == "<a:glenoeoeogif:567000592979853312><a:glenoeoeogif:567000592979853312>")
				await message.Channel.SendMessageAsync("<a:glenoeoeogif:567000592979853312>");

			if (message.Content == "!vecka")
				await message.Channel.SendMessageAsync(System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString());

			if (message.Content == "!ping")
				await message.Channel.SendMessageAsync("pong!");
		}
	}
}
