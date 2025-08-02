using System.Text.RegularExpressions;

namespace FiapCloudGames.Domain.Utils
{
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PasswordRegex = new Regex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#+\-_.=])[A-Za-z\d@$!%*?&#+\-_.=]{8,}$",
            RegexOptions.Compiled);

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return PasswordRegex.IsMatch(password);
        }

        public static string GetPasswordRequirements()
        {
            return "A senha deve ter no mínimo 8 caracteres, incluindo pelo menos: " +
                   "uma letra minúscula, uma maiúscula, um número e um caractere especial (@$!%*?&#+\\-_.=).";
        }

        public static List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("A senha é obrigatória.");
                return errors;
            }

            if (password.Length < 8)
                errors.Add("A senha deve ter no mínimo 8 caracteres.");

            if (!password.Any(char.IsLower))
                errors.Add("A senha deve conter pelo menos uma letra minúscula.");

            if (!password.Any(char.IsUpper))
                errors.Add("A senha deve conter pelo menos uma letra maiúscula.");

            if (!password.Any(char.IsDigit))
                errors.Add("A senha deve conter pelo menos um número.");

            if (!password.Any(c => "@$!%*?&#+\\-_.=".Contains(c)))
                errors.Add("A senha deve conter pelo menos um caractere especial (@$!%*?&#+\\-_.=).");

            return errors;
        }
    }
}