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
				})
				.ToArray();

			var examples = cb.Contexts()
				.Build()
				.AllContexts()
				.SelectMany(context => context.Examples)
				.Select(example => example.FullName())
				.ToArray();

			var result = from m in methods
						 from e in examples
						 let name = (m.Class + ". " + m.Method).Replace("_", " ")
						 where e.Contains(name)
						 select new TestCaseDTO
						 {
							 Name = e,
							 FileName = m.Location.FileName,
							 MinLineNumber = m.Location.MinLineNumber,
							 MaxLineNumber = m.Location.MaxLineNumber
						 };

			return result.ToArray();
		}
	}
}
