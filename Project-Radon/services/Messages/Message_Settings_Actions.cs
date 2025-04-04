namespace Project_Radon.Services.Messages
{
    public enum EnumMessageStatus
    {
        Added,
        Bookmark,
        GoBack,
        GoForward,
        Homepage,
        Informational,
        Login,
        Logout,
        Removed,
        Settings,
        Updated,
        XorError,
    }
    public class Message_Settings_Actions
    {
        public Message_Settings_Actions(string payload, EnumMessageStatus status)
        {
            Payload = payload;
            Status = status;
        }

        public Message_Settings_Actions(string payload) : this(payload, EnumMessageStatus.Updated)
        {
        }

        public Message_Settings_Actions(EnumMessageStatus status) : this(null, status)
        {
        }

        public Message_Settings_Actions(RadonAppSettings payload) : this(null, EnumMessageStatus.Settings)
        {
            AppSettings = payload;
        }

        public RadonAppSettings AppSettings { get; set; }
        public EnumMessageStatus Status { get; }
        public string Payload { get; }
    }
}