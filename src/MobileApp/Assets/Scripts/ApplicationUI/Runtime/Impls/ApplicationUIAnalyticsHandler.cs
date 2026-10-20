using System;
using Sotbify.MobileApp.Analytics;
using VContainer.Unity;

namespace Sotbify.MobileApp.ApplicationUI
{
    public class ApplicationUIAnalyticsHandler : IInitializable, IDisposable
    {
        private readonly IApplicationUI _applicationUI;
        private readonly ApplicationStateCommonParameter _applicationStateCommonParameter;
        private readonly AnalyticsEvents _analyticsEvents;
        
        private Guid _entryId;

        public ApplicationUIAnalyticsHandler
        (
            IApplicationUI applicationMenu,
            ApplicationStateCommonParameter applicationMenuStateCommonParameter,
            AnalyticsEvents analyticsEvents
        )
        {
            _applicationUI = applicationMenu ?? throw new ArgumentNullException(nameof(applicationMenu));
            _applicationStateCommonParameter = applicationMenuStateCommonParameter
                                                   ?? throw new ArgumentNullException(nameof(applicationMenuStateCommonParameter));
            _analyticsEvents = analyticsEvents ?? throw new ArgumentNullException(nameof(analyticsEvents));
        }

        public void Initialize()
        {
            OnCurrentRequestChanged();
            _applicationUI.CurrentRequestChanged += OnCurrentRequestChanged;
        }

        public void Dispose()
        {
            _applicationUI.CurrentRequestChanged -= OnCurrentRequestChanged;
        }

        private void OnCurrentRequestChanged()
        {
            if (_entryId != Guid.Empty) 
                _analyticsEvents.ExitApplicationState(_entryId);
            
            var stateType = _applicationUI.CurrentRequest?.StateType;
            _applicationStateCommonParameter.State = stateType;
            
            _entryId = Guid.NewGuid();
            _analyticsEvents.EnterApplicationState(_entryId);
        }
    }
}