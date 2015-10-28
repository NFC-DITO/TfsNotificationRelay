using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.TelegramBot;
using DevCore.TfsNotificationRelay.TelegramBot.Model;


namespace TelegramBotTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var basicProxy = WebRequest.GetSystemWebProxy();
			basicProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

			var botClient = new BotClient("162741643:AAGNaxXv56htAOi00TtW2seTSgfOkJJS0zA", basicProxy);
			var updates = botClient.GetUpdates().Result;

			var messages = new List<Message>();

			foreach (var update in updates)
			{
				if (update.Message.Text != null && update.Message.Text.StartsWith("/tfsproject"))
				{
					if (update.Message.Text.IndexOf("e-Factoring", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						messages.Add(update.Message);
					}
				}
			}
			
		}
	}
}
