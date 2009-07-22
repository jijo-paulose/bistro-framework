using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Configuration.Logging;

namespace NoRecruiters.Logging
{
    public class Log4netLoggerFactory : ILoggerFactory
    {
        static Log4netLoggerFactory()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public ILogger GetLogger(Type type)
        {
            return new Log4netLogger(log4net.LogManager.GetLogger(type));
        }
    }

    public class Log4netLogger: DefaultLogger
    {
        log4net.ILog log;

        public Log4netLogger(log4net.ILog log)
        {
            this.log = log;
        }

        protected override void DoReport(string message, params string[] args)
        {
            log.InfoFormat(message, args);
        }
    }
}
