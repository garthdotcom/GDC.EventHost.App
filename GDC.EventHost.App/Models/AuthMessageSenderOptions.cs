namespace GDC.EventHost.App.Models
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string DefaultSenderEmail { get; set; }
        public string DefaultSenderEmailName { get; set; }
        public string DefaultReceiverEmail { get; set; }
    }
}