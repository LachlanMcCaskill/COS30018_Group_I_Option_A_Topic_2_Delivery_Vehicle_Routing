using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    public class ClusterGeneticRouteSolver : IRouteSolver
    {

        public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, List<TransportAgentIntroductionMessage> agentsWithCapacities, List<DestinationMessage> NONE)
        {
            string parameterLog = "Parameters passed to Solve()\n";
            parameterLog += "Start: " + start.ToString() + "\n";
            //  List<Vector3> points = new List<Vector3>();
            //parameterLog += "Points (" + destinations.Count + "): ";
            //for (int i = 0; i < destinations.Count; i++)
            //{
            //    points.Add(destinations[i].Position);
            //    parameterLog += destinations[i].Position.ToString() + " ";
            //}
            parameterLog += "\nAgents: ";
            for (int i = 0; i < agentsWithCapacities.Count; i++)
            {
                parameterLog += agentsWithCapacities[i].Capacity;
            }
            Debug.Log(parameterLog);
            
            int totalCapacity = 0;
            parameterLog += "\nAgents: ";
            for (int i = 0; i < agentsWithCapacities.Count; i++)
            {
                totalCapacity += agentsWithCapacities[i].Capacity;
                parameterLog += agentsWithCapacities[i].Capacity + " ";
            }

            List<RoutePlan> result = new List<RoutePlan>();

            int minimumSuccessiveTrips = totalCapacity; // Mathf.CeilToInt((float)destinations.Count / (float)totalCapacity);

            //  Setting Varaibles
            //  int[] vars = GetVariables();    //  user can input variables or use default variables
            //  int numberPoints = vars[0];
            //  int numberPopulation = vars[1];
            int maxGeneration = 20; //  vars[2];
            //  int numberBuses = vars[3];
            //  List<string> subroutes = new List<string>();
            //  List<float> averages = new List<float>();
            //  List<float> best = new List<float>();

            System.Random r = new System.Random();
            List<ClusterGeneration> allGenerations = new List<ClusterGeneration>();

            ClusterGeneration currentGeneration = new ClusterGeneration(start, points, agentsWithCapacities, r, minimumSuccessiveTrips);
            ClusterGeneration bestGeneration = currentGeneration;

            allGenerations.Add(currentGeneration);

            currentGeneration.Print();

            // create and print next generation
            for (int i = 0; i < maxGeneration; i++)
            {
                currentGeneration = new ClusterGeneration(currentGeneration);
                currentGeneration.Print();
                if (currentGeneration.ShortestRoute().TotalDistance < bestGeneration.ShortestRoute().TotalDistance)
                {
                    bestGeneration = currentGeneration;
                    Debug.Log("New Best Route in Generation " + i);
                }
            }

            result = bestGeneration.GetRoutePlan();

            string log = "Generation " + bestGeneration.number + " Wins! \n";   // With a distance of " + bestGeneration.ShortestRoute().TotalDistance + "\n"

            log += "Total Distance: " + bestGeneration.ShortestRoute().TotalDistance + "\n";
            log +=  bestGeneration.ShortestRoute().GetGeneString();

            Debug.Log(log);

            return result;
        }

        static Vector2[] GeneratePoints(int count, System.Random r)
        {
            Vector2[] points = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                float x = (float)r.NextDouble() * 10;
                float y = (float)r.NextDouble() * 10;
                points[i] = new Vector2(x, y);
            }
            return points;
        }

        static void PrintFinalResults(float firstRoute, float fastestRoute, int fastestGeneration, string fastestSubRoutes, float[] generationAverages, float finalRoute, List<float> averages, List<float> bests, List<string> subroutes)
        {
            Debug.Log("Evolution complete.");
            Debug.Log("The best route in the initial generation was " + firstRoute);
            Debug.Log("The best route in the final generation was " + finalRoute);

            if (finalRoute < firstRoute)
            {
                Debug.Log("Which was " + (firstRoute - finalRoute) + " faster than the initial fastest route ");
            }
            else
            {
                Debug.Log("Which was " + (finalRoute - firstRoute) + " slower than the initial fastest route :(");
            }

            Debug.Log("The fastest route was " + fastestRoute + "[" + fastestSubRoutes + "] in generation " + fastestGeneration);

            if (fastestGeneration == 0)
            {
                Debug.Log("Woops the System.Random generation beat your optimisation algorithm...");
            }
            else
            {
                Debug.Log("Which was " + (firstRoute - fastestRoute) + " faster than the initial fastest route ");
            }
            string log = "\nAverage weighted distance for each generation \n";

            for (int i = 0; i < averages.Count; i++)
            {
                log += i + ": " + averages[i] + "\n";
            }

            log += "\nShortest weighted distance for each generation [length of each subroute]\n";

            for (int i = 0; i < bests.Count; i++)
            {
                log += i + ": " + bests[i] + "[" + subroutes[i] + "] \n";
            }
            Debug.Log(log);
        }
    }
}

