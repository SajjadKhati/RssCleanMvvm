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

            string[] dateTimeSupportedFormats =
            {
                // با روز هفته، روز دورقمی
                "ddd, dd MMM yyyy HH:mm:ss zzz",
                "ddd, dd MMM yyyy HH:mm zzz",        // بدون ثانیه

                // با روز هفته، روز یک‌رقمی
                "ddd, d MMM yyyy HH:mm:ss zzz",
                "ddd, d MMM yyyy HH:mm zzz",         // بدون ثانیه

                // بدون روز هفته، روز دورقمی
                "dd MMM yyyy HH:mm:ss zzz",
                "dd MMM yyyy HH:mm zzz",             // بدون ثانیه

                // بدون روز هفته، روز یک‌رقمی
                "d MMM yyyy HH:mm:ss zzz",
                "d MMM yyyy HH:mm zzz"               // بدون ثانیه
            };
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(rfc2822DateTimeString, dateTimeSupportedFormats, 
                CultureInfo.InvariantCulture, DateTimeStyles.None);
            // برگرداندن UTC برای ذخیره‌سازی و مقایسهٔ استاندارد
            return dateTimeOffset.UtcDateTime;
        }

    }
}
