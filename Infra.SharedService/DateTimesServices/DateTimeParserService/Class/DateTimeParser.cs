using System.Globalization;
using Application.InfrastructureContracts.SharedProject.DateTimes.DateTimeParser.Interface;

namespace Infra.SharedService.DateTimesServices.DateTimeParserService.Class
{
    public class DateTimeParser : IDateTimeParser
    {

        public DateTime ParseDateTime(string rfc2822DateTimeString)
        {
            if (string.IsNullOrWhiteSpace(rfc2822DateTimeString))
                throw new ArgumentException("تاریخ نباید خالی باشد.", nameof(rfc2822DateTimeString));

            string dateTimeFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(rfc2822DateTimeString, dateTimeFormat, 
                CultureInfo.InvariantCulture, DateTimeStyles.None);

            // برگرداندن UTC برای ذخیره‌سازی و مقایسهٔ استاندارد
            return dateTimeOffset.UtcDateTime;
        }

    }
}
