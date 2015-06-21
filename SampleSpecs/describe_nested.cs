using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSpec;

namespace SampleSpecs
{
    class describe_nested_before_all : nspec
    {
        private string sequence;
        private string outerBeforChar;
        private string methodBeforeAll;

        void before_all()
        {
            outerBeforChar = "A";
        }

        void before_each()
        {
            sequence = outerBeforChar + methodBeforeAll + "C";
        }

        void method_context()
        {
            beforeAll = () => methodBeforeAll = "B";

            before = () => sequence += "D";

            context["lambda context"] = () =>
            {
                it["should output ABCD"] = () =>
                {
                    sequence.should_be("ABCD");
                };

                it["should have length 4"] = () =>
                {
                    sequence.Length.should_be(4);
                };
            };

            after = () => sequence += "E";

            afterAll = () => sequence += "G";
        }

        void it_should_output_AC()
        {
            sequence.should_be("AC");
            sequence.Length.should_be(2);
        }

        void after_each()
        {
            sequence += "F";
        }

        void after_all()
        {
            sequence += "H";
        }
    }
}
