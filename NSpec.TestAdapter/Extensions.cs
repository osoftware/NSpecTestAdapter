using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSpec.Domain;

namespace NSpec.TestAdapter
{
	static class Extensions
	{
		public static TestCase ToTestCase(this Example example, string source)
		{
			var name = example.FullName();
			return NSpecTestDiscoverer.Cache.ContainsKey(name) ? NSpecTestDiscoverer.Cache[name] : name.ToTestCase(source);
		}

		public static TestCase ToTestCase(this string example, string source)
		{
			var tc = new TestCase(example, NSpecExecutor.Uri, source);
			tc.DisplayName = tc.FullyQualifiedName.Remove(0, 7).Replace(Constants.InternalSeparator, Constants.VisualSeparator);
			return tc;
		}

		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			foreach (var item in sequence)
			{
				action(item);
			}
		}

		public static TestResult ToTestResult(this TestResultDTO ev)
		{
			var result = new TestResult(new TestCase(ev.TestName, NSpecExecutor.Uri, ev.Source));
			result.Outcome = ev.Outcome;
			result.ErrorMessage = ev.Message;
			result.ErrorStackTrace = ev.StackTrace;
			return result;
		}
	}
}
