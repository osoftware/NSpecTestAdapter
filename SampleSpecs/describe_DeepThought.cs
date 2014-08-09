using NSpec;

namespace SampleSpecs
{
	class describe_DeepThought : nspec
	{
		DeepThought dt = new DeepThought();

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

	class describe_Earth : describe_DeepThought
	{
		void when_asked_for_question()
		{
			it["should have the answer"] = todo;
		}
	}
}
