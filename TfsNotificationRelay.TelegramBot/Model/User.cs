using RestSharp.Deserializers;


namespace DevCore.TfsNotificationRelay.TelegramBot.Model
{
	public class User
	{
		[DeserializeAs(Name = "id")]
		public int Id { get; internal set; }

		[DeserializeAs(Name = "first_name")]
		public string FirstName { get; internal set; }

		[DeserializeAs(Name = "last_name")]
		public string LastName { get; internal set; }

		[DeserializeAs(Name = "username")]
		public string Username { get; internal set; }
	}
}