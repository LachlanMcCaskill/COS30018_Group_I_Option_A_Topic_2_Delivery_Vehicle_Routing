using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    class Generation
    {
        const int selectionSize = 20;   // takes a System.Random selection of population then finds fittest member of that selection
        //  const int defaultPoints = 20;   // takes a System.Random selection of population then finds fittest member of that selection
        const int populationSize = 100;   // takes a System.Random selection of population then finds fittest member of that selection
        //  const int defaultGenerations = 25;   // takes a System.Random selection of population then finds fittest member of that selection
        //  const int defaultBuses = 4;   // takes a System.Random selection of population then finds fittest member of that selection

        System.Random r;
        int max = 1;
        public int number;
        List<GeneticRoute> population;
        List<Vector3> points;
        List<TransportAgentIntroductionMessage> agents;
        //  int trips;

        public List<RoutePlan> GetRoutePlan()
        {
            return ShortestRoute().GetRoutePlan();
        }

        //  Initial
        public List<GeneticRoute> PopulateGeneration(List<int> initialOrder, List<Vector3> _points, Vector3 start, int trips)
        {
            //  trips = _trips;
            population = new List<GeneticRoute>();
            for (int i = 0; i < populationSize; i++)
            {
                //  Debug.Log("initial order " + initialOrder.Count);
                GeneticRoute newRoute = new GeneticRoute(start, _points, initialOrder, r, agents, trips);
                population.Add(newRoute);
            }
            return population;
        }

        public Generation(Vector3 start, List<Vector3> _points, List<TransportAgentIntroductionMessage> agentsWithCapacities, System.Random _r, int trips)
        {
            r = _r;
            points = _points;
            agents = agentsWithCapacities;

            List<int> initialOrder = CreateOrder(points.Count, agents, trips); // the initial order is just a list the length of the total capacity of all agents, containing the order of points from first to last point and then the excess space filled with 0s to represent empty spots on an agent
            population = PopulateGeneration(initialOrder, _points, start, trips);
        }

        public Generation(Generation parent)
        {
            r = parent.r;
            points = parent.points;
            max = parent.max;
            number = parent.number + 1;
            agents = parent.agents;

            GetNextGeneration(parent);
        }

        void GetNextGeneration(Generation parent)
        {
            population = new List<GeneticRoute>();
            for (int i = 0; i < populationSize; i++)
            {
                GeneticRoute[] parents = parent.GetParents();
                population.Add(new GeneticRoute(parents[0], parents[1], r));
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
        GeneticRoute[] GetParents()
        {
            int size = Math.Min(selectionSize, population.Count / 2);
            int[][] selection = ParentSelection(size, r);

            GeneticRoute mother = GetFittestRouteFromRandomSelection(population, selection[0]);
            GeneticRoute father = GetFittestRouteFromRandomSelection(population, selection[1]); ;

            return new GeneticRoute[] { mother, father };
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
        GeneticRoute GetFittestRouteFromRandomSelection(List<GeneticRoute> selectedPopulation, int[] selection)
        {
            GeneticRoute fittest = selectedPopulation[selection[0]];

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
            foreach (GeneticRoute r in population)
            { distance += r.WeightedDistance; }
            return distance / population.Count;
        }

        float ShortestDistance
        {
            get
            {
                GeneticRoute shortest = population[0];
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

        public GeneticRoute ShortestRoute()
        {
            GeneticRoute shortestRoute = population[0];
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
            for (int i = 0; i < agents.Count; i++)
            {
                chromosomeLength += agents[i].Capacity;
                //Debug.Log("agents[i].Capacity " + agents[i].Capacity);
            }

            chromosomeLength *= trips;

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