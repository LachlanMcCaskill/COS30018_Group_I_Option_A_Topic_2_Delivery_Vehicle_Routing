using System.Collections.Generic;
using UnityEngine;

namespace RouteSolver
{
	public class ConstraintAlgorithm : IRouteSolver
    {
        public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, int vehicleCount)
        {
            List<RoutePlan> result = new List<RoutePlan>();
            List<Vector3> pointsToSet = new List<Vector3>();

            points.CopyTo(pointsToSet.ToArray());

			for (int i=0; i<vehicleCount; i++) 
			{
				RoutePlan route = new RoutePlan();
				route.Destinations.Push(start);
				result.Add(route);
			}

            float shortestDistance = Mathf.Infinity;
            float currentDistance = 0;
            Vector3 pointToSet = Vector3.zero;

            foreach(RoutePlan r in result)
            {
                //get the most recent destination and compare distance with every other destination in the scene
                for(int i = 0; i<3; i++) //don't know the capacity of each agent, so assume it is three for each one
                {
                    foreach(Vector3 p in pointsToSet)
                    {
                        //calculate the distance beteween the top item of the stack and all the points
                        currentDistance = Vector3.Distance(r.Destinations.Peek(), p);
                        if(currentDistance < shortestDistance)
                        {
                            pointToSet = p;
                            shortestDistance = currentDistance;
                        }
                    }

                    if(pointToSet != Vector3.zero)
                    {
                        r.Destinations.Push(pointToSet);
                        pointsToSet.Remove(pointToSet);
                        shortestDistance = Mathf.Infinity;
                        currentDistance = 0;
                    }
                }
            }

            return result;
        }
    }
}