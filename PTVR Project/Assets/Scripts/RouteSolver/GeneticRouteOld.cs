using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
    class GeneticRouteOld
    {
        List<TransportAgentIntroductionMessage> agents;

        int generation;
        public string name;
        public List<int> order;
        //  public List<Vector3> points;   // this is just the points in no specific order, change the order variable only
        public List<DestinationMessage> destinations;
        public Vector3 depotPoint;
        public int trips;
        List<bool> specialArray;

        public int RouteLength
        {
            get
            {
                return destinations.Count;
                //return points.Count;
            }
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
            for (int i = 0; i < Math.Max(order.Count / 10, 1); i++) // mutations scale to the size of the chromosome
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

        public GeneticRouteOld(Vector3 _start, List<int> _order, System.Random r, List<TransportAgentIntroductionMessage> agentsWithCapacities,
            int _trips, List<DestinationMessage> _destinations, List<bool> _specialArray)
        {
            specialArray = _specialArray;
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

            OrderSpecialDestinations(r);
        }

        // create route from parents
        public GeneticRouteOld(GeneticRoute mother, GeneticRoute father, System.Random _r)
        {
            // this is the constructor for a child
            // ordered crossover https://towardsdatascience.com/evolution-of-a-salesman-a-complete-genetic-algorithm-tutorial-for-python-6fe5d2b3ca35
            depotPoint = mother.depotPoint;
            trips = mother.trips;
            //specialArray = mother.specialArray;
            //generation = mother.generation + 1;
            //agents = mother.agents;
            destinations = mother.destinations;
            //  points = mother.points;
            order = new List<int>(mother.order);
            List<int> segment = CreateSegment(father, _r);
            int insertIndex = segment[0];
            segment.RemoveAt(0);

            Mutate(_r);

            string logSegments = "M: " + GetGeneString();
            logSegments += "F: " + father.GetGeneString();
            logSegments += "Segment: ";
            foreach (int s in segment)
            {
                logSegments += s + " ";
            }
            logSegments += "\n";

            OverwriteGenesFromSegment(segment);
            logSegments += "Ov: " + GetGeneString();

            InsertSegment(insertIndex, segment);
            logSegments += "In: " + GetGeneString();

            IncorporateTrailingGenes();
            logSegments += "Rm: " + GetGeneString();

            OrderSpecialDestinations(_r);

            PrintBreeding(mother, father, insertIndex, insertIndex + segment.Count, logSegments);
        }

        void OrderSpecialDestinations(System.Random r)
        {
            for (int i = 0; i < order.Count; i++)
            {
                if (order[i] != -1)
                {
                    if (destinations[order[i]].special && !specialArray[i]) // if the destination is special but is currently carried by a non special agent
                    {
                        int random = r.Next(TotalAgentCapacity / 2, TotalAgentCapacity);
                        while (specialArray[random] && (order[random] == -1 || !destinations[order[random]].special))   // new agent must be special, 
                        {
                            random = r.Next(TotalAgentCapacity / 2, TotalAgentCapacity);
                        }
                        Debug.Log("Swapping: " + order[random] + " with " + order[i]);
                        int temp = order[random];
                        order[random] = order[i];
                        order[i] = temp;
                    }
                }
            }
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

        void PrintBreeding(GeneticRoute mother, GeneticRoute father, int startGene, int endGene, string segmentLog)
        {
            string log = "Breeding: Mother, Father, Child \n";
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
                totalCapacity *= 2;
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

        public List<RoutePlan> GetRoutePlan()
        {
            List<RoutePlan> routePlan = new List<RoutePlan>();

            int k = 0;

            for (int i = 0; i < agents.Count; i++)
            {
                RoutePlan newPlan = new RoutePlan();

                for (int t = 0; t < trips; t++)
                {
                    if (t != 0)
                    {
                        newPlan.Destinations.Push(depotPoint);  // Debug.LogError("This is the line that causes the error"); fixed by adding depot to TransportNetwork.CreateRouteFromPlan
                    }
                    for (int j = 0; j < agents[i].Capacity; j++)
                    {
                        if (k < order.Count)
                        {
                            int pointIndex = order[k];
                            k++;
                            if (pointIndex > -1)    //  skip -1s, theyre empty capacity
                            {
                                Vector3 point = destinations[pointIndex].Position;
                                newPlan.Destinations.Push(point);
                            }
                        }
                    }
                }
                newPlan.Destinations = Reverse(newPlan.Destinations);
                routePlan.Add(newPlan);
            }

            return routePlan;
        }

        public List<List<Vector3>> GetSubroutes()
        {
            List<List<Vector3>> subRoutes = new List<List<Vector3>>();

            int k = 0;

            for (int i = 0; i < agents.Count; i++)
            {
                List<Vector3> newSubRoute = new List<Vector3>();
                for (int t = 0; t < trips; t++)
                {
                    if (t != 0)
                    {
                        newSubRoute.Add(new Vector3(51, 0, 51));
                    }
                    for (int j = 0; j < agents[i].Capacity; j++)
                    {
                        if (k < order.Count)
                        {
                            int pointIndex = order[k];
                            k++;
                            if (pointIndex > -1)    //  skip -1s, theyre empty capacity
                            {
                                Vector3 point = destinations[pointIndex].Position;
                                newSubRoute.Add(point);
                            }
                        }
                    }
                }
                subRoutes.Add(newSubRoute);
            }

            return subRoutes;
        }

        public List<float> SubrouteDistancesOrdered
        {
            get
            {
                List<float> subRouteDistances = new List<float>();
                List<List<Vector3>> routes = GetSubroutes();
                for (int i = 0; i < routes.Count; i++)
                {
                    float subDistance = 0;
                    Vector3 start = depotPoint;   // need to add the proper start coordinates to this later

                    for (int j = 0; j < routes[i].Count; j++)
                    {
                        Vector3 next = routes[i][j];
                        subDistance += Vector3.Distance(start, next);
                        start = next;
                    }
                    subDistance += Vector3.Distance(start, depotPoint);
                    subRouteDistances.Add(subDistance);
                }

                subRouteDistances.Sort();       // sort from longest to shortest
                subRouteDistances.Reverse();    // this is used for some weighted distance calculations

                return subRouteDistances;
            }
        }

        public float TotalDistance
        {
            get
            {
                float distance = 0;
                List<List<Vector3>> routes = GetSubroutes();
                for (int i = 0; i < routes.Count; i++)
                {
                    Vector3 start = depotPoint;   // need to add the proper start coordinates to this later

                    for (int j = 0; j < routes[i].Count; j++)
                    {
                        Vector3 next = routes[i][j];
                        distance += Vector3.Distance(start, next);
                        start = next;
                    }
                    distance += Vector3.Distance(start, depotPoint);
                }

                return distance;
            }
        }

        public float WeightedDistance      // the weighted distances I tried seemed less than optimal so just stick with total distance
        {
            get
            {
                //  return WeightedSubRoutes.Sum();
                return TotalDistance;
                List<float> subRouteLengths = SubrouteDistancesOrdered;
                for (int i = 0; i < subRouteLengths.Count; i++)
                {
                    subRouteLengths[i] /= (i + 1);
                }
                return subRouteLengths.Sum();
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