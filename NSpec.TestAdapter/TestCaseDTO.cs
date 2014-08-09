using System;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace NSpec.TestAdapter
{
	[Serializable]
	class TestCaseDTO
	{
		public string Name { get; set; }

		public string FileName { get; set; }

		public int MinLineNumber { get; set; }

		public string[] Traits { get; set; }

		public TestCase ToTestCase(string source)
		{
			var tc = new TestCase(this.Name, NSpecExecutor.Uri, source);
			tc.DisplayName = tc.FullyQualifiedName.Remove(0, 7).Replace(Constants.InternalSeparator, Constants.VisualSeparator);
			tc.CodeFilePath = this.FileName;
			tc.LineNumber = this.MinLineNumber;
			tc.Traits.AddRange(this.Traits.Select(t => new Trait(t.Replace("_", " "), null)));
			return tc;
		}
	}
}
