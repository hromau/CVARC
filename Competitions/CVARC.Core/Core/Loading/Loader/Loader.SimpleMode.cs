using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;

namespace CVARC.V2
{
	/// <summary>
	/// This part of the class creates a world in the case when no network interaction is required.
	/// </summary>
	partial class Loader
	{
        
        /// <summary>
        /// Creates a world. 
        /// </summary>
        /// <param name="configuration">Contains the cometition/level name and the settings</param>
        /// <param name="controllerFactory">Is used to create controllers, i.e. entities that control the robots</param>
        /// <param name="state">The initial state of the world</param>
        /// <returns></returns>
        public IWorld CreateWorld(Configuration configuration, ControllerFactory controllerFactory, IWorldState state)
		{
			var competitions = GetCompetitions(configuration.LoadingData);
			var world = competitions.Logic.CreateWorld();
			world.Initialize(competitions, configuration, controllerFactory, state);
			world.Exit += ()=>
				controllerFactory.Exit();
			return world;
		}


	}
}
