using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BistroApi;
using BistroModel;

namespace BistroModel.Test 
{
	public class TestHarness 
	{
		public static IBinding NewBinding() { return new BBinding(); }
		public static IBinding NewBinding(IUrl url) { return new BBinding((BUrl)url); }
		public static IBinding NewBinding(IUrl url, IBinding baseBinding) { return new BBinding((BUrl)url, (BBinding)baseBinding); }
		public static IBinding[] NewBindings(BindAttribute bindAttribute) { return BBinding.Create(bindAttribute); }

		public static IControllerInfo NewControllerInfo(MemberInfo memberInfo, IDispatcher dispatcher) { return new BControllerInfo(memberInfo, dispatcher); }
		public static IControllerInfo NewControllerInfo(string name, IResources resources, bool isSecurity, IControllerHandler controllerHandler) { return new BControllerInfo(name, resources, isSecurity, controllerHandler); }

		public static IEngine NewEngine() { return new BEngine(); }

		public static IResource NewResource() { return new BResource(); }
		public static IResource NewResource(MemberInfo memberInfo) { return new BResource(memberInfo); }
		public static IResource NewResource(string name) { return new BResource(name); }
		public static IResource NewResource(string name, string dataType) { return new BResource(name, dataType); }

		public static IResources NewResources(){return new BResources();}
		public static IResources NewResources(IControllerInfo controllerInfo) { return new BResources(controllerInfo); }

		public static IUrl NewUrl(){return new BUrl();}
		public static IUrl NewUrl(string url){return new BUrl(url);}
		public static IUrl NewUrl(string url, HttpAction defaultAction) { return new BUrl(url, defaultAction); }
	}
}
