using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BistroApi;

namespace BistroModel
{
	class BRequestFilter {
		#region private
		enum Messages {
			[DefaultMessage("{0} is not a valid extension, and will be skipped")]
			InvalidExtension
		}
		List<string> _allowedExtensions;
		bool _allowAllExtensions;
		List<string> _ignoredDirectories;
		ILogger _logger;
		#endregion

		#region construction
		public BRequestFilter(IApplication application) 
		{
			_logger = application.CreateLogger(null);
			LoadUrlRules(application);
		}
		#endregion

		#region public
		/// <summary>
		/// Determines whether the requested url should be treated as a bistro bind-point
		/// </summary>
		/// <param name="p">The path.</param>
		/// <returns>
		/// 	<c>true</c> if the requested url is valid for processing; otherwise, <c>false</c>.
		/// </returns>
		public bool IsValidPath(string requestPath, string applicationPath) {
			return HasValidExtension(requestPath) && !IgnorePath(requestPath, applicationPath);
		}
		#endregion

		#region private methods
		/// <summary>
		/// Determines whether the path has a valid extension for processing
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		/// 	<c>true</c> if the path has a valid extension; otherwise, <c>false</c>.
		/// </returns>
		bool HasValidExtension(string path) {
			if (_allowAllExtensions)
				return true;

			string extension = VirtualPathUtility.GetExtension(path).ToUpper();
			if (_allowedExtensions == null)
				return String.Empty.Equals(extension);
			else
				return _allowedExtensions.Contains(extension);
		}
		/// <summary>
		/// Determines whether the path starts with an ignored directory
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		/// 	<c>true</c> if the path is on the ignored directory list; otherwise, <c>false</c>.
		/// </returns>
		bool IgnorePath(string requestPath, string applicationPath) {
			if (_ignoredDirectories == null)
				return false;
			else {
				string appRelativePath = requestPath.Substring(applicationPath.Length).TrimStart('/');
				foreach (string directory in _ignoredDirectories)
					if (appRelativePath.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
						return true;

				return false;
			}
		}
		/// <summary>
		/// Loads the allowedExtensions and ignoredDirectories lists. To speed processing, if empty,
		/// these lists should be kept null. For allowedExtensions, if this list is empty, no extensions
		/// are allowed. For ignoredDirectories, if this list is empty, all directories are treated as
		/// bind points. If not empty, the path is taken as an absolute path, starting from the app root
		/// </summary>
		void LoadUrlRules(IApplication app) {
			string[] exts = app.AllowedExtensions;
			string[] dirs = app.IgnoredDirectories;

			if (exts != null && exts.Length > 0) {
				_allowedExtensions = new List<string>(exts.Length);

				foreach (string ext in exts) {
					if (ext.Contains(".")) {
						_logger.Report(Messages.InvalidExtension, ext);
						continue;
					}
					// the empty extension should be added as just that, because the virtual
					// path tool will yield .extension for extensions, and "" for none
					if (ext == String.Empty)
						_allowedExtensions.Add(System.String.Empty);
					else
						_allowedExtensions.Add("." + ext);
				}
				
				_allowAllExtensions = _allowedExtensions.Contains(".*");
			}
			
			if (dirs != null && dirs.Length > 0) {
				_ignoredDirectories = new List<string>(dirs.Length);

				foreach (string dir in dirs)
					_ignoredDirectories.Add(dir.Replace('\\', '/').Trim(' ', '/'));
			}
		}
		#endregion
	}
}
