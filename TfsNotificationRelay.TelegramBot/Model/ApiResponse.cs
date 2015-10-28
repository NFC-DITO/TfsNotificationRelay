using RestSharp.Deserializers;


namespace DevCore.TfsNotificationRelay.TelegramBot.Model
{
	public class ApiResponse<T>
	{
		[DeserializeAs(Name = "ok")]
		public bool Ok { get; internal set; }

		[DeserializeAs(Name = "result")]
		public T ResultObject { get; internal set; }

		[DeserializeAs(Name = "description")]
		public string Message { get; internal set; }

		[DeserializeAs(Name = "error_code")]
		public int Code { get; internal set; }
	}
}