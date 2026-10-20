using Sotbify.MobileApp.ApplicationUI;
using VContainer.Unity;

namespace Sotbify.MobileApp.ApplicationUIScopeFacade
{
    public interface IApplicationUIScopeFacade
    {
        public IApplicationUI ApplicationUI { get; set; }
        public LifetimeScope Scope { get; set; }
    }
}