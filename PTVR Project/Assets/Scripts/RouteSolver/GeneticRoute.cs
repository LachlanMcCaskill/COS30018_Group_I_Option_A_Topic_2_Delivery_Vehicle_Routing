using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    class GeneticRoute
    {
        int generation;
        //public string name;
        public List<int> order;                             //  the chromosome, the routes in the order they are visited
        List<TransportAgentIntroductionMessage> agents;
        public List<DestinationMessage> destinations;
        public Vector3 depotPoint;
        public int trips;

        //  calculated
        List<List<Vector3>> tripRoutes;
        public float distance;
        public List<RoutePlan> routePlan;

        int TotalAgentCapacity
        {
            get
            {
                int totalCapacity = 0;
                for (int i = 0; i < agents.Count; i++)
                {
                    totalCapacity += agents[i].Capacity;
                }
                totalCapacity *= trips;
                return totalCapacity;
            }
        }

        public float WeightedDistance      // the weighted distances I tried seemed less than optimal so just stick with total distance
        {
            get
            {
                //  return WeightedSubRoutes.Sum();
                return TotalDistance;
                //List<float> subRouteLengths = SubrouteDistancesOrdered;
                //for (int i = 0; i < subRouteLengths.Count; i++)
                //{
                //    subRouteLengths[i] /= (i + 1);
                //}
                //return subRouteLengths.Sum();
            }
        }

        public float TotalDistance
        {
            get
            {
                return distance;
            }
        }

        public int RouteLength
        {
            get
            {
                return destinations.Count;
            }
        }

        //  contructor for individuals in the initial generation, the first individuals have their chromosome randomised
        public GeneticRoute(Vector3 _start, List<int> _order, System.Random r, List<TransportAgentIntroductionMessage> agentsWithCapacities,
            int _trips, List<DestinationMessage> _destinations)
        {
            trips = _trips;
            generation = 0;
            depotPoint = _start;
            agents = agentsWithCapacities;
            destinations = _destinations;

            order = new List<int>(_order);

            RandomiseGenes(r);

            Calculate();
        }

        //  Runs the calculations for the individual to determine the route planned by the chromosome
        void Calculate()
        {
            tripRoutes = CalculateRouteLoop();
            distance = CalculateDistance();
            routePlan = GetRoutePlan();
        }

        //  calculates each of the individual trips used in the route
        //  the number of trips can exceed the number of transport agents as agents can take multiple trips
        List<List<Vector3>> CalculateRouteLoop()
        {
            List<List<Vector3>> tripPlan = new List<List<Vector3>>();

            int orderIndex = 0;
            int pendingIndex = 0;

            List<DestinationMessage> pendingSpecial = new List<DestinationMessage>();

            for (int t = 0; t < trips; t++)
            {
                for (int i = 0; i < agents.Count; i++)
                {
                    NewTrip(ref tripPlan, ref pendingSpecial, ref i, ref pendingIndex, ref orderIndex);
                }
            }

            //  if there are still unassigned special destinations waiting they must be assigned
            RoutePending(ref tripPlan, pendingSpecial, pendingIndex);

            return tripPlan;
        }

        //  Creates a new trip by parsing the chromosome into a valid route structure given the additional constraints of special Passengers requiring special transports
        void NewTrip(ref List<List<Vector3>> tripPlan, ref List<DestinationMessage> pendingSpecial, ref int i, ref int pendingIndex, ref int orderIndex)
        {
            bool specialAgent = agents[i].Special;
            List<Vector3> newTrip = new List<Vector3>();
            tripPlan.Add(newTrip);

            for (int j = 0; j < agents[i].Capacity; j++)
            {
                if (orderIndex < order.Count)
                {
                    //  if we are creating a trip for a special transport and there are special passengers set aside we deliver them first
                    if (pendingSpecial.Count > pendingIndex && specialAgent)
                    {
                        newTrip.Add(pendingSpecial[pendingIndex].Position); pendingIndex++;
                    }
                    else
                    {
                        if (!specialAgent)  // if this is a non-special transport filter out any special destination
                        {
                            FindNormalDestination(ref newTrip, ref pendingSpecial, ref orderIndex);
                        }
                        else    // special agents can take any destination
                        {
                            if (order[orderIndex] > -1)    //  skip -1s, theyre empty capacity
                            {
                                Vector3 point = destinations[order[orderIndex]].Position;
                                newTrip.Add(point);
                            }
                            orderIndex++;
                        }
                    }
                }
            }
        }

        //  Filters out sepcial destinations and sets them aside until a special transport is available
        void FindNormalDestination(ref List<Vector3> newTrip, ref List<DestinationMessage> pendingSpecial, ref int orderIndex)
        {
            while (orderIndex < order.Count && order[orderIndex] > -1 && destinations[order[orderIndex]].special)   // cycle through all specials and set them aside until we find a normal or empty destination or have cycled through all destinations
            {
                pendingSpecial.Add(destinations[order[orderIndex]]);
                orderIndex++;
            }

            //  if we have a normal destination we can add it to our normal transports route
            if (orderIndex < order.Count)
            {
                if (order[orderIndex] > -1)    //  skip -1s, theyre empty capacity
                {
                    Vector3 point = destinations[order[orderIndex]].Position;
                    newTrip.Add(point);
                }
                orderIndex++;
            }
        }

        //  assigns any left over special destinations
        void RoutePending(ref List<List<Vector3>> tripPlan, List<DestinationMessage> pendingSpecial, int pendingIndex)
        {
            int runs = 2;   //  2 runs seems to be necessary to explore all options
                            //  otherwise loop while the number of special destinations pending exceeds the number of pending destinations assigined (pendingIndex)
            while (pendingSpecial.Count > pendingIndex && runs > 0)
            {
                runs--;
                for (int t = 0; t < trips; t++)
                {
                    for (int i = 0; i < agents.Count; i++)
                    {
                        if (agents[i].Special)
                        {
                            int tripNumber = t * agents.Count + i;
                            //  if we have a special transport
                            //  and that special transport has fewer than its capacity in its current trip
                            //  assign one of the pending special destinations to this trip
                            for (int j = tripPlan[tripNumber].Count; j < agents[i].Capacity; j++)
                            {
                                if (pendingSpecial.Count > pendingIndex)
                                {
                                    tripPlan[tripNumber].Add(pendingSpecial[pendingIndex].Position);
                                    pendingIndex++;
                                }
                                else return;
                            }
                        }
                    }
                }
            }

            //  if we run the above check and there are still special destinations waiting this will just create a new trip for them
            //  this is not at all optimal but allows the individual to be tested properly and discarded if it (probably) proves unfit

            int check = 0;  // compare to pendingIndex to see if there are new assignments
            if (pendingSpecial.Count > pendingIndex && check != pendingIndex)
            {
                check = pendingIndex;

                for (int t = 0; t < trips; t++)
                {
                    for (int i = 0; i < agents.Count; i++)
                    {
                        if (agents[i].Special)
                        {
                            int tripNumber = t * agents.Count + i;
                            tripPlan[tripNumber].Add(depotPoint);
                            for (int j = 0; j < agents[i].Capacity; j++)
                            {
                                if (pendingSpecial.Count > pendingIndex)
                                {
                                    tripPlan[tripNumber].Add(pendingSpecial[pendingIndex].Position);
                                    pendingIndex++;
                                }
                                else return;
                            }
                        }
                    }
                }
            }

            if (pendingSpecial.Count > pendingIndex)
            {
                Debug.LogError("pending not routed. Generation: " + generation);
            }
        }

        //  find the total distance of the individuals route by summing the distance between each of the destinations in order
        public float CalculateDistance()
        {
            distance = 0;
            List<List<Vector3>> routes = GetSubroutes();

            for (int i = 0; i < routes.Count; i++)
            {
                Vector3 start = depotPoint;                     //  each routes starts at the depot

                int destinationCount = routes[i].Count;
                for (int j = 0; j < destinationCount; j++)
                {
                    Vector3 next = routes[i][j];                //  find the next point
                    distance += Vector3.Distance(start, next);  //  calculate distance from the start point to the next
                    start = next;                               //  the next point is the start for the next cycle
                }
                distance += Vector3.Distance(start, depotPoint);//  the route must also end at the depot
            }

            return distance;
        }

        //  creates the routes in List<Vector3> form
        //  takes each of the trips in the tripRoutes and assigns them to a subroute
        //  the subroute represents a all the trips taken by a single transport agent
        public List<List<Vector3>> GetSubroutes()
        {
            List<List<Vector3>> subRoutes = new List<List<Vector3>>();
            for (int i = 0; i < agents.Count; i++)
            {
                subRoutes.Add(tripRoutes[i]);
            }

            for (int i = agents.Count; i < tripRoutes.Count; i++)
            {
                subRoutes[i % agents.Count].Add(depotPoint);
                subRoutes[i % agents.Count].AddRange(tripRoutes[i]);
            }

            return subRoutes;
        }

        //  creates the routes in RoutePlan form
        //  using the tripRoutes calculated above
        //  trips are assigned to transport agents until all are assigned
        public List<RoutePlan> GetRoutePlan()
        {
            List<RoutePlan> routePlan = new List<RoutePlan>();
            for (int i = 0; i < agents.Count; i++)
            {
                routePlan.Add(new RoutePlan());
            }

            for (int i = 0; i < agents.Count; i++)
            {
                mergeTrips(routePlan[i % agents.Count], tripRoutes[i]);
            }

            return routePlan;
        }

        //  translates vector lists into stack for the routeplan
        public RoutePlan mergeTrips(RoutePlan route, List<Vector3> appending)
        {
            route.Destinations.Push(depotPoint);
            for (int i = 0; i < appending.Count; i++)
            {
                route.Destinations.Push(appending[i]);
            }
            return route;
        }

        //  used to randomise the initial individuals
        void RandomiseGenes(System.Random r)
        {
            for (int i = 0; i < 50; i++)    // randomise initial routes
            {
                order = Mutate(r);
            }
        }

        //  re-inserts genes into different locations on the chromosome to prevent stagnation
        List<int> Mutate(System.Random r)
        {
            int maxMutations = Math.Max(order.Count / 8, 1); // mutations scale to the size of the chromosome
            int numberMutations = r.Next(0, maxMutations + 1);

            for (int i = 0; i < numberMutations; i++)
            {
                //  takes a gene and inserts it into a new position
                int gene = r.Next(0, order.Count - 1);
                int location = r.Next(0, order.Count - 2);

                int geneValue = order[gene];

                int temp = geneValue;
                order.Remove(geneValue);
                order.Insert(location, geneValue);
            }
            return order;
        }

        //  Constructor that creates an individual through recombination of two parent individuals of the previous generation
        //  uses ordered crossover to combine genes
        public GeneticRoute(GeneticRoute mother, GeneticRoute father, System.Random _r)
        {
            depotPoint = mother.depotPoint;
            trips = mother.trips;
            generation = mother.generation + 1;
            agents = mother.agents;
            destinations = mother.destinations;

            //  base order uses mothers chromosome
            order = new List<int>(mother.order);

            //  segment is taken from father's chromosome
            List<int> segment = CreateSegment(father, _r);
            int insertIndex = segment[0];   // index 0 holds the index location the segment starts from
            segment.RemoveAt(0);

            OverwriteGenesFromSegment(segment);

            InsertSegment(insertIndex, segment);

            IncorporateTrailingGenes();

            //  PrintBreeding(mother, father, insertIndex, insertIndex + segment.Count, "Segment Logs Placeholder");

            Mutate(_r);

            CheckForDuplicateGenes();
            CheckForMissingGenes();

            Calculate();
        }

        //  Creates the father segment
        List<int> CreateSegment(GeneticRoute father, System.Random r)
        {
            int gene1 = r.Next(0, order.Count - 1);
            int gene2 = r.Next(0, order.Count - 1);
            int startGene = Math.Min(gene1, gene2);
            int endGene = Math.Max(gene1, gene2);

            List<int> segment = new List<int>();

            segment.Add(startGene); //  segment[0] is the index

            for (int i = startGene; i < endGene; i++)
            {
                segment.Add(father.order[i]);
            }

            return segment;
        }

        // genes in the base chromosome that are in the segment overwritten with -1
        void OverwriteGenesFromSegment(List<int> segment)
        {
            for (int i = 0; i < order.Count; i++)
            {
                if (segment.Contains(order[i]))
                {
                    order[i] = -1;
                }
            }
        }

        //  segment is inserted into base chromosome
        void InsertSegment(int insertIndex, List<int> segment)
        {
            for (int i = 0; i < segment.Count; i++)
            {
                int index = insertIndex + i;
                order.Insert(index, segment[i]);
            }
        }

        //  the chromosome must remain the same length
        //  any genes that are on a point above this length need to be re-incorporated into the chromosome
        void IncorporateTrailingGenes()
        {
            List<int> trailing = new List<int>();

            for (int i = TotalAgentCapacity; i < order.Count; i++)
            {
                trailing.Add(order[i]);
            }

            order.RemoveRange(TotalAgentCapacity, trailing.Count);
            //  there are now two lists
            //  a chromosome of the correct length with some missing genes
            //  a list of trailing genes that will need to be re-inserted

            int j = 0;
            int max = 10 * order.Count;  // hard limit on the while loopp
                                         // just a precaution but it is inelegant

            for (int i = 0; i < trailing.Count; i++)
            {
                if (trailing[i] != -1)
                {
                    while (order[j % order.Count] != -1 && j < max)
                    {
                        j++;
                    }

                    if (j < max)
                    {
                        order[j % order.Count] = trailing[i];
                    }
                    else
                    {
                        Debug.LogError("Could not incorporate trailing genes");
                    }
                }
            }
        }

        void CheckForMissingGenes()
        {
            for (int i = 0; i < destinations.Count; i++)
            {
                if (!order.Contains(i))
                {
                    Debug.LogError("Trailing: Chromosome is missing a gene: " + i + " in generation: " + generation);
                }
            }
        }

        void CheckForDuplicateGenes()
        {
            for (int i = 0; i < order.Count; i++)
            {
                for (int j = i + 1; j < order.Count; j++)
                {
                    if (order[i] == order[j] && order[i] != -1)
                    {
                        Debug.LogError("Duplicate gene found " + i + " = " + j);
                    }
                }
            }
        }

        void PrintBreeding(GeneticRoute mother, GeneticRoute father, int startGene, int endGene, string segmentLog)
        {
            string log = "Generation " + generation + " Breeding: Mother, Father, Child \n";
            log += mother.GetGeneString();

            log += father.GetGeneString().Split('\n')[0] + "\n";

            for (int i = 0; i < order.Count; i++)
            {
                if (i < startGene)
                {
                    log += "   ";
                }
                else if (i <= endGene)
                {
                    if (father.order[i] >= RouteLength)
                    {
                        log += 0 + "  ";
                    }
                    else
                    {
                        log += father.order[i] + " ";
                        if (father.order[i] < 10)
                        {
                            log += " ";
                        }
                    }
                }
                else
                {
                    log += "   ";
                }
            }
            log += "\n";

            log += GetGeneString();
            CheckForDuplicateGenes();
            CheckForMissingGenes();
            //if (pointString.Length > 0)
            //{
            //    log += pointString + " Printing Segment Log\n ";
            //    log += segmentLog;
            //}
            Debug.LogWarning(log);  // put it here so it doesn't clog things up
        }

        //  used to reverse the stack to maintain the correct order of routes
        private Stack<T> Reverse<T>(Stack<T> stack)
        {
            Stack<T> reversed = new Stack<T>();
            while (stack.Count > 0) reversed.Push(stack.Pop());
            return reversed;
        }

        public void PrintGenes()
        {
            Debug.Log(GetGeneString());
        }

        public string GetGeneString()
        {
            string geneString = "Weighted Distance: " + (int)WeightedDistance + "\n";
            foreach (int g in order)
            {
                geneString += g + " ";
                if (g < 10 && g > -1)
                {
                    geneString += " "; // want spaces between genes the same, doesn't matter in unity though
                }
            }
            geneString += "\n";

            return geneString;
        }
    }
}