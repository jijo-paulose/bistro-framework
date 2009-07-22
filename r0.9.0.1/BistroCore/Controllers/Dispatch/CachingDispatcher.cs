using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Bistro.Configuration.Logging;

namespace Bistro.Controller.Dispatch
{
    public class CachingDispatcher: ControllerDispatcher
    {
        enum Messages
        {
            [DefaultMessage("Cache chain for {0} has been removed. Reason is {1}")]
            ChainRemoved,
            [DefaultMessage("Using cached entry for {0}")]
            UsingCachedEntry,
            [DefaultMessage("Caching entry {0}")]
            CachingEntry
        }

        Cache cache;
        CacheItemRemovedCallback onCacheItemRemoved;

        public CachingDispatcher()
        {
            cache = HttpContext.Current.Cache;
            onCacheItemRemoved = new CacheItemRemovedCallback(CacheItemRemoved);
        }

        public override ControllerInvocationInfo[] GetControllers(string requestUrl)
        {
            var controllers = cache.Get(requestUrl) as ControllerInvocationInfo[];

            if (controllers != null)
            {
                WSApplication.Application.Report(Messages.UsingCachedEntry, requestUrl);
                return controllers;
            }

            controllers = base.GetControllers(requestUrl);

            cache.Add(
                requestUrl,
                controllers,
                null,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration,
                CacheItemPriority.Normal,
                onCacheItemRemoved);

            WSApplication.Application.Report(Messages.CachingEntry, requestUrl);
            return controllers;
        }

        /// <summary>
        /// Invoked when the system removes an execution chain from cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="reason">The reason.</param>
        public void CacheItemRemoved(string key, Object value, CacheItemRemovedReason reason)
        {
            WSApplication.Application.Report(Messages.ChainRemoved, key, reason.ToString());
        }
    }
}
