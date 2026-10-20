namespace Sotbify.MobileApp.PinPassword
{
    public interface IPinPasswordService
    {
        public bool HasPassword { get; }
        public bool IsValid(string password);
        public bool CheckPassword(string password);
        public void SavePassword(string password);
        public void DeletePassword();
    }
}