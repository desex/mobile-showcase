using System;

namespace Sotbifay.Analytics
{
    /// <summary>Логгер аналитики</summary>
    /// <remarks>
    /// Уровни логирования (<c>logLevel</c>):
    /// <list type="table">
    ///   <item><term>0</term><description>Verbose: максимально подробная трассировка</description></item>
    ///   <item><term>1</term><description>Debug: отладочная информация</description></item>
    ///   <item><term>2</term><description>Information: штатные события работы</description></item>
    ///   <item><term>3</term><description>Warning: нештатная ситуация, не нарушающая работу</description></item>
    ///   <item><term>4</term><description>Error: ошибка, операция не выполнена</description></item>
    ///   <item><term>5</term><description>Fatal: критический сбой, дальнейшая работа невозможна</description></item>
    /// </list>
    /// </remarks>
    public interface IAnalyticsLogger
    {
        /// <summary>Проверяет, включено ли логирование для указанного уровня</summary>
        /// <param name="logLevel">Уровень логирования: 0 = Verbose, 1 = Debug, 2 = Information, 3 = Warning, 4 = Error, 5 = Fatal</param>
        /// <returns><c>true</c>, если сообщения этого уровня будут записаны; иначе <c>false</c>.</returns>
        bool IsEnabled(int logLevel);

        /// <summary>Записывает сообщение в лог</summary>
        /// <param name="logLevel"> Уровень логирования: 0 = Verbose, 1 = Debug, 2 = Information, 3 = Warning, 4 = Error, 5 = Fatal.</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="exception">Связанное исключение или <c>null</c></param>
        /// <param name="context">Дополнительный контекст (объект-источник, данные события) или <c>null</c></param>
        void Log(int logLevel, string message, Exception exception, object context);
    }
}