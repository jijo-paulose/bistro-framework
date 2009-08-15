using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers;

namespace Bistro.Entity
{
    /// <summary>
    /// Defines the contract for a class that can perform a mapping between a controller and an entity
    /// </summary>
    public interface IEntityMapper
    {
        /// <summary>
        /// Performs the mapping from the controller to the entity
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="entity">The entity.</param>
        void Map(IController controller, object entity);

        /// <summary>
        /// Performs the mapping from the entity to the controller
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="entity">The entity.</param>
        void Unmap(IController controller, object entity);
    }
}
