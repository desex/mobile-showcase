using Best.HTTP.Request.Authenticators;

namespace Sotbify.MobileApp.Server
{
    public interface IAuthenticatorProvider
    {
        public IAuthenticator GetAuthenticator();
    }
}