namespace Application.InfrastructureContracts.SharedProject.DateTimes.DateTimeParser.Interface
{
    public interface IDateTimeParser
    {
        DateTime ParseDateTime(string rfc2822DateTimeString);
    }
}
