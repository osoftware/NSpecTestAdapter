﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace NSpec.TestAdapter
{
	/// <summary>
	/// Executes tests. Should be used in Sandbox only.
	/// </summary>
	class Executor : DomainProxy, ILiveFormatter
	{
		private IReceiveTestResult observer;

		public void Execute(IReceiveTestResult observer)
		{
			this.observer = observer;

			var finder = new SpecFinder(SandboxedAssembly.GetTypes(), "");
			var cb = new ContextBuilder(finder, new Tags());

			cb.Contexts()
				.Build()
				.ForEach(context => context.Run(this, false));
		}
		public void Execute(IReceiveTestResult observer, string[] testNames)
		{
			this.observer = observer;

			var examples = new HashSet<string>(testNames);

			var finder = new SpecFinder(SandboxedAssembly.GetTypes(), "");
			var cb = new ContextBuilder(finder, new Tags());

			cb.Contexts()
				.Build()
				.SelectMany(c => c.AllExamples())
				.Where(example => examples.Contains(example.FullName()))
				.ForEach(example => example.Context.Run(this, false, example.Context.GetInstance()));
		}

		public void Write(ExampleBase example, int level)
		{
			var result = example.Failed()
				? new TestResultDTO { Outcome = TestOutcome.Failed, StackTrace = example.Exception.StackTrace, Message = example.Exception.Message }
				: new TestResultDTO { Outcome = example.Pending ? TestOutcome.Skipped : TestOutcome.Passed };
			result.TestName = example.FullName();
			result.Source = this.Source;

			this.observer.Receive(result);
		}

		public void Write(Context context)
		{
		}
	}
}
