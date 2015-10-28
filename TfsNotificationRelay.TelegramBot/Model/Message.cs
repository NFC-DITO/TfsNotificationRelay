using System;
using RestSharp.Deserializers;


namespace DevCore.TfsNotificationRelay.TelegramBot.Model
{
	public class Message
	{
		[DeserializeAs(Name = "message_id")]
		public int MessageId { get; internal set; }

		[DeserializeAs(Name = "from")]
		public User From { get; internal set; }

		[DeserializeAs(Name = "date")]
		public DateTime Date { get; internal set; }

		[DeserializeAs(Name = "chat")]
		public Chat Chat { get; internal set; }

		[DeserializeAs(Name = "forward_from" )]
		public User ForwardFrom { get; internal set; }

		[DeserializeAs(Name = "forward_date" )]
		public DateTime ForwardDate { get; internal set; }

		[DeserializeAs(Name = "reply_to_message" )]
		public Message ReplayToMessage { get; internal set; }

		[DeserializeAs(Name = "text" )]
		public string Text { get; internal set; }

		//[DeserializeAs(Name = "audio" )]
		//public Audio Audio { get; internal set; }

		//[DeserializeAs(Name = "document" )]
		//public Document Document { get; internal set; }

		//[DeserializeAs(Name = "photo" )]
		//public PhotoSize[] Photo { get; internal set; }

		//[DeserializeAs(Name = "sticker" )]
		//public Sticker Sticker { get; internal set; }

		//[DeserializeAs(Name = "video" )]
		//public Video Video { get; internal set; }

		//[DeserializeAs(Name = "voice" )]
		//public Voice Voice { get; internal set; }

		[DeserializeAs(Name = "caption" )]
		public string Caption { get; internal set; }

		//[DeserializeAs(Name = "contact" )]
		//public Contact Contact { get; internal set; }

		//[DeserializeAs(Name = "location" )]
		//public Location Location { get; internal set; }

		[DeserializeAs(Name = "new_chat_participant" )]
		public User NewChatParticipant { get; internal set; }

		[DeserializeAs(Name = "left_chat_participant" )]
		public User LeftChatParticipant { get; internal set; }

		[DeserializeAs(Name = "new_chat_title" )]
		public string NewChatTitle { get; internal set; }

		//[DeserializeAs(Name = "new_chat_photo" )]
		//public PhotoSize[] NewChatPhoto { get; internal set; }

		[DeserializeAs(Name = "delete_chat_photo" )]
		public bool DeleteChatPhoto { get; internal set; }

		[DeserializeAs(Name = "group_chat_created" )]
		public bool GroupChatCreated { get; internal set; }

		public MessageType Type
		{
			get
			{
				if (!string.IsNullOrEmpty(Text))
					return MessageType.TextMessage;
				//if (this.Audio != null)
				//	return MessageType.AudioMessage;
				//if (this.Document != null)
				//	return MessageType.DocumentMessage;
				//if (this.Photo != null)
				//	return MessageType.PhotoMessage;
				//if (this.Sticker != null)
				//	return MessageType.StickerMessage;
				//if (this.Video != null)
				//	return MessageType.VideoMessage;
				//if (this.Voice != null)
				//	return MessageType.VoiceMessage;
				//if (this.Contact != null)
				//	return MessageType.ContactMessage;
				//if (this.Location != null)
				//	return MessageType.LocationMessage;
				throw new FormatException("MessageType unknown");
			}
		}
	}
}
