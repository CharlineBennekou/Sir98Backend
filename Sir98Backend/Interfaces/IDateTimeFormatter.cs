namespace Sir98Backend.Interfaces
{
    public interface IDateTimeFormatter
    {
        DateTimeOffset ToDanishLocal(DateTimeOffset utc);

        string DanishDateTime(DateTimeOffset utc);
        string DanishDate(DateTimeOffset utc);
        string DanishTime(DateTimeOffset utc);

    }
}
