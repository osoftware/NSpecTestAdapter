using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSpec.Domain;
using NSpec.Domain.Extensions;

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

			var methods = finder.SpecClasses()
				.SelectMany(t => t.Methods())
				.Where(m => conv.IsMethodLevelContext(m.Name) || conv.IsMethodLevelExample(m.Name))
				.Select(m => new
				{
					Class = m.DeclaringType.Name,
					Method = m.Name,
					Location = dia.GetNavigationData(m.DeclaringType.FullName, m.Name)
				});

			var examples = cb.Contexts()
				.Build()
				.AllContexts()
				.SelectMany(context => context.Examples)
				.Select(example => example);

			var result = from m in methods
						 from e in examples
						 let name = (m.Class + ". " + m.Method).Replace("_", " ")
						 where e.FullName().Contains(name)
						 select new TestCaseDTO
						 {
							 Name = e.FullName(),
							 FileName = m.Location.FileName,
							 MinLineNumber = m.Location.MinLineNumber,
							 Traits = e.Tags.ToArray()
						 };

			return result.ToArray();
		}
	}
}
