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

			var testLogger = new TestLogger(frameworkHandle);

			testLogger.SendMainMessage("Execution started");

			foreach (var source in sources)
			{
				try
				{
					using (var sandbox = new Sandbox<Executor>(source))
					{
						testLogger.SendInformationalMessage(String.Format("Running: '{0}'", source));

						sandbox.Content.Execute(this);
					}
				}
				catch (Exception ex)
				{
					testLogger.SendErrorMessage(ex, String.Format("Exception found while executing tests in source '{0}'", source));

					// just go on with the next
				}
			}

			testLogger.SendMainMessage("Execution finished");
		}

		public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
		{
			this.frameworkHandle = frameworkHandle;

			var testLogger = new TestLogger(frameworkHandle);

			testLogger.SendMainMessage("Execution started");

			foreach (var group in tests.GroupBy(t => t.Source))
			{
				testLogger.SendInformationalMessage(String.Format("Running selected: '{0}'", group.Key));

				try
				{
					using (var sandbox = new Sandbox<Executor>(group.Key))
					{
						sandbox.Content.Execute(this, group.Select(t => t.FullyQualifiedName).ToArray());
					}
				}
				catch (Exception ex)
				{
					testLogger.SendErrorMessage(ex, String.Format("Exception found while executing tests in group '{0}'", group.Key));

					// just go on with the next
				}
			}

			testLogger.SendMainMessage("Execution finished");
		}

		public void Receive(TestResultDTO result)
		{
			this.frameworkHandle.RecordResult(result.ToTestResult());
		}
	}
}
