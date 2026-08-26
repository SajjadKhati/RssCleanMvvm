namespace Domain.Utility.Validators
{
    public static class Validator
    {

        public static void ValidateUri(Uri uri)
        {
            if (uri is null)
                throw new ArgumentException($"پارامتر {nameof(uri)} نباید خالی باشد .");
        }

    }
}
