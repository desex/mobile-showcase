using System;
using Sotbify.MobileApp.Common.Preferences;

namespace Sotbify.MobileApp.PinPassword
{
    public sealed class PinPasswordService : IPinPasswordService
    {
        private const string PasswordKey = "Profile.PinPassword";
        private readonly IPreferences _preferences;

        public PinPasswordService(IPreferences preferences)
        {
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public bool HasPassword => IsValid(_preferences.GetString(PasswordKey, string.Empty));

        public bool IsValid(string password)
        {
            if (password == null || password.Length != 4)
                return false;

            foreach (var character in password)
                if (character < '0' || character > '9')
                    return false;

            return true;
        }

        public bool CheckPassword(string password) => IsValid(password) &&
            string.Equals(password, _preferences.GetString(PasswordKey, string.Empty), StringComparison.Ordinal);

        public void SavePassword(string password)
        {
            if (!IsValid(password))
                throw new ArgumentException("PIN must contain exactly four digits.", nameof(password));

            _preferences.SetString(PasswordKey, password);
            _preferences.Save();
        }

        public void DeletePassword()
        {
            _preferences.Remove(PasswordKey);
        }
    }
}