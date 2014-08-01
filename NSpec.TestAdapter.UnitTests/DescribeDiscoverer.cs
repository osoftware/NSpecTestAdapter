using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSpec.TestAdapter.UnitTests
{
	[TestClass]
	public class DescribeDiscoverer
	{
		class SinkMock : ITestCaseDiscoverySink
		{
			public SinkMock()
			{
				this.TestCases = new List<TestCase>();
			}
			public List<TestCase> TestCases { get; set; }
			public void SendTestCase(TestCase discoveredTest)
			{
				this.TestCases.Add(discoveredTest);
			}
		}

		[TestMethod]
		public void Discoverer_should_return_examples()
		{
			var sink = new SinkMock();
			var target = new NSpecTestDiscoverer();
			var specs = Path.GetFullPath(@"..\..\..\SampleSpecs\bin\Debug\SampleSpecs.dll");

			target.DiscoverTests(new string[] { specs }, null, null, sink);

			Assert.AreEqual(3, sink.TestCases.Count);
		}
	}
}