//  static public int[] GetVariables()
//  {
//      //  points, population, generation 
//      int input = 0;
//      int numberPoints = defaultPoints;
//      int numberGenerations = defaultGenerations;
//      int numberPopulation = defaultPopulation;
//      int numberBuses = defaultBuses;
//      int[] vars = new int[] { numberPoints, numberPopulation, numberGenerations, numberBuses };
//      Debug.Log("Enter number of points: Enter 0 for defaults (Points: " + defaultPoints + ", Population: " + defaultPopulation +
//          ", Generations: " + defaultGenerations + ", Buses: " + defaultBuses + ", Selection size: " + selectionSize + ")");
//  
//      //   const int selectionSize = 20;   // takes a System.Random selection of population then finds fittest member of that selection
//      //   const int defaultPoints = 40;   // takes a System.Random selection of population then finds fittest member of that selection
//      //   const int defaultPopulation = 100;   // takes a System.Random selection of population then finds fittest member of that selection
//      //   const int defaultGenerations = 30;   // takes a System.Random selection of population then finds fittest member of that selection
//      //   const int defaultBuses = 5;   // takes 
//  
//      input = int.Parse(Console.ReadLine());
//      if (input == 0)
//      {
//          return vars;
//      }
//      else numberPoints = input;
//  
//      input = 0;  // reset for validation
//  
//      while (input == 0)
//      {
//          Debug.Log("Number of generations:");
//          input = int.Parse(Console.ReadLine());
//      }
//      numberGenerations = input;
//      input = 0;  // reset for validation
//  
//      while (input == 0)
//      {
//          Debug.Log("Population number:");
//          input = int.Parse(Console.ReadLine());
//      }
//  
//      numberPopulation = input;
//      input = 0;  // reset for validation
//  
//      while (input == 0)
//      {
//          Debug.Log("Buses number:");
//          input = int.Parse(Console.ReadLine());
//      }
//  
//      numberBuses = input;
//  
//      vars = new int[] { numberPoints, numberPopulation, numberGenerations, numberBuses };
//  
//      return vars;
//  }

//  static void RunNextGeneration(GeneticRoute[] routes, System.Random r)
//  {
//      GeneticRoute[] temp = CopyRoutes(routes);
//  
//      //  GetNextGeneration(routes, temp, r);
//  
//      for (int i = 0; i < temp.Length; i++)
//      {
//          temp[i].name = null;
//          temp[i].order = null;
//          temp[i].points = null;
//          temp[i] = null;
//      }
//      temp = null;
//  }
//  
//  static GeneticRoute[] CopyRoutes(GeneticRoute[] source)
//  {
//      GeneticRoute[] newRoutes = new GeneticRoute[source.Length];
//      for (int i = 0; i < source.Length; i++)
//      {
//          newRoutes[i] = new GeneticRoute(source[i].points, source[i].order, source[i].name);
//      }
//      return newRoutes;
//  }

// Removed from Solve()
// Get First Generation Information
//   string shortestSubRoutes = ShortestRoute(routes).SubRouteStrings;
//   float currentShortest = ShortestRoute(routes).WeightedDistance;    // shortest from CURRENTGENERATION not the currently fastest generated route
//   float currentAverage = AverageDistance(routes);                 // see above
//   
//   PrintInitialGeneration(routes, currentGeneration, numberPopulation, numberPoints, maxGeneration, currentAverage, currentShortest);
//   
//   float firstRoute = currentShortest; // fastest route from initial System.Random generation (if you can't beat this then the optimisation is not so good)
//   float fastestRoute = firstRoute;         // best route so far
//   string fastestSubRoutes = ShortestRoute(routes).SubRouteStrings;
//   int fastestGeneration = 0;      // generation of best route
//   float[] nGenerationAverages = new float[maxGeneration];   // where n is some division of max generations (ie average of each 100 generations)
//   
//   GeneticRoute fastest = ShortestRoute(routes);
//   
//   // Generation Loop
//   while (currentGeneration < maxGeneration)
//   {
//       averages.Add(currentAverage);
//       best.Add(currentShortest);
//       subroutes.Add(shortestSubRoutes);
//       float oldShortest = currentShortest;
//       float oldAverage = currentAverage;
//   
//       currentGeneration++;
//       RunNextGeneration(routes, r);
//   
//       shortestSubRoutes = ShortestRoute(routes).SubRouteStrings;
//       currentShortest = ShortestRoute(routes).WeightedDistance;
//       currentAverage = AverageDistance(routes);
//   
//       PrintSubsequentGeneration(currentGeneration, numberPopulation, numberPoints, maxGeneration, currentAverage, currentShortest, oldAverage, oldShortest);
//   
//       if (currentShortest < fastestRoute)
//       {
//           fastest = ShortestRoute(routes);
//           fastestSubRoutes = ShortestRoute(routes).SubRouteStrings;
//           fastestRoute = currentShortest;
//           fastestGeneration = currentGeneration;
//       }
//   }

//  PrintFinalResults(firstRoute, fastestRoute, fastestGeneration, fastestSubRoutes, nGenerationAverages, currentShortest, averages, best, subroutes);

//  int.Parse(Console.ReadLine());


//  GeneticRoute GetParent(GeneticRoute[] routes)
//  {
//      GeneticRoute shortest = routes[0];
//      for (int i = 1; i < routes.Length; i++)
//      {
//          if (routes[i].WeightedDistance < shortest.WeightedDistance)
//          {
//              shortest = routes[i];
//          }
//      }
//      return shortest;
//  }
