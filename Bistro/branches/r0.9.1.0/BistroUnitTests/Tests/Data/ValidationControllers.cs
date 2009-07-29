using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;
using Bistro.Controllers.Descriptor;
using Bistro.Extensions.Validation;
using Bistro.Extensions.Validation.Common;
using System.Text.RegularExpressions;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers;

namespace Bistro.UnitTests.Tests.Data
{
    class ControllerValidator : Validator<ValidatingController>
    {
        protected override void Define()
        {
            this
                .As("validationTest")
                .Value(c => c.someField)
                    .AliasedAs("otherField")
                    .IsRequired("someField is required")
                    .IsInRange("a", "zzzzzzzzzzzzzzzzzz", "someField must be alpha")
                    .IsLongerThan(2, "someField must be at least two characters in length")
                    .MatchesRegex("ab", RegexOptions.None, "someField must be 'ab'");
        }
    }

    [Bind("/validationTest/{someField}")]
    [ValidateWith(typeof(ControllerValidator))]
    public class ValidatingController : AbstractController, IValidatable
    {
        public string someField;

        [Request]
        public List<IValidationResult> Messages { get; set; }
        public bool IsValid { get; set; }

        public override void DoProcessRequest(IExecutionContext context)
        {
        }
    }

}
