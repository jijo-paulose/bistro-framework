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
            this.As("validationTest")
                .Define(
                    Value(c => c.someField)
                        .AliasedAs("otherField")
                        .IsRequired("someField is required")
                        .IsInRange("a", "zzzzzzzzzzzzzzzzzz", "someField must be alpha")
                        .IsLongerThan(2, "someField must be at least two characters in length")
                        .IsOfType(ValidatableTypes.date, "someField must be type of date")
                        .IsOfType(ValidatableTypes.email, "someField must be type of email")
                        .IsOfType(ValidatableTypes.dateISO, "someField must be type of dateISO")
                        .IsOfType(ValidatableTypes.number, "someField must be type of number")
                        .IsOfType(ValidatableTypes.digits, "someField must be type of digits")
                        .IsOfType(ValidatableTypes.url, "someField must be type of url")
                        .MatchesRegex("ab", RegexOptions.None, "someField must be 'ab'"))                       
                .And(
                    Value(c => c.thirdField)
                        .IsRequired("thirdField is required"));
        }
    }

    [Bind("/validationTest/{someField}?{thirdField}")]
    [ValidateWith(typeof(ControllerValidator))]
    public class ValidatingController : AbstractController, IValidatable
    {
        public string someField;

        public string thirdField;

        [Request]
        public List<IValidationResult> Messages { get; set; }
        public bool IsValid { get; set; }

        public override void DoProcessRequest(IExecutionContext context)
        {
        }
    }

}
