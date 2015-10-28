using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.TelegramBot.Model;
using RestSharp;


namespace DevCore.TfsNotificationRelay.TelegramBot
{
	public class BotClient
	{
		private readonly RestClient _client;

		public BotClient(string apiToken, IWebProxy proxy = null)
		{
			_client = new RestClient("https://api.telegram.org/bot" + apiToken)
			{
				Proxy = proxy
			};
		}

		public async Task<User> GetMe()
		{
			var request = new RestRequest("getMe", Method.GET);
			var response = await _client.ExecuteGetTaskAsync<ApiResponse<User>>(request);

			return response.Data.ResultObject;
		}

		public async Task<List<Update>> GetUpdates(int lastUpdatesId = 0)
		{
			var request = new RestRequest("getUpdates", Method.GET);

			var response = await _client.ExecuteGetTaskAsync<ApiResponse<List<Update>>>(request);

			return response.Data.ResultObject;
		}

		public async Task<Message> SendTextMessage(int chatId, string text, bool useMarkdown = false)
		{
			var request = new RestRequest("sendMessage", Method.POST) { RequestFormat = DataFormat.Json };

			request.AddParameter("chat_id", chatId);
			request.AddParameter("text", text);

			if (useMarkdown)
				request.AddParameter("parse_mode", "Markdown");

			request.AddParameter("disable_web_page_preview", true);

			var response = await _client.ExecutePostTaskAsync<ApiResponse<Message>>(request);

			return response.Data.ResultObject;
		}
	}
}