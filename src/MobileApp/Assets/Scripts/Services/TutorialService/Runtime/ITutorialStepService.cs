using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Services.TutorialService
{
    public interface ITutorialStepService
    {
        public void Initialize();
        public void Load();
        public bool HasStep(ETutorialStep step);
        public void TutorialStepPassed(ETutorialStep step);
    }
}