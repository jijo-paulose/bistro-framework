using System.Collections.Generic;
using Bistro.Tests.Errors;
using Bistro.Methods.Reflection;
using Bistro.Methods;
//using Bistro.Designer.Explorer;

namespace TestDate
{
    public class TestHelper
    {

        List<IErrorDescriptor> errorList = new List<IErrorDescriptor>();
        Binding binding = null;
        TestDescriptor testDescriptor;

        public TestDescriptor GetTestDescriptor
        {
            get { return testDescriptor; }
            set { testDescriptor = value; }
        }

        public Binding GetTestBinding
        {
            get { return binding; }
            set { binding = value; }
        }

        public TestHelper()
        {
            TestData td = new TestData();
            IList<TestDescriptor> test = td.GetTestData;
            BistroEngine e = new BistroEngine();
            e.OnInvalidBinding += new BistroEngine.ControllerEvent(e_OnInvalidBinding);
            e.OnResourceLoop += new BistroEngine.MethodEvent(e_OnResourceLoop);
            e.OnMissingProvider += new BistroEngine.MethodEvent(e_OnMissingProvider);
            e.OnInconsistentResourceType += new BistroEngine.MethodResourceEvent(e_OnInconsistentResourceType);
            
            TestDescriptor descriptor = (TestDescriptor)test[0];
            this.GetTestDescriptor = descriptor;
            e.ProcessControllers(new List<string>(), new List<ITypeInfo>(descriptor.Controllers));
            GetTestBinding = e.Root;
        }

        #region Error handlers

        void e_OnMissingProvider(object sender, BistroEngine.MethodEventArgs e)
        {
            List<string> loopControllers = new List<string>();
            foreach (ITypeInfo ctrType in e.Controllers)
            {
                loopControllers.Add(ctrType.FullName);
            }

            ErrorMissingProvider errMissProv = new ErrorMissingProvider(e.MethodUrl, e.ResourceName, loopControllers.ToArray());
            errorList.Add(errMissProv);

        }

        void e_OnInconsistentResourceType(object sender, BistroEngine.MethodResourceEventArgs e)
        {
            List<string> loopValues = new List<string>();
            foreach (ControllerType ctrType in e.Controllers)
            {
                loopValues.Add(ctrType.Type.FullName);
                loopValues.Add(ctrType.GetResourceType(e.ResourceName));
            }

            ErrorInconsResourceType errInconsResType = new ErrorInconsResourceType(e.MethodUrl, e.ResourceName, loopValues.ToArray());
            errorList.Add(errInconsResType);
        }

        void e_OnResourceLoop(object sender, BistroEngine.MethodEventArgs e)
        {
            List<string> loopControllers = new List<string>();
            foreach (ITypeInfo ctrType in e.Controllers)
            {
                loopControllers.Add(ctrType.FullName);
            }

            ErrorResourceLoop errResLoop = new ErrorResourceLoop(e.MethodUrl, loopControllers.ToArray());
            errorList.Add(errResLoop);
        }

        void e_OnInvalidBinding(object sender, BistroEngine.ControllerEventArgs e)
        {
            ErrorInvalidBinding errBind = new ErrorInvalidBinding(e.Controller.FullName, e.Args);
            errorList.Add(errBind);
        }
        #endregion
    }
}
