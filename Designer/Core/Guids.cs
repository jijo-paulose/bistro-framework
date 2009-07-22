using System;

namespace Bistro.Designer.Core
{
    static class Guids
    {
        public const string guidCorePkgString = "7cabca5e-ce45-4126-aaca-9121947c8e85";
        public const string guidProjectManager = "20EB4A8F-3A72-4abe-88A3-9C044ED4E89E";
        public const string guidCoreCmdSetString = "2c865fa7-9504-4fcc-a68d-04234beda0b7";
        public const string guidToolWindowPersistanceString = "3f3c1dea-5502-43c5-9ccd-e2e642aae265";
//        public const string guidProjectFlavorTypeString = "F1042C9C-091F-4b54-83AB-BF1BC7E3B57E";

        public static readonly Guid guidCorePkg = new Guid(guidCorePkgString);
        public static readonly Guid guidCoreCmdSet = new Guid(guidCoreCmdSetString);
        //public static readonly Guid guidProjectFlavorType = new Guid(guidProjectFlavorTypeString);
    };
}