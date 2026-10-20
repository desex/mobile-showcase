using Sotbify.MobileApp.ApplicationUI;
using VContainer.Unity;

namespace Sotbify.MobileApp.ApplicationUIScopeFacade
{
    public sealed class ApplicationUIScopeFacade : IApplicationUIScopeFacade
    {
        public IApplicationUI ApplicationUI { get; set; }
        public LifetimeScope Scope { get; set; }
    }   
}