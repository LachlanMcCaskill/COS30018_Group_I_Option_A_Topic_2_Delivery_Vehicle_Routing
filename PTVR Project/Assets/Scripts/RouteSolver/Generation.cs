using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    class Generation
    {
        const int selectionSize = 10;   //   determines the size of the group used to select parents
        const int populationSize = 100;   
        System.Random r;
        public int number;              //  generation number
        List<GeneticRoute> population;
        List<DestinationMessage> destinations;
        List<TransportAgentIntroductionMessage> agents;

        //  Initial Generation constructor
        public Generation(Vector3 start, List<TransportAgentIntroductionMessage> agentsWithCapacities, System.Random _r, int trips, List<DestinationMessage> _destinations)
        {
            r = _r;
            destinations = _destinations;
            agents = agentsWithCapacities;

            List<int> initialOrder = CreateOrder(destinations.Count, agents, trips);
            specialArray = CreateSpecialArray(trips);

            population = PopulateGeneration(initialOrder, start, trips);
        }

        //  Create an initial population of randomised individuals
        public List<GeneticRoute> PopulateGeneration(List<int> initialOrder, Vector3 start, int trips)
        {
            population = new List<GeneticRoute>();
            for (int i = 0; i < populationSize; i++)
            {
                GeneticRoute newRoute = new GeneticRoute(start, initialOrder, r, agents, trips, destinations);
                population.Add(newRoute);
            }
            return population;
        }

        //  Constructor for subsequent generations
        public Generation(Generation parent)
        {
            r = parent.r;
            destinations = parent.destinations;
            max = parent.max;
            number = parent.number + 1;
            agents = parent.agents;

            GetNextGeneration(parent);
        }

        //  Populates the next generation using recombination with parents from this generation 
        void GetNextGeneration(Generation parent)
        {
            population = new List<GeneticRoute>();
            for (int i = 0; i < populationSize; i++)
            {
                GeneticRoute[] parents = parent.GetParents();
                population.Add(new GeneticRoute(parents[0], parents[1], r));
            }
        }

        //  Parents are selected by taking the fittest individual from a random selection from the population
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
                    while (parentArray[0].Contains(select) || parentArray[1].Contains(select))  // if we have already selected that route then cycle through till we reach a route we havent selected
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

        // find the fittest route from the random selection by comparing weighted distance (which is just total distance)
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

        //  average distance of the population
        float AverageDistance()
        {
            float distance = 0;
            foreach (GeneticRoute r in population)
            { distance += r.WeightedDistance; }
            return distance / population.Count;
        }

        //  distance of shortest route in population
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

        public List<RoutePlan> GetRoutePlan()
        {
            return ShortestRoute().routePlan;
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

        //  Creates the initial chromosome with all the required genes/destinations
        //  The chromosome length is determined by total agent capacity and the number of trips necessary for the agents to carry every passenger.
        List<int> CreateOrder(int pointCount, List<TransportAgentIntroductionMessage> agents, int trips)
        {
            // just generates an array of sequential ints (this is basically the chromosome)
            List<int> order = new List<int>();

            int chromosomeLength = 0;

            for (int i = 0; i < agents.Count; i++)
            {
                chromosomeLength += agents[i].Capacity;
            }

            chromosomeLength *= trips;

            for (int i = 0; i < chromosomeLength; i++)
            {
                if (i < pointCount)
                {
                    order.Add(i);    //  add each destination
                }
                else order.Add(-1);  //  add -1s to fill empty capacity
            }

            return order;
        }

        public void Print()
        {
            string log = "Generation " + number + " (Population: " + population.Count + ", Points " + destinations.Count + ", Generations: " + max +
                ")\n"
                + "Shortest Route: " + (int)ShortestRoute().WeightedDistance + "\n"
                + "Average Distance: " + (int)AverageDistance()
                 + "\n";

            if (number > 0) ;  // add some more info regarding comparison to previous generations

            for (int i = 0; i < population.Count; i++)
            {
                log += "Route " + i + " ";
                log += population[i].GetGeneString();
            }

            Debug.Log(log);
        }
    }
}