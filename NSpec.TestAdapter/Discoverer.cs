using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSpec.Domain;

namespace NSpec.TestAdapter
{
	/// <summary>
	/// Discovers tests in an assembly. Should be used in Sandbox only.
	/// </summary>
	class Discoverer : DomainProxy
	{
		public TestCaseDTO[] DiscoverTests()
		{
			var conv = new DefaultConventions().Initialize();
			var finder = new SpecFinder(SandboxedAssembly.GetTypes(), "");
			var cb = new ContextBuilder(finder, new Tags());
			var dia = new DiaSession(SandboxedAssembly.Location);

			var examples = cb.Contexts()
				.Build()
				.AllContexts()
				.SelectMany(context => context.Examples);

			var result = from example in examples
						 let method = GetAction(example)
						 let location = dia.GetNavigationData(method.DeclaringType.FullName, method.Name)
							?? new DiaNavigationData(null, 0, 0)
						 select new TestCaseDTO
						 {
							 Name = example.FullName(),
							 FileName = location.FileName,
							 LineNumber = location.MinLineNumber,
							 Traits = example.Tags.ToArray()
						 };

			return result.ToArray();
		}

		public MethodInfo GetAction(Example example)
		{
			if (example is MethodExample)
			{
				return example.GetType()
					.GetField("method", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(example) as MethodInfo;
			}

			var action = example.GetType()
				.GetField("action", BindingFlags.Instance | BindingFlags.NonPublic)
				.GetValue(example) as Action;
			return action.Method;
		}
	}
}
