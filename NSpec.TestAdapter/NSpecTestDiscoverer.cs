using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;

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
			testLogger = new TestLogger(logger);

			testLogger.SendMainMessage("Discovery started");

			foreach (var source in sources)
			{
				testLogger.SendDebugMessage(String.Format("Processing: '{0}'", source));

				try
				{
					using (var sandbox = new Sandbox<Discoverer>(source))
					{
						if (sandbox.Content != null)
						{
							sandbox.Content
								.DiscoverTests()
								.Select(name => name.ToTestCase(source))
								.ForEach(discoverySink.SendTestCase);
						}
					}
				}
				catch (Exception ex)
				{
					testLogger.SendErrorMessage(ex, String.Format("Exception found while discovering tests in source '{0}'", source));

					// just go on with the next
				}
			}

			testLogger.SendMainMessage("Discovery finished");
		}

		private TestLogger testLogger;
	}
}
