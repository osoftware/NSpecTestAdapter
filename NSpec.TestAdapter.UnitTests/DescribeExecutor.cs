using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSpec.TestAdapter.UnitTests
{
	[TestClass]
	public class DescribeExecutor
	{
		private const string NSpecTestsPath = @"..\..\..\SampleSpecs\bin\Debug\SampleSpecs.dll";
		private const string MSTestTestsPath = @"..\..\..\NSpec.TestAdapter.UnitTests\bin\Debug\NSpec.TestAdapter.UnitTests.dll";

		public class HandleMock : IFrameworkHandle
		{
			public readonly List<TestResult> results = new List<TestResult>();
			public bool called;

			public bool EnableShutdownAfterTestRun
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public int LaunchProcessWithDebuggerAttached(string filePath, string workingDirectory, string arguments, IDictionary<string, string> environmentVariables)
			{
				throw new NotImplementedException();
			}

			public void RecordAttachments(IList<AttachmentSet> attachmentSets)
			{
			}

			public void RecordEnd(TestCase testCase, TestOutcome outcome)
			{
			}

			public void RecordResult(TestResult testResult)
			{
				this.results.Add(testResult);
				this.called = true;
			}

			public void RecordStart(TestCase testCase)
			{
			}

			public void SendMessage(TestMessageLevel testMessageLevel, string message)
			{
			}
		}

		[TestMethod]
		public void Executor_should_run_all_examples()
		{
			var handle = new HandleMock();
			var target = new NSpecExecutor();
			var specs = Path.GetFullPath(NSpecTestsPath);

			target.RunTests(new string[] { specs }, null, handle);

			Assert.AreEqual(7, handle.results.Count);
			Assert.AreEqual(1, handle.results.Where(r => r.Outcome == TestOutcome.Failed).Count());
			Assert.AreEqual(1, handle.results.Where(r => r.Outcome == TestOutcome.Skipped).Count());
			Assert.AreEqual(5, handle.results.Where(r => r.Outcome == TestOutcome.Passed).Count());
		}


		[TestMethod]
		public void Executor_should_run_selected_examples()
		{
			var handle = new HandleMock();
			var target = new NSpecExecutor();
			var specs = Path.GetFullPath(NSpecTestsPath);
			var testCases = new List<TestCase>
			{
				new TestCase("nspec. describe DeepThought. when examined. should know the answer.", NSpecExecutor.Uri, specs)
			};

			target.RunTests(testCases, null, handle);

			Assert.AreEqual(1, handle.results.Count);
			Assert.AreEqual(1, handle.results.Where(r => r.Outcome == TestOutcome.Passed).Count());
		}

        [TestMethod]
        public void Executor_should_run_selected_example_taking_into_consideration_before_all()
        {
            var handle = new HandleMock();
            var target = new NSpecExecutor();
            var specs = Path.GetFullPath(NSpecTestsPath);
            var testCases = new List<TestCase>
			{
				new TestCase("nspec. describe nested before all. method context. lambda context. should output ABCD.", NSpecExecutor.Uri, specs)
			};

            target.RunTests(testCases, null, handle);

            Assert.AreEqual(1, handle.results.Count);
            Assert.AreEqual(1, handle.results.Where(r =>
            {
                return r.Outcome == TestOutcome.Passed;
            }).Count());
        }
	}
}
