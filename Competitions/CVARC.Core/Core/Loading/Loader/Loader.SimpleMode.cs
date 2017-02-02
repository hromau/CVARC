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
        /// Gets the test with the specific name
        /// </summary>
        /// <param name="data"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        public ICvarcTest GetTest(LoadingData data, string testName)
        {
            var assemblyName = data.AssemblyName;
            var level = data.Level;
            Competitions competitions;
            try
            {
                competitions = GetCompetitions(assemblyName, level);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(string.Format("The competition '{0}'.'{1}' were not found", assemblyName, level));
            }
            ICvarcTest test;
            try
            {
                test = competitions.Logic.Tests[testName];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(string.Format("The test with name '{0}' was not found in competitions {1}.{2}", testName, assemblyName, level));
            }
            return test;
        }
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
