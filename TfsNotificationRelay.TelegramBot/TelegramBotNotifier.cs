using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.Notifications;
using DevCore.TfsNotificationRelay.TelegramBot.Model;
using Microsoft.TeamFoundation.Framework.Server;


namespace DevCore.TfsNotificationRelay.TelegramBot
{
	public class TelegramBotNotifier : INotifier
	{
		// http://apps.timwhitlock.info/emoji/tables/unicode
		readonly byte[] _checkinEmj = { 0xF0, 0x9F, 0x93, 0xA6 };
		readonly byte[] _wichangedEmj = { 0xF0, 0x9F, 0x93, 0x92 };
		readonly byte[] _buildSuccessEmj = { 0xF0, 0x9F, 0x91, 0x8C };
		readonly byte[] _buildFailEmj = { 0xF0, 0x9F, 0x91, 0x8E };

		public async Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
		{
			var apiToken = bot.GetSetting("apiToken");
			var proxyUrl = bot.GetSetting("proxyUrl");
			var proxyUser = bot.GetSetting("proxyUser");
			var proxyPassword = bot.GetSetting("proxyPassword");

			var proxyUri = new Uri(proxyUrl);
			var basicProxy = new WebProxy(proxyUri) { Credentials = new NetworkCredential(proxyUser, proxyPassword) };
			var botClient = new BotClient(apiToken, basicProxy);
			var messages = await GetMessages(botClient, matchingRule.TeamProject);

			var textMessage = CreateTextMessage(notification, bot);

			if (string.IsNullOrEmpty(textMessage))
			{
				requestContext.Trace(0, TraceLevel.Verbose, Constants.TraceArea, "TelegramBotNotifier", "Nothing to send: ", notification);
				return;
			}

			var sended = new List<int>();

			foreach (var message in messages)
			{
				var chatId = message.Chat.Id;

				if (sended.Contains(chatId)) continue;

				requestContext.Trace(0, TraceLevel.Verbose, Constants.TraceArea, "TelegramBotNotifier", "Sending notification to {0}\n{1}", message.From.Username);

				await botClient.SendTextMessage(chatId, textMessage, true);

				sended.Add(chatId);
			}
		}

		public async Task<List<Message>> GetMessages(BotClient botClient, string teamProject)
		{
			var updates = await botClient.GetUpdates();
			var messages = new List<Message>();

			foreach (var update in updates)
			{
				var messageText = update.Message.Text;

				if (!string.IsNullOrEmpty(messageText) && messageText.StartsWith("/tfsproject"))
				{
					if (update.Message.Text.IndexOf(teamProject, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						messages.Add(update.Message);
					}
				}
			}

			return messages;
		}

		private static string GetEmojiString(byte[] data)
		{
			return Encoding.UTF8.GetString(data);
		}

		private string CreateTextMessage(INotification notification, BotElement bot)
		{
			var lines = notification.ToMessage(bot, s => s);
			if (lines == null || !lines.Any()) return null;

			var textMessage = string.Join("\n", lines);

			var buildNotification = notification as BuildCompletionNotification;
			if (buildNotification != null)
			{
				var emoji = buildNotification.IsSuccessful ? GetEmojiString(_buildSuccessEmj) : GetEmojiString(_buildFailEmj);
				textMessage = emoji + " " + textMessage;
			}

			return textMessage.Replace("%br%", "\n")
				.Replace("%checkin%", GetEmojiString(_checkinEmj))
				.Replace("%wichanged%", GetEmojiString(_wichangedEmj));
		}
	}
}
