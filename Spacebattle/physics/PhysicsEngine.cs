using System;
using Spacebattle.entity;
using System.Collections.Generic;

namespace Spacebattle.physics
{
    //https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-oriented-rigid-bodies--gamedev-8032
    // though note that we don't intend to implement collision for now.
    class PhysicsEngine : IUpdateable
    {

        List<Entity> entities = new List<Entity>();

        public void Update(uint roundNumber)
        {

            //Eventually there may be collisions and gravity, but for now it is assumed that the pilots are skilled enough to avoid collisions, 
            //and that space battles happen in interstellar space far away from large gravity wells.
            // as of now there seems to be a few bugs in the physics... all stop doesn't succeed all the way, so there is either leakage of some sort or some math is messed up :/

            foreach (var entity in entities)
            {
                entity.DoPhysicsStep();
            }
        }

        /// <summary>
        /// The physics engine will update the position, velocity etc of the given object each tick.
        /// </summary>
        /// <param name="entity"></param>
        public void Register(Entity entity)
        {
            if (entity != null)
                entities.Add(entity);
        }

        public void DeRegister(Entity entity)
        {
            if (entities.Contains(entity))
                entities.Remove(entity);
        }
    }



}
