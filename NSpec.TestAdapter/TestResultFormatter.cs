using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace NSpec.TestAdapter
{
	class TestResultFormatter : ILiveFormatter
	{
		private IFrameworkHandle frameworkHandle;
		private string source;

		public TestResultFormatter(IFrameworkHandle frameworkHandle, string source)
		{
			this.frameworkHandle = frameworkHandle;
			this.source = source;
		}

		public void Write(Example example, int level)
		{
			var result = new TestResult(example.ToTestCase(this.source));
			if (example.Failed())
			{
				result.Outcome = TestOutcome.Failed;
				result.Messages.Add(new TestResultMessage(example.Spec, example.Exception.Message));
			}
			else
			{
				result.Outcome = TestOutcome.Passed;
			}

			frameworkHandle.RecordResult(result);
		}

		public void Write(Context context)
		{
		}
	}
}
