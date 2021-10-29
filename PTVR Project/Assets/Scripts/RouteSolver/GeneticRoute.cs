using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    class GeneticRoute
    {
        List<TransportAgentIntroductionMessage> agents;

        int generation;
        public string name;
        public List<int> order;
        public List<DestinationMessage> destinations;
        public Vector3 depotPoint;
        public int trips;

        //  calculated
        List<List<Vector3>> tripRoutes;
        public float distance;
        public List<RoutePlan> routePlan;
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
                //return points.Count;
            }
        }

        public GeneticRoute(Vector3 _start, List<int> _order, System.Random r, List<TransportAgentIntroductionMessage> agentsWithCapacities,
            int _trips, List<DestinationMessage> _destinations)
        {
            trips = _trips;
            generation = 0;
            depotPoint = _start;
            agents = agentsWithCapacities;
            destinations = _destinations;
            //  points = _points;

            order = new List<int>(_order);

            string orderString = "Trips: " + trips + "\nOrder: ";
            for (int i = 0; i < order.Count; i++)
            {
                orderString += order[i] + ", ";
            }
            Debug.Log(orderString);

            RandomiseGenes(r);

            Calculate();
        }

        void Calculate()
        {
            tripRoutes = CalculateRouteLoop();
            distance = CalculateDistance();
            routePlan = GetRoutePlan();
        }

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

        public float CalculateDistance()
        {
            distance = 0;
            List<List<Vector3>> routes = GetSubroutes();
            for (int i = 0; i < routes.Count; i++)
            {
                Vector3 start = depotPoint;   // need to add the proper start coordinates to this later

                int destinationCount = routes[i].Count;
                for (int j = 0; j < destinationCount; j++)
                {
                    Vector3 next = routes[i][j];// routes[i][j];
                    distance += Vector3.Distance(start, next);
                    start = next;
                }
                distance += Vector3.Distance(start, depotPoint);
            }

            return distance;
        }

        void RandomiseGenes(System.Random r)
        {
            for (int i = 0; i < 50; i++)    // randomise initial routes
            {
                order = Mutate(r);
            }
        }

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

        // genes in the code that are in the segment overwritten with -1
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

        void InsertSegment(int insertIndex, List<int> segment)
        {
            for (int i = 0; i < segment.Count; i++)
            {
                int index = insertIndex + i;
                order.Insert(index, segment[i]);
            }
        }

        void IncorporateTrailingGenes()
        {
            List<int> trailing = new List<int>();

            for (int i = TotalAgentCapacity; i < order.Count; i++)
            {
                trailing.Add(order[i]);
            }

            order.RemoveRange(TotalAgentCapacity, trailing.Count);

            int j = 0;

            for (int i = 0; i < trailing.Count; i++)
            {
                if (trailing[i] != -1)
                {
                    while (order[j % order.Count] != -1 && j < 99)
                    {
                        j++;
                    }
                    if (j < 99)
                    {
                        order[j % order.Count] = trailing[i];
                    }
                }
            }
        }

        //    string log = "Generation " + generation +
        //        "\nOrder Before: ";
        //        for (int k = 0; k<order.Count; k++)
        //        {
        //            log += (order[k] + " ");
        //        }
        //log += "\nOrder Cut: ";
        //        for (int k = 0; k<order.Count; k++)
        //        {
        //            log += (order[k] + " ");
        //        }

        // create route from parents
        public GeneticRoute(GeneticRoute mother, GeneticRoute father, System.Random _r)
        {
            // this is the constructor for a child
            // ordered crossover https://towardsdatascience.com/evolution-of-a-salesman-a-complete-genetic-algorithm-tutorial-for-python-6fe5d2b3ca35
            depotPoint = mother.depotPoint;
            trips = mother.trips;
            generation = mother.generation + 1;
            agents = mother.agents;
            destinations = mother.destinations;
            //  points = mother.points;
            order = new List<int>(mother.order);
            List<int> segment = CreateSegment(father, _r);
            int insertIndex = segment[0];
            segment.RemoveAt(0);

            Mutate(_r);
            PrintBreeding(mother, father, insertIndex, insertIndex + segment.Count, "Segment Logs Placeholder");


            OverwriteGenesFromSegment(segment);

            InsertSegment(insertIndex, segment);

            IncorporateTrailingGenes();

            Calculate();
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
            string pointString = CheckForMissingGenes();
            if (pointString.Length > 0)
            {
                log += pointString + " Printing Segment Log\n ";
                log += segmentLog;
            }
            Debug.LogWarning(log);  // put it here so it doesn't clog things up
        }

        string CheckForMissingGenes()
        {
            for (int i = 0; i < destinations.Count; i++)
            {
                if (!order.Contains(i))
                {
                    Debug.LogError("Trailing: Chromosome is missing a gene: " + i + " in generation: " + generation);
                    return "Chromosome is missing a gene: " + i;
                }
            }
            return "";
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

        private Stack<T> Reverse<T>(Stack<T> stack)
        {
            Stack<T> reversed = new Stack<T>();
            while (stack.Count > 0) reversed.Push(stack.Pop());
            return reversed;
        }

        // strings
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

        List<List<Vector3>> CalculateRouteLoop()
        {
            List<List<Vector3>> tripPlan = new List<List<Vector3>>();

            int k = 0;
            int p = 0;

            List<DestinationMessage> pendingSpecial = new List<DestinationMessage>();

            for (int t = 0; t < trips; t++)
            {
                for (int i = 0; i < agents.Count; i++)
                {
                    NewTrip(ref tripPlan, ref pendingSpecial, ref i, ref p, ref k);
                }
            }

            RoutePending(ref tripPlan, pendingSpecial, p);

            return tripPlan;
        }

        void RoutePending(ref List<List<Vector3>> tripPlan, List<DestinationMessage> pendingSpecial, int p)
        {
            //int count = 0;
            int runs = 2;
            while (pendingSpecial.Count > p && runs > 0)
            {
                runs--;
                //Debug.LogError("number of special reassignment runs: " + runs);
                for (int t = 0; t < trips; t++)
                {
                    for (int i = 0; i < agents.Count; i++)
                    {
                        if (agents[i].Special)
                        {
                            int tripNumber = t * agents.Count + i;
                            for (int j = tripPlan[tripNumber].Count; j < agents[i].Capacity; j++)
                            {
                                if (pendingSpecial.Count > p)
                                {
                                    tripPlan[tripNumber].Add(pendingSpecial[p].Position);
                                    p++;
                                }
                                else return;
                            }
                        }
                    }
                }
            }

            int check = 0;  // once there are no new assignments
            // just make a new route
            if (pendingSpecial.Count > p && check != p)
            {
                check = p;
                //Debug.LogError("Making a  new route from unvisited special destinations.");
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
                                if (pendingSpecial.Count > p)
                                {
                                    tripPlan[tripNumber].Add(pendingSpecial[p].Position);
                                    p++;
                                }
                                else return;
                            }
                        }
                    }
                }
            }

            if (pendingSpecial.Count > p)
            {                
                Debug.LogError("pending not routed. Generation: " + generation);
            }
        }

        void NewTrip(ref List<List<Vector3>> tripPlan, ref List<DestinationMessage> pendingSpecial, ref int i, ref int p, ref int k)
        {
            bool specialAgent = agents[i].Special;
            List<Vector3> newTrip = new List<Vector3>();
            tripPlan.Add(newTrip);

            for (int j = 0; j < agents[i].Capacity; j++)
            {
                if (k < order.Count)
                {
                    if (pendingSpecial.Count > p && specialAgent)   // is there a pending location?
                    {
                        newTrip.Add(pendingSpecial[p].Position);    p++;
                    }
                    else
                    {
                        if (!specialAgent)  // cycle till we find a non-special destination
                        {
                            FindNormalDestination(ref newTrip, ref pendingSpecial, ref k);
                        }
                        else    // special agents can take any destination
                        {
                            if (order[k] > -1)    //  skip -1s, theyre empty capacity
                            {
                                Vector3 point = destinations[order[k]].Position;
                                newTrip.Add(point);
                            }
                            k++;
                        }
                    }
                }
            }
        }

        void FindNormalDestination(ref List<Vector3> newTrip, ref List<DestinationMessage> pendingSpecial, ref int k)
        {
            while (k < order.Count && order[k] > -1 && destinations[order[k]].special)   // cycle through all specials and set them aside
            {
                pendingSpecial.Add(destinations[order[k]]);
                k++;
            }
            if (k < order.Count)
            {
                if (order[k] > -1)    //  skip -1s, theyre empty capacity
                {
                    Debug.Log("order.Count = " + order.Count + ",    k = " + k + "\n destinations.Count = " + destinations.Count + ",    order[k] = " + order[k]);
                    Vector3 point = destinations[order[k]].Position;
                    newTrip.Add(point);
                }
                k++;
            }
        }

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

        public RoutePlan mergeTrips(RoutePlan route, List<Vector3> appending)
        {
            route.Destinations.Push(depotPoint);
            for (int i = 0; i < appending.Count; i++)
            {
                route.Destinations.Push(appending[i]);
            }
            return route;
        }


        //    List<List<Vector3>> subRoutes = new List<List<Vector3>>();

        //    int k = 0;

        //    for (int i = 0; i < agents.Count; i++)
        //    {
        //        List<Vector3> newSubRoute = new List<Vector3>();
        //        for (int t = 0; t < trips; t++)
        //        {
        //            if (t != 0)
        //            {
        //                newSubRoute.Add(new Vector3(51, 0, 51));
        //            }
        //            for (int j = 0; j < agents[i].Capacity; j++)
        //            {
        //                if (k < order.Count)
        //                {
        //                    int pointIndex = order[k];
        //                    k++;
        //                    if (pointIndex > -1)    //  skip -1s, theyre empty capacity
        //                    {
        //                        Vector3 point = destinations[pointIndex].Position;
        //                        newSubRoute.Add(point);
        //                    }
        //                }
        //            }
        //        }
        //        subRoutes.Add(newSubRoute);
        //    }

        //    return subRoutes;
        //}

        //public List<float> SubrouteDistancesOrdered
        //{
        //    get
        //    {
        //        List<float> subRouteDistances = new List<float>();
        //        List<List<Vector3>> routes = GetSubroutes();
        //        for (int i = 0; i < routes.Count; i++)
        //        {
        //            float subDistance = 0;
        //            Vector3 start = depotPoint;   // need to add the proper start coordinates to this later

        //            for (int j = 0; j < routes[i].Count; j++)
        //            {
        //                Vector3 next = routes[i][j];
        //                subDistance += Vector3.Distance(start, next);
        //                start = next;
        //            }
        //            subDistance += Vector3.Distance(start, depotPoint);
        //            subRouteDistances.Add(subDistance);
        //        }

        //        subRouteDistances.Sort();       // sort from longest to shortest
        //        subRouteDistances.Reverse();    // this is used for some weighted distance calculations

        //        return subRouteDistances;
        //    }
        //}


        public float WeightedDistance      // the weighted distances I tried seemed less than optimal so just stick with total distance
        {
            get
            {
                //  return WeightedSubRoutes.Sum();
                return distance;
                //List<float> subRouteLengths = SubrouteDistancesOrdered;
                //for (int i = 0; i < subRouteLengths.Count; i++)
                //{
                //    subRouteLengths[i] /= (i + 1);
                //}
                //return subRouteLengths.Sum();
            }
        }
    }
}

