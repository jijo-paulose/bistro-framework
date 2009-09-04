using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bistro.Tests.Errors
{
    internal interface IErrorDescriptor
    {
        void Validate(IErrorDescriptor errorDesc);
    }

    internal class ErrorDescriptor : IErrorDescriptor
    {
        internal ErrorDescriptor()
        {

        }

        #region IErrorDescriptor Members

        public virtual void Validate(IErrorDescriptor errorDesc)
        {
            Assert.IsNotNull(errorDesc, "Compared Error is null.");
        }

        #endregion
    }
}
