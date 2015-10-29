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
			var allowedUsers = new[] { "fake" };
			var telegramBotNotifier = new TelegramBotNotifier();

			var updatesEF = telegramBotNotifier.GetBotUpdates(botClient, "e-Factoring");
			var updatesBI = telegramBotNotifier.GetBotUpdates(botClient, "bicenter");

			//DoWork(updatesEF, allowedUsers);
			//DoWork(updatesBI, allowedUsers);

			Console.ReadKey();
		}
	}
}
