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
using Bistro.Entity;

namespace Bistro.UnitTests.Tests.Data
{
    public class SimpleEntityValidator: Validator<SimpleEntity>
    {
        protected override void Define()
        {
            As("SimpleEntity")
            .Define(
                Value(c => c.foo).IsRequired("Foo is required")
                );
        }
    }

    [ValidateWith(typeof(SimpleEntityValidator))]
    public class SimpleEntity: IValidatable
    {
        public string foo;
        public string bar;
        public string baz;

        public List<IValidationResult> Messages { get; set; }
        public bool IsValid { get; set; }
    }

    class TestMapper: EntityMapper<EntityController, SimpleEntity>
    {
        public TestMapper()
        {
            Infer()
                .Map(x => x.thirdField).To(y => y.baz);
        }
    }

    class EntityControllerValidator: Validator<EntityController>
    {
        protected override void Define()
        {
            As("Entity")
                .ByMapping();
        }
    }

    [Bind("/entityTest?{foo}&{bar}&{thirdField}&{unwrap}")]
    [ValidateWith(typeof(EntityControllerValidator))]
    [MapsWith(typeof(TestMapper))]
    public class EntityController : AbstractController, IMappable, IValidatable
    {
        [Request]
        public string 
            foo,
            bar,
            thirdField;

        public bool unwrap;

        [FormField, Request]
        public SimpleEntity entity;

        [Request]
        public List<IValidationResult> Messages { get; set; }

        [Request]
        public bool IsValid { get; set; }

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (unwrap)
                Mapper.Unmap(this, entity);
            else
            {
                if (!IsValid)
                    return;

                entity = new SimpleEntity();
                Mapper.Map(this, entity);

                context.Response.Return(entity);
            }
        }

        public IEntityMapper Mapper { get; set;}
    }

}
