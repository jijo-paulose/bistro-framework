using System;
using System.Collections.Generic;
using System.Text;

namespace BistroModel
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class SeverityLevelAttribute : Attribute
    {
        public SeverityLevelAttribute(Severity severity)
        {
            this.severity = severity;
        }

        Severity severity;
        public Severity Severity { get { return severity; } }
    }

    public enum Severity
    {
        Critical = 4,
        Error = 3,
        Warning = 2,
        Information = 1,
        Message = 0
    }

		public enum CopyOfSeverity {
			Critical = 4,
			Error = 3,
			Warning = 2,
			Information = 1,
			Message = 0
		}

		[Flags]
		public enum CopyOfSeverityMask {
			None = 0x00,  // Special value

			#region Severity Levels
			Critical = 0x10,
			Error = 0x08,
			Warning = 0x04,
			Information = 0x02,
			Message = 0x01,
			#endregion

			#region Severity Masks
			All = Critical | Error | Warning | Information | Message,
			Errors = Critical | Error,
			Warnings = Warning,
			Messages = Information | Message
			#endregion
		}

    [Flags]
    public enum SeverityMask
    {
        None = 0x00,  // Special value

        #region Severity Levels
        Critical = 0x10,
        Error = 0x08,
        Warning = 0x04,
        Information = 0x02,
        Message = 0x01,
        #endregion

        #region Severity Masks
        All = Critical | Error | Warning | Information | Message,
        Errors = Critical | Error,
        Warnings = Warning,
        Messages = Information | Message
        #endregion
    }
}