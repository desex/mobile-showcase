using System;
using System.Collections.Generic;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Services.TutorialService
{
    public class TutorialStepService : ITutorialStepService
    {
        private readonly List<ETutorialStep> _steps = new();
        private readonly IPreferences _preferences;

        public TutorialStepService(IPreferences preferences)
        {
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public void Initialize()
        {
            _steps.Clear();
            _steps.AddRange((ETutorialStep[])Enum.GetValues(typeof(ETutorialStep)));
            _steps.Remove(ETutorialStep.None);
            _preferences.Set(SavePath.TutorialSteps, new TutorialStepsData { Steps = _steps });
            _preferences.SetBool(SavePath.TutorialStepsInitialized, true);
        }

        public void Load()
        {
            _steps.Clear();

            var data = _preferences.Get<TutorialStepsData>(SavePath.TutorialSteps);
            if (data?.Steps is not null)
                _steps.AddRange(data.Steps);
        }

        public bool HasStep(ETutorialStep targetStep)
        {
            foreach (var step in _steps)
                if (targetStep == step)
                    return true;
            
            return false;
        }

        public void TutorialStepPassed(ETutorialStep step)
        {
            if (_steps.Remove(step) == false)
                return;

            _preferences.Set(SavePath.TutorialSteps, new TutorialStepsData { Steps = _steps });

            if (_steps.Count == 0)
                _preferences.SetBool(SavePath.TutorialPassed, true);

            _preferences.Save();
        }

        [Serializable]
        private class TutorialStepsData
        {
            public List<ETutorialStep> Steps;
        }
    }
}