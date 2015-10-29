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
			var allowedUsers = bot.GetCsvSetting("allowedUsers")?.ToArray();

			var proxyUri = new Uri(proxyUrl);
			var basicProxy = new WebProxy(proxyUri) { Credentials = new NetworkCredential(proxyUser, proxyPassword) };
			var botClient = new BotClient(apiToken, basicProxy);

			var botUpdates = GetBotUpdates(botClient, matchingRule.TeamProject);

			var messageText = CreateMessageText(notification, bot);

			if (string.IsNullOrEmpty(messageText))
			{
				requestContext.Trace(0, TraceLevel.Verbose, Constants.TraceArea, "TelegramBotNotifier", "Nothing to send: {0}", notification.GetType());
				return;
			}

			foreach (var chatId in botUpdates.Keys)
			{
				var message = botUpdates[chatId];
				var user = message.From;

				if (user == null)
				{
					requestContext.Trace(0, TraceLevel.Warning, Constants.TraceArea, "TelegramBotNotifier",
						"User is unknown for chat: {1}. \n {0}", messageText, message.Chat.Id);
					continue;
				}

				var fullName = $"{user.FirstName} {user.LastName}";

				if (allowedUsers != null && !allowedUsers.Contains(user.Username))
				{
					requestContext.Trace(0, TraceLevel.Warning, Constants.TraceArea, "TelegramBotNotifier",
						"Access denied for {1} ({0}). \nAllowed users: {2}", fullName, user.Username, string.Join(",", allowedUsers));
					continue;
				}

				var result = botClient.SendTextMessage(message.Chat.Id, messageText, true).Result;

				requestContext.Trace(0, TraceLevel.Verbose, Constants.TraceArea, "TelegramBotNotifier",
					"Send notification to {1} ({0}). \nMessageId: {3} \n{2}", fullName, user.Username, messageText, result.MessageId);
			}
		}

		public Dictionary<int, Message> GetBotUpdates(BotClient botClient, string teamProject)
		{
			var updates = botClient.GetUpdates().Result;
			var messages = new Dictionary<int, Message>();

			foreach (var update in updates)
			{
				var message = update.Message;

				var messageText = message.Text;

				if (string.IsNullOrEmpty(messageText)) continue;

				if (message.Chat == null) continue;

				var chatId = message.Chat.Id;

				if (update.Message.Text.IndexOf(teamProject, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					if (messageText.StartsWith("/subscribe_tfs") && !messages.ContainsKey(chatId))
						messages.Add(chatId, message);

					if (messageText.StartsWith("/unsubscribe_tfs") && messages.ContainsKey(chatId))
						messages.Remove(chatId);
				}
			}

			return messages;
		}

		private static string GetEmojiString(byte[] data)
		{
			return Encoding.UTF8.GetString(data);
		}

		private string CreateMessageText(INotification notification, BotElement bot)
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
