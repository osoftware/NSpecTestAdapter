using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

			Assert.AreEqual(7, sink.TestCases.Count);
		}

		[TestMethod]
		public void Discoverer_should_recognize_tags()
		{
			var sink = new SinkMock();
			var target = new NSpecTestDiscoverer();
			var specs = Path.GetFullPath(@"..\..\..\SampleSpecs\bin\Debug\SampleSpecs.dll");

			target.DiscoverTests(new string[] { specs }, null, null, sink);
			var tags = sink.TestCases.SelectMany(tc => tc.Traits);

			Assert.AreEqual(4, tags.Where(t => t.Name == "describe DeepThought").Count());
			Assert.IsTrue(tags.Any(t => t.Name == "describe Earth"));
			Assert.IsTrue(tags.Any(t => t.Name == "One-should-fail"));
			Assert.IsTrue(tags.Any(t => t.Name == "One-should-pass"));
			Assert.IsTrue(tags.Any(t => t.Name == "Should be skipped"));
			Assert.IsTrue(tags.Any(t => t.Name == "Derived"));
		}
	}
}
