namespace GDC.EventHost.App.Utilities
{
    public static class Extensions
    {
        public static DateTime WithoutSeconds(this DateTime date)
        {
            return date.Date + new TimeSpan(date.TimeOfDay.Hours, date.TimeOfDay.Minutes, 0);
        }
    }
}
