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

		public string Category { get; set; }

		public string Message { get; set; }
	}
}
