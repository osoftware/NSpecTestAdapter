using NSpec;

namespace SampleSpecs
{
	class describe_DeepThought : nspec
	{
		DeepThought dt = new DeepThought();

		[Tag("One-should-fail One-should-pass")]
		void when_examined()
		{
			it["should know the answer"] = () => dt.AnswerQuestion().should_be(42);
			it["should know the question"] = () => dt.GetQuestion().should_be("What do you get if you multiply six by nine?");
		}

		void when_asked_for_help_with_finding_question()
		{
			it["should build the Earth"] = () => dt.BuildComputerForFindingQuestion().should_be("Earth");
		}
	}

	[Tag("Derived")]
	class describe_Earth : describe_DeepThought
	{
		[Tag("Should_be_skipped")]
		void when_asked_for_question()
		{
			it["should have the answer"] = todo;
		}
	}
}
