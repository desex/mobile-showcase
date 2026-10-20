using System.Reflection;
using Best.HTTP.Hosts.Connections;
using Best.HTTP.Shared;

namespace Sotbifay.Launcher.Server
{
    public static class BestHttpUtils
    {
        private static readonly PropertyInfo HttpManagerIsQuittingProperty = typeof(HTTPManager).GetProperty
        (
            "IsQuitting",
            BindingFlags.Static | BindingFlags.Public
        );
        
        private static readonly FieldInfo HttpManagerIsSetupCalledField = typeof(HTTPManager).GetField(
            "IsSetupCalled",
            BindingFlags.Static | BindingFlags.NonPublic
        );
        
        
        private static readonly MethodInfo RequestEventHelperAbortAllRequestsMethod = typeof(RequestEventHelper).GetMethod
        (
            "AbortAllRequests",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );

        private static readonly MethodInfo RequestEventHelperRemoveAllRegisteredRequestsMethod = typeof(RequestEventHelper).GetMethod
        (
            "RemoveAllRegisteredRequests",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );
        
        private static readonly MethodInfo RequestEventHelperClearMethod = typeof(RequestEventHelper).GetMethod
        (
            "Clear",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );
        
        
        private static readonly MethodInfo ConnectionEventClearMethod = typeof(ConnectionEvents).GetMethod
        (
            "Clear",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );
        
        public static bool HttpManager_SetIsQuitting(bool isQuitting)
        {
            if (HttpManagerIsQuittingProperty is null)
                return false;

            var setMethod = HttpManagerIsQuittingProperty.GetSetMethod(true);
            if (setMethod is null)
                return false;

            setMethod.Invoke(null, new object[] { isQuitting });
            return true;
        }
        
        public static bool HttpManager_SetIsSetupCalled(bool isSetup)
        {
            if (HttpManagerIsSetupCalledField is null)
                return false;
            
            HttpManagerIsSetupCalledField.SetValue(null, isSetup);
            return true;
        }

        public static bool RequestEventHelper_AbortAllRequests()
        {
            if (RequestEventHelperAbortAllRequestsMethod is null)
                return false;

            RequestEventHelperAbortAllRequestsMethod.Invoke(null, null);
            return true;
        }

        public static bool RequestEventHelper_RemoveAllRegisteredRequests()
        {
            if (RequestEventHelperRemoveAllRegisteredRequestsMethod is null)
                return false;
            
            RequestEventHelperRemoveAllRegisteredRequestsMethod.Invoke(null, null);
            return true;
        }
        
        public static bool RequestEventHelper_Clear()
        {
            if (RequestEventHelperClearMethod is null)
                return false;
            
            RequestEventHelperClearMethod.Invoke(null, null);
            return true;
        }

        public static bool ConnectionEvents_Clear()
        {
            if (ConnectionEventClearMethod is null)
                return false;
            
            ConnectionEventClearMethod.Invoke(null, null);
            return true;
        }
    }
}