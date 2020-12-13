using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.ProfessionalTranslator.Net
{
    internal class Rules
    {
        internal enum Passed
        {
            Yes,
            No
        }

        internal static Passed MinIntValue(int input, string name, int minValue, ref List<string> messages)
        {
            var output = Passed.Yes;

            // ReSharper disable once InvertIf
            if (input < minValue)
            {
                output = Passed.No;
                messages.Add($"{name} must be more than {minValue}.");
            }

            return output;
        }

        internal static Passed StringRequired(string input, string name, ref List<string> messages)
        {
            var output = Passed.Yes;

            // ReSharper disable once InvertIf
            if (string.IsNullOrEmpty(input))
            {
                output = Passed.No;
                messages.Add($"{name} cannot be empty.");
            }

            return output;
        }

        internal static Passed StringRequiredMaxLength(string input, string name, int maxLength, ref List<string> messages)
        {
            var output = Passed.Yes;

            if (string.IsNullOrEmpty(input))
            {
                output = Passed.No;
                messages.Add($"{name} cannot be empty.");
            }
            else if (input.Length > maxLength)
            {
                output = Passed.No;
                messages.Add($"{name} must be {maxLength} characters or fewer.");
            }

            return output;
        }

        internal static Passed ValidateEmailAddress(string input, string name, ref List<string> messages)
        {
            var output = Passed.Yes;

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new System.Net.Mail.MailAddress(input);
            }
            catch
            {
                messages.Add($"{name} is an invalid email address.");
                output = Passed.No;
            }

            return output;
        }

        internal static Passed ValidateUrl(string input, string name, ref List<string> messages)
        {
            var output = Passed.Yes;

            bool result = Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            // ReSharper disable once InvertIf
            if (!result)
            {
                output = Passed.No;
                messages.Add($"{name} is an invalid URL.");
            }

            return output;
        }
    }
}
