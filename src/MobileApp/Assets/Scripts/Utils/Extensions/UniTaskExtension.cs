using System;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;

namespace Sotbify.MobileApp.Utils.Extensions
{
    public static class UniTaskExtension
    {
        public static void ForgetIgnoreOperationCancelledException(this UniTask task, ILogger logger, string message, object context = null)
        {
            task.Forget(e =>
            {
                try
                {
                    throw e;
                }
                catch (OperationCanceledException)
                {
                
                }
                catch (Exception exception)
                {
                    logger.Error(message, exception, context);
                }
            });
        }
        
        public static void ForgetIgnoreOperationCancelledException<T>(this UniTask<T> task, ILogger logger, string message, object context = null)
        {
            task.Forget(e =>
            {
                try
                {
                    throw e;
                }
                catch (OperationCanceledException)
                {
                
                }
                catch (Exception exception)
                {
                    logger.Error(message, exception, context);
                }
            });
        }
        
        public static UniTask Before(this UniTask task, Action before)
        {
            before?.Invoke();
            return task;
        }
    }
}