using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    class ClusterGeneration
    {
        const int selectionSize = 20;   // takes a System.Random selection of population then finds fittest member of that selection
        //  const int defaultPoints = 20;   // takes a System.Random selection of population then finds fittest member of that selection
        const int populationSize = 100;   // takes a System.Random selection of population then finds fittest member of that selection
        //  const int defaultGenerations = 25;   // takes a System.Random selection of population then finds fittest member of that selection
        //  const int defaultBuses = 4;   // takes a System.Random selection of population then finds fittest member of that selection

        System.Random r;
        int max = 20;
        public int number;
        List<ClusterGeneticRoute> population;
        List<Vector3> points;
        List<TransportAgentIntroductionMessage> agents;

        public List<RoutePlan> GetRoutePlan()
        {
            return ShortestRoute().GetRoutePlan();
        }

        //  Initial
        public List<ClusterGeneticRoute> PopulateGeneration(List<int> initialOrder, List<Vector3> _points, Vector3 start, int trips)
        {
            population = new List<ClusterGeneticRoute>();
            for (int i = 0; i < populationSize; i++)
            {
                //  Debug.Log("initial order " + initialOrder.Count);
                ClusterGeneticRoute newRoute = new ClusterGeneticRoute(start, _points, initialOrder, r, agents, trips);
                population.Add(newRoute);
            }
            return population;
        }

        public ClusterGeneration(Vector3 start, List<Vector3> _points, List<TransportAgentIntroductionMessage> agentsWithCapacities, System.Random _r, int trips)
        {
            r = _r;
            points = _points;
            agents = agentsWithCapacities;

            List<int> initialOrder = CreateOrder(points.Count, agents, trips); // the initial order is just a list the length of the total capacity of all agents, containing the order of points from first to last point and then the excess space filled with 0s to represent empty spots on an agent
            population = PopulateGeneration(initialOrder, _points, start, trips);
        }

        public ClusterGeneration(ClusterGeneration parent)
        {
            r = parent.r;
            points = parent.points;
            max = parent.max;
            number = parent.number + 1;
            agents = parent.agents;

            GetNextGeneration(parent);
        }

        void GetNextGeneration(ClusterGeneration parent)
        {
            population = new List<ClusterGeneticRoute>();
            for (int i = 0; i < populationSize; i++)
            {
                ClusterGeneticRoute[] parents = parent.GetParents();
                population.Add(new ClusterGeneticRoute(points, parents[0], parents[1], r, agents, parent.population[0].trips));
            }
        }

        public void Print()
        {
            string log = "Generation " + number + " (Population: " + population.Count + ", Points " + points.Count + ", Generations: " + max +
                ")\n"
                + "Shortest Route: " + (int)ShortestRoute().WeightedDistance + "\n"
                +  "Average Distance: " + (int)AverageDistance()
                 + "\n";

            if (number > 0) ;  // add some more info regarding comparison to previous generations

            for (int i = 0; i < population.Count; i++)
            {
                log += "Route " + i + " ";
                log += population[i].GetGeneString();
            }

            Debug.Log(log);
        }

        //  this is the fitness test, these are the important parts (assuming the rest works)
        ClusterGeneticRoute[] GetParents()
        {
            int size = Math.Min(selectionSize, population.Count / 2);
            int[][] selection = ParentSelection(size, r);

            ClusterGeneticRoute mother = GetFittestRouteFromRandomSelection(population, selection[0]);
            ClusterGeneticRoute father = GetFittestRouteFromRandomSelection(population, selection[1]); ;

            return new ClusterGeneticRoute[] { mother, father };
        }

        int[][] ParentSelection(int size, System.Random r)
        {
            int select;
            int[][] parentArray = new int[2][];
            parentArray[0] = new int[size/2];
            parentArray[1] = new int[size/2];

            for (int i = 0; i < parentArray.Length; i++)
            {
                for (int j = 0; j < parentArray[i].Length; j++)
                {
                    select = r.Next(0, population.Count);  // pick a random route from the population

                    int firstSelect = select;
                    while (parentArray[0].Contains(select) || parentArray[1].Contains(select))  // if we have already selected a route then cycle through till we reach a route we havent selected
                    {
                        select = (select + 1) % population.Count;

                        if (select == firstSelect)  // if these are equal we have cycled through the entire population and all routes have been selected
                        {
                            Debug.Log("Population Selection is stuck in a loop, check the selection size isn't larger than the population");
                            return null;
                        }
                    }
                    parentArray[i][j] = select;
                }
            }
            return parentArray;
        }

        // find the fittest route by comparing weighteddistance
        ClusterGeneticRoute GetFittestRouteFromRandomSelection(List<ClusterGeneticRoute> selectedPopulation, int[] selection)
        {
            ClusterGeneticRoute fittest = selectedPopulation[selection[0]];

            for (int i = 1; i < selection.Length; i++)
            {
                if (selectedPopulation[selection[i]].WeightedDistance < fittest.WeightedDistance)
                {
                    fittest = selectedPopulation[selection[i]];
                }
            }
            return fittest;
        }
        // End fitness test

        float AverageDistance()
        {
            float distance = 0;
            foreach (ClusterGeneticRoute r in population)
            { distance += r.WeightedDistance; }
            return distance / population.Count;
        }

        float ShortestDistance
        {
            get
            {
                ClusterGeneticRoute shortest = population[0];
                for (int i = 1; i < population.Count; i++)
                {
                    if (population[i].WeightedDistance < shortest.WeightedDistance)
                    {
                        shortest = population[i];
                    }
                }
                return shortest.WeightedDistance;
            }
        }

        public ClusterGeneticRoute ShortestRoute()
        {
            ClusterGeneticRoute shortestRoute = population[0];
            for (int i = 1; i < population.Count; i++)
            {
                if (population[i].WeightedDistance < shortestRoute.WeightedDistance)
                {
                    shortestRoute = population[i];
                }
            }
            return shortestRoute;
        }

        List<int> CreateOrder(int pointCount, List<TransportAgentIntroductionMessage> agents, int trips)
        {
            // just generates an array of sequential ints (this is basically the chromosome)
            List<int> order = new List<int>();

            int chromosomeLength = 0;

            //debug.log("agents.count " + agents.count);
            for (int j = 0; j < trips; j++)
            {
                for (int i = 0; i < agents.Count; i++)
                {
                    chromosomeLength += agents[i].Capacity;
                    //Debug.Log("agents[i].Capacity " + agents[i].Capacity);
                }
            }

            for (int i = 0; i < chromosomeLength; i++)
            {
                if (i < pointCount)
                {
                    order.Add(i);
                }
                else order.Add(-1);  // add -1s to fill empty capacity
            }

            return order;
        }
    }
}