//using System.Web.Services.Description;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//
//namespace IgorekBot
//{
//    /// <summary>
//    /// This object represents a custom keyboard with reply options (see Introduction to bots for details and examples).
//    /// </summary>
//    [JsonObject(MemberSerialization = MemberSerialization.OptIn,
//                NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
//    public class ReplyKeyboardMarkup : ReplyMarkup
//    {
//        /// <summary>
//        /// Array of button rows, each represented by an Array of KeyboardButton objects
//        /// </summary>
//        [JsonProperty(Required = Required.Always)]
//        public KeyboardButton[][] Keyboard { get; set; }
//
//        /// <summary>
//        /// Optional. Requests clients to resize the keyboard vertically for optimal fit (e.g., make the keyboard smaller if there are just two rows of <see cref="KeyboardButton"/>). Defaults to <c>false</c>, in which case the custom keyboard is always of the same height as the app's standard keyboard.
//        /// </summary>
//        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
//        public bool ResizeKeyboard { get; set; }
//
//        /// <summary>
//        /// Optional. Requests clients to hide the keyboard as soon as it's been used. Defaults to <c>false</c>.
//        /// </summary>
//        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
//        public bool OneTimeKeyboard { get; set; }
//
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ReplyKeyboardMarkup"/> class.
//        /// </summary>
//        public ReplyKeyboardMarkup() { }
//
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ReplyKeyboardMarkup"/> class.
//        /// </summary>
//        /// <param name="keyboardRow">The keyboard row.</param>
//        /// <param name="resizeKeyboard">if set to <c>true</c> the keyboard resizes vertically for optimal fit.</param>
//        /// <param name="oneTimeKeyboard">if set to <c>true</c> the client hides the keyboard as soon as it's been used.</param>
//        public ReplyKeyboardMarkup(KeyboardButton[] keyboardRow, bool resizeKeyboard = false,
//            bool oneTimeKeyboard = false)
//        {
//            Keyboard = new[]
//            {
//                keyboardRow
//            };
//            ResizeKeyboard = resizeKeyboard;
//            OneTimeKeyboard = oneTimeKeyboard;
//        }
//
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ReplyKeyboardMarkup"/> class.
//        /// </summary>
//        /// <param name="keyboard">The keyboard.</param>
//        /// <param name="resizeKeyboard">if set to <c>true</c> the keyboard resizes vertically for optimal fit.</param>
//        /// <param name="oneTimeKeyboard">if set to <c>true</c> the client hides the keyboard as soon as it's been used.</param>
//        public ReplyKeyboardMarkup(KeyboardButton[][] keyboard, bool resizeKeyboard = false,
//            bool oneTimeKeyboard = false)
//        {
//            Keyboard = keyboard;
//            ResizeKeyboard = resizeKeyboard;
//            OneTimeKeyboard = oneTimeKeyboard;
//        }
//    }
//
//    /// <summary>
//    /// This object represents one button of the reply keyboard. For simple text buttons String can be used instead of this object to specify text of the button. Optional fields are mutually exclusive.
//    /// </summary>
//    [JsonObject(MemberSerialization = MemberSerialization.OptIn,
//                NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
//    public class KeyboardButton
//    {
//        /// <summary>
//        /// Text of the button. If none of the optional fields are used, it will be sent to the bot as a message when the button is pressed
//        /// </summary>
//        [JsonProperty(Required = Required.Always)]
//        public string Text { get; set; }
//
//        /// <summary>
//        /// Optional. If <c>true</c>, the user's phone number will be sent as a contact when the button is pressed. Available in private chats only
//        /// </summary>
//        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
//        public bool RequestContact { get; set; }
//
//        /// <summary>
//        /// Optional. If <c>true</c>, the user's current location will be sent when the button is pressed. Available in private chats only
//        /// </summary>
//        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
//        public bool RequestLocation { get; set; }
//
//        /// <summary>
//        /// Performs an implicit conversion from <see cref="string"/> to <see cref="KeyboardButton"/>.
//        /// </summary>
//        /// <param name="key">The key.</param>
//        /// <returns>
//        /// The result of the conversion.
//        /// </returns>
//        public static implicit operator KeyboardButton(string key)
//            => new KeyboardButton(key);
//        
//        /// <summary>
//        /// Initializes a new instance of the <see cref="KeyboardButton"/> class.
//        /// </summary>
//        /// <param name="text">The <see cref="Text"/></param>
//        public KeyboardButton(string text)
//        {
//            Text = text;
//        }
//    }
//    /// <summary>
//    /// Defines how clients display a reply interface to the user
//    /// </summary>
//    /// <seealso cref="IReplyMarkup" />
//    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
//    public abstract class ReplyMarkup : IReplyMarkup
//    {
//        /// <summary>
//        /// Optional. Use this parameter if you want to show the keyboard to specific users only. Targets: 1) users that are @mentioned in the text of the Message object; 2) if the bot's message is a reply (has <see cref="Message.ReplyToMessage"/>), sender of the original message.
//        /// Example: A user requests to change the bot's language, bot replies to the request with a keyboard to select the new language. Other users in the group don't see the keyboard.
//        /// </summary>
//        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
//        public bool Selective { get; set; }
//    }
//
//    /// <summary>
//    /// Objects implementing this Interface define how a <see cref="User"/> can reply to the sent <see cref="Message"/>
//    /// </summary>
//    public interface IReplyMarkup
//    {
//    }
//}