using RestSharp.Deserializers;


namespace DevCore.TfsNotificationRelay.TelegramBot.Model
{
	public class Update
	{
		[DeserializeAs(Name = "update_id")]
		public int Id { get; internal set; }

		[DeserializeAs(Name = "message")]
		public Message Message { get; internal set; }
	}
}
