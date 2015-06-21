using System;
using System.Collections.Generic;
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

			var selectedExamples = new HashSet<string>(testNames);

			var finder = new SpecFinder(SandboxedAssembly.GetTypes(), "");
			var cb = new ContextBuilder(finder, new Tags());    

		    ContextCollection contextCollection = cb.Contexts().Build();

            FilterOutNonSelectedExamples(contextCollection, selectedExamples);

            contextCollection.ForEach(context =>
            {
                context.Run(this, false);
            });
		}

	    private static void FilterOutNonSelectedExamples(List<Context> contextCollection, HashSet<string> selectedExamples)
	    {
	        contextCollection.ForEach(context =>
	        {
                var contextSelectedExamples = context.Examples.Where(ce => selectedExamples.Contains(ce.FullName()));

                if (contextSelectedExamples.Any())
	            {
                    context.Examples = contextSelectedExamples.ToList();
                }
                else
                {
                    context.Examples.Clear();                    
                }

	            if (context.Contexts == null) return;

	            IEnumerable<Context> innerContextWithExamples = context.Contexts.Where(x => x.AllExamples().Any());
	            if (innerContextWithExamples.Any())
	            {
	                FilterOutNonSelectedExamples(innerContextWithExamples.ToList(), selectedExamples);
	            }
	        });

	    }

	    public void Write(Example example, int level)
		{
	        var result = example.Failed()
				? new TestResultDTO { Outcome = TestOutcome.Failed, StackTrace = GetStackTrace(example.Exception), Message = example.Exception.Message }
				: new TestResultDTO { Outcome = example.Pending ? TestOutcome.Skipped : TestOutcome.Passed };
			result.TestName = example.FullName();
			result.Source = this.Source;

			this.observer.Receive(result);
		}

        private string GetStackTrace(Exception exception)
        {
            while (exception != null)
            {
                var stackTrace = exception.StackTrace;
                if (! string.IsNullOrEmpty(stackTrace))
                {
                    return stackTrace;
                }

                exception = exception.InnerException;
            }

            return null;
        }

	    public void Write(Context context)
		{
		}
	}
}
