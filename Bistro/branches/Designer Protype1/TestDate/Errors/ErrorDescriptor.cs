using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Tests.Errors
{
    public interface IErrorDescriptor
    {
        void Validate(IErrorDescriptor errorDesc);
    }

    public class ErrorDescriptor : IErrorDescriptor
    {
        internal ErrorDescriptor()
        {

        }

        #region IErrorDescriptor Members

        public virtual void Validate(IErrorDescriptor errorDesc)
        {
        }

        #endregion
    }
}
