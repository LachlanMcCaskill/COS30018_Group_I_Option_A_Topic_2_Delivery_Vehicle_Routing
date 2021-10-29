using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    public class GeneticRouteSolver : IRouteSolver
    {
        const int maxGeneration = 20;

        //  This runs the whole genetic algorithm creating generations and populations of individuals
        //  Returns a solution to the master routing agent
        public List<RoutePlan> Solve(Vector3 start, List<Vector3> _pointsREMOVE, List<TransportAgentIntroductionMessage> agentsWithCapacities, List<DestinationMessage> destinations)
        {
            int specialDestinationCount = 0;
            int specialcapacity = 0;
            int totalCapacity = 0;
            System.Random r = new System.Random();
            List<Generation> allGenerations = new List<Generation>();
            List<RoutePlan> result = new List<RoutePlan>();

            for (int i = 0; i < destinations.Count; i++)
            {
                if (destinations[i].special)
                {
                    specialDestinationCount++;
                }
            }

            for (int i = 0; i < agentsWithCapacities.Count; i++)
            {
                if (agentsWithCapacities[i].Special)
                {
                    specialcapacity += agentsWithCapacities[i].Capacity;
                }
                totalCapacity += agentsWithCapacities[i].Capacity;
            }

            //  if destinations exceed capacity (or special destinations exceed special capacity) take multiple trips
            int minimumSuccessiveTrips = Mathf.CeilToInt((float)destinations.Count / (float)totalCapacity);
            int minimumSuccessiveSpecialTrips = Mathf.CeilToInt((float)specialDestinationCount / (float)specialcapacity);

            //  Trips must be able to cover both total and special capacity
            minimumSuccessiveTrips = Math.Max(minimumSuccessiveTrips, minimumSuccessiveSpecialTrips);

            //  Create initial generation
            Generation currentGeneration = new Generation(start, agentsWithCapacities, r, minimumSuccessiveTrips, destinations);
            Generation bestGeneration = currentGeneration;

            allGenerations.Add(currentGeneration);

            currentGeneration.Print();

            //  This loop creates each new generation until the max generations are reached
            //  Also finds and remembers the best route produced
            for (int i = 0; i < maxGeneration; i++)
            {
                currentGeneration = new Generation(currentGeneration);
                currentGeneration.Print();
                if (currentGeneration.ShortestRoute().TotalDistance < bestGeneration.ShortestRoute().TotalDistance)
                {
                    bestGeneration = currentGeneration;
                    Debug.Log("New Best Route in Generation " + i);
                }
            }

            string log = "Generation " + bestGeneration.number + " Wins! \n";   // With a distance of " + bestGeneration.ShortestRoute().TotalDistance + "\n"
            log += "Total Distance: " + bestGeneration.ShortestRoute().TotalDistance + "\n";
            log +=  bestGeneration.ShortestRoute().GetGeneString();
            Debug.Log(log);

            //  return the best route
            result = bestGeneration.GetRoutePlan();
            return result;
        }
    }
}
