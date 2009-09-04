using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Tests.Errors;
using Bistro.Tests;

namespace TestDate
{

    public class TestDescriptor
    {
        public string Name { get; set; }
        public TestTypeInfo[] Controllers { get; set; }
        public IErrorDescriptor[] Errors { get; set; }
        public BindingTest[] BindingTree { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public TestDescriptor(string name, TestTypeInfo[] controllers, IErrorDescriptor[] errors, params BindingTest[] bindingTree)
        {
            Name = name;
            Controllers = controllers;
            Errors = errors ?? new IErrorDescriptor[0];
            BindingTree = bindingTree;
        }

        public void ValidateErrors(List<IErrorDescriptor> baseErrorsList)
        {

            IErrorDescriptor[] baseErrorsArr = baseErrorsList.ToArray();

            for (int i = 0; i < baseErrorsArr.Length; i++)
            {
                baseErrorsArr[i].Validate(Errors[i]);
            }

        }

    }
}

