using System;
using Bistro.Controllers;
using System.Collections.Generic;

using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

namespace NoRecruiters.Controllers.Actions
{
    /// <summary>
    /// Tagging aspect controller. This controller reacts to any request
    /// that has an tagging aspect to it, parsing the {tag} parameter
    /// and modifying the currentTags sesion variable to reflect the added
    /// values. 
    /// </summary>
    [Bind("get ?/with-tag/{tagList}")]
    public class Tag : AbstractController
    {
        [Session]
        protected List<string> currentTags;

        protected string tagList;

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (currentTags == null)
                currentTags = new List<string>();

            foreach (string tag in tagList.Split(','))
                if (!currentTags.Contains(tag))
                    currentTags.Add(tag);
        }
    }
}
