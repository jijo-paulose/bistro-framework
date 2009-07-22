using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;
using BistroApi;

namespace BistroModel {
	/// <summary>
	/// Controller for handling non-rendered data
	/// </summary>
	[Bind("?", ControllerBindType = BindType.Payload)]
	public class ReturnTypesController : ControllerBase {
		const int BLOCK_SIZE = 8192;

		/// <summary>
		/// Processes the request.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="requestContext"></param>
		public override void ProcessRequest(HttpContextBase context, IContext requestContext) {
			var response = requestContext.Response;

			// if transfer is requested, no return should be done
			if (requestContext.TransferRequested)
				return;

			// we explicitly want to only check for presence of data we care about
			// don't check whether render target has been set, that's not our concern
			if (response.CurrentReturnType == ReturnType.Template ||
					response.ExplicitResult == null)
				return;

			var contentType = response.ContentType;
			var returnValue = response.ExplicitResult;

			context.Response.AppendHeader("content-type", contentType);
			context.Response.StatusCode = Convert.ToInt32(response.Code);

			switch (response.CurrentReturnType) {
				case ReturnType.XML:
				case ReturnType.JSON:
				case ReturnType.Other:
					context.Response.Write(returnValue.ToString());
					break;
				case ReturnType.File:
					var stream = returnValue as Stream;
					var fileName = response.AttachmentName as string;

					if (fileName != null)
						context.Response.AppendHeader("content-disposition", "attachment; filename=" + fileName);

					// we weren't given a stream
					if (stream == null)
						context.Response.WriteFile(returnValue.ToString());
					else {
						// since we were given a stream, we'll have to encode it somehow
						using (stream) {
							Encoding encoding =
									response.Encoding != null ? response.Encoding : new UTF8Encoding();

							byte[] buffer = new byte[BLOCK_SIZE];
							int read = stream.Read(buffer, 0, BLOCK_SIZE);
							while (read > 0) {
								context.Response.Write(encoding.GetChars(buffer, 0, read), 0, read);
								read = stream.Read(buffer, 0, BLOCK_SIZE);
							}

							stream.Close();
						}
					}
					break;
			}

			context.Response.Flush();
			context.Response.Close();
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IController"/> is reusable. If reusable and Stateful,
		/// the recycle method will be called once request processing is complete.
		/// </summary>
		/// <value><c>true</c> if reusable; otherwise, <c>false</c>.</value>
		public override bool Reusable {
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IController"/> is stateful. If reusable and Stateful,
		/// the recycle method will be called once request processing is complete.
		/// </summary>
		/// <value><c>true</c> if stateful; otherwise, <c>false</c>.</value>
		public override bool Stateful {
			get { return false; }
		}

		/// <summary>
		/// Initializes this instance. This method is called before system-manipulated fields have been populated.
		/// </summary>
		public override void Initialize() { }

		/// <summary>
		/// Recycles this instance. Recycle is called after ProcessRequest completes for stateful reusable controllers.
		/// This method is intended to put the controller in a state ready for a new request. This method may not be
		/// called from the request execution thread.
		/// </summary>
		public override void Recycle() { }

		/// <summary>
		/// Gets a global type-system identifier for this class of controllers. In most cases this is simply the Type of
		/// the controller class.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		public override System.Reflection.MemberInfo GlobalHandle {
			get { return GetType(); }
		}
	}
}
