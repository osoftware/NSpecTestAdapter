using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace NSpec.TestAdapter
{
	[Serializable]
	public class TestResultDTO
	{
		public string TestName { get; set; }

		public string Source { get; set; }

		public TestOutcome Outcome { get; set; }

		public string StackTrace { get; set; }

		public string Message { get; set; }

		public TestResult ToTestResult()
		{
			return new TestResult(new TestCase(this.TestName, NSpecExecutor.Uri, this.Source))
			{
				Outcome = this.Outcome,
				ErrorMessage = this.Message,
				ErrorStackTrace = this.StackTrace
			};
		}
	}
}