//Stack<Vector3> GetSubroute(int agentNumber)
//{
//    Stack<Vector3> subRoute = new Stack<Vector3>();
//    int routeNumber = 0;
//    for (int i = 0; i < agentNumber + 1; i++)
//    {
//        routeNumber += agents[i].Capacity;
//    }
//    return subRoute;
//}


//public string SubRouteStrings
//{
//    get
//    {
//        string routeStrings = "";
//        List<float> subRouteLengths = SubRouteDistance;

//        for (int i = 0; i < subRouteLengths.Count; i++)
//        {
//            routeStrings += subRouteLengths[i] + ", ";
//        }

//        if (routeStrings.Length > 3)
//        {
//            routeStrings = routeStrings.Substring(0, routeStrings.Length - 2);
//        }
//        return routeStrings;
//    }
//}

//public List<float> SubRouteDistance
//{
//    get
//    {
//        return new List<float>();
//        float distance = 0;
//        float nextPointDistance;
//        //float currentLongestSubroute = 0;
//        //float totalDistance = 0;
//        List<float> subRouteLengths = new List<float>();

//        Vector3 start = new Vector3(0, 0, 0);
//        Vector3 end = new Vector3(0, 0, 0);

//        for (int i = 0; i < order.Count; i++)
//        {
//            if (i % agents[0].Capacity == 0)    // is a new Subroute

//            if (order[i] < RouteLength)    // is not a bus index
//            {
//                end = points[order[i]];
//            }
//            else                           // is a bus index so go back to depot
//            {
//                if (subRouteLengths.Count == 0)
//                {
//                    subRouteLengths.Add(distance);
//                }
//                else
//                {
//                    for (int j = 0; j < subRouteLengths.Count; j++)
//                    {
//                        if (distance > subRouteLengths[j])
//                        {
//                            subRouteLengths.Insert(j, distance);
//                            goto LoopEnd;
//                        }
//                    }
//                    subRouteLengths.Add(distance);
//                LoopEnd:;
//                }

//                distance = 0;
//                end = new Vector2(0, 0);
//            }

//            nextPointDistance = Vector2.Distance(start, end);

//            distance += nextPointDistance;

//            start = end;
//        }

//        return subRouteLengths;
//    }
//}