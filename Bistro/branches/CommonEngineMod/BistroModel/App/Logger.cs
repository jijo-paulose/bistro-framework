using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using BistroApi;

namespace BistroModel
{
    /// <summary>
    /// Default logger implementation. This class will log to debug-out
    /// </summary>
    public class Logger : LoggerBase
    {
        /// <summary>
        /// Reports the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="args">The args.</param>
        public override void Report(Enum code, params object[] args)
        {
            Report(code, null, args);
        }

        /// <summary>
        /// Reports the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="e">The e.</param>
        /// <param name="args">The args.</param>
        public override void Report(Enum code, Exception e, params object[] args)
        {
            DoReport(GetMessageFormat(code), args);
            ReportException("Exception", e);
        }

				public override Exception Report(Exception e) {
					ReportException("Exception", e);
					return e;
				}

        /// <summary>
        /// Retrieves the message format from the DefaultMessage attribute decorating the
        /// message code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected string GetMessageFormat(Enum code)
        {
            System.Reflection.FieldInfo fi = code.GetType().GetField(code.ToString());
            var attributes = (DefaultMessageAttribute[])fi.GetCustomAttributes(typeof(DefaultMessageAttribute), false);
            return (attributes.Length > 0)
                ? attributes[0].Message
                : code.GetType().DeclaringType.FullName + "." + code.ToString();
        }

        /// <summary>
        /// Retrieves the message format from the DefaultMessage attribute decorating the
        /// message code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected Severity GetMessageSeverity(Enum code)
        {
            System.Reflection.FieldInfo fi = code.GetType().GetField(code.ToString());
            var attributes = (SeverityLevelAttribute[])fi.GetCustomAttributes(typeof(SeverityLevelAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Severity : Severity.Information;
        }

        /// <summary>
        /// Reports the exception.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="e">The e.</param>
        private void ReportException(string header, Exception e)
        {
            if (e == null)
                return;
            ReportException("Inner Exception: ", e.InnerException);
            DoReport("\t\t" + header + ": " + e.Message);
            DoReport("\t\tStack Trace : \n\n");
            DoReport(e.StackTrace);
        }

        /// <summary>
        /// Actually writes out to debug out. Classes wishing to simply intercept and redirect 
        /// the message should override this method.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments for formatting the message.</param>
        protected virtual void DoReport(string message, params object[] args) {
            Debug.WriteLine(String.Format(message, args));
        }
    }
}
