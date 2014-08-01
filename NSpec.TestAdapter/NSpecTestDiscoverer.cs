using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace NSpec.TestAdapter
{
	/// <summary>
	/// Main class used by Visual Studio to discover tests.
	/// </summary>
	[DefaultExecutorUri(NSpecExecutor.UriString)]
	[FileExtension(".dll")]
	[FileExtension(".exe")]
	public class NSpecTestDiscoverer : ITestDiscoverer
	{
		public static Dictionary<string, TestCase> Cache { get; private set;}

		static NSpecTestDiscoverer()
		{
			Cache = new Dictionary<string, TestCase>();
		}

		public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
		{
			Cache.Clear();

			foreach (var source in sources)
			{
				using (var sandbox = new Sandbox<Discoverer>(source))
				{
					sandbox.Content
						.DiscoverTests()
						.Select(name => name.ToTestCase(source))
						.ForEach(tc =>
							{
								Cache.Add(tc.FullyQualifiedName, tc);
								discoverySink.SendTestCase(tc);
							});
				}
			}
		}
	}
}
