using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
		{
			foreach (var source in sources)
			{
				using (var sandbox = new Sandbox<Discoverer>(source))
				{
					//System.Windows.Forms.MessageBox.Show("DEBUG: " + sandbox.IsValidTestProject);
					if (sandbox.IsValidTestProject)
						sandbox.Content
							.DiscoverTests()
							.Select(name => name.ToTestCase(source))
							.ForEach(discoverySink.SendTestCase);
				}
			}
		}
	}
}
