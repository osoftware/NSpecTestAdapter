using System.Linq;
using NSpec.Domain;

namespace NSpec.TestAdapter
{
	/// <summary>
	/// Discovers tests in an assembly. Should be used in Sandbox only.
	/// </summary>
	class Discoverer : DomainProxy
	{
		public string[] DiscoverTests()
		{
			var finder = new SpecFinder(SandboxedAssembly.GetTypes(), "");
			var cb = new ContextBuilder(finder, new Tags());

			return cb.Contexts()
				.Build()
				.AllContexts()
				.SelectMany(context => context.Examples)
				.Select(example => example.FullName())
				.ToArray();
		}
	}
}
