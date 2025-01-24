namespace GDC.EventHost.App.Auth
{
    public static class EventHostClaimTypes
    {
        // will need to change
        public static List<string> ClaimsList { get; set; } = 
            new List<string> { "Manage Users", "Send Email" };
    }
}
