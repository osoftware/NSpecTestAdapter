using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace NSpec.TestAdapter
{
	/// <summary>
	/// Main class used by Visual Studio to execute tests.
	/// </summary>
	[ExtensionUri(NSpecExecutor.UriString)]
	public class NSpecExecutor : MarshalByRefObject, ITestExecutor, IReceiveTestResult
	{
		public const string UriString = "executor://NSpecExecutor";
		public static readonly Uri Uri = new Uri(UriString);
		private IFrameworkHandle frameworkHandle;

		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
		{
			this.frameworkHandle = frameworkHandle;

			foreach (var source in sources)
			{
				using(var sandbox = new Sandbox<Executor>(source))
				{
					frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running: " + source);

					sandbox.Content.Execute(this);
				}
			}
		}

		public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
		{
			this.frameworkHandle = frameworkHandle;

			foreach (var group in tests.GroupBy(t => t.Source))
			{
				frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running selected: " + group.Key);

				using (var sandbox = new Sandbox<Executor>(group.Key))
				{
					sandbox.Content.Execute(this, group.Select(t => t.FullyQualifiedName).ToArray());
				}
			}
		}

		public void Receive(TestResultDTO result)
		{
			this.frameworkHandle.RecordResult(result.ToTestResult());
		}
	}
}
