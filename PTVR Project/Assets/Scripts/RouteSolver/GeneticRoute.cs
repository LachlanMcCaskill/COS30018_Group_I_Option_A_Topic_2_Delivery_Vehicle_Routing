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
        public List<Vector3> points;   // this is just the points in no specific order, change the order variable only
        public Vector3 depotPoint;

        //  public List<RoutePlan> GetRoutePlan2()
        //  {
        //      List<RoutePlan> routePlan = new List<RoutePlan>();
        //  
        //      for (int i = 0; i < agents.Count; i++)
        //      {
        //          RoutePlan newPlan = new RoutePlan();
        //  
        //          newPlan.Destinations = GetSubroute(i);
        //  
        //          routePlan.Add(newPlan);
        //      }
        //      return routePlan;
        //  }

        public List<RoutePlan> GetRoutePlan()
        {
            List<RoutePlan> routePlan = new List<RoutePlan>();

            int k = 0;

            for (int i = 0; i < agents.Count; i++)
            {
                RoutePlan newPlan = new RoutePlan();

                for (int j = 0; j < agents[i].Capacity; j++)
                {
                    if (k < order.Count)
                    {
                        int pointIndex = order[k];
                        k++;
                        if (pointIndex > -1)    //  skip -1s, theyre empty capacity
                        {
                            Vector3 point = points[pointIndex];
                            newPlan.Destinations.Push(point);
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
            List < List < Vector3>> subRoutes = new List<List<Vector3>>();

            int k = 0;

            for (int i = 0; i < agents.Count; i++)
            {
                List<Vector3> newSubRoute = new List<Vector3>();

                for (int j = 0; j < agents[i].Capacity; j++)
                {
                    if (k < order.Count)
                    {
                        int pointIndex = order[k];
                        k++;
                        if (pointIndex > -1)    //  skip -1s, theyre empty capacity
                        {
                            Vector3 point = points[pointIndex];
                            newSubRoute.Add(point);
                        }
                    }
                }
                subRoutes.Add(newSubRoute);
            }

            return subRoutes;
        }

        public List<float> SubRouteDistance
        {
            get
            {
                return new List<float>();
                float distance = 0;
                float nextPointDistance;
                //float currentLongestSubroute = 0;
                //float totalDistance = 0;
                List<float> subRouteLengths = new List<float>();

                Vector3 start = new Vector3(0, 0, 0);
                Vector3 end = new Vector3(0, 0, 0);

                for (int i = 0; i < order.Count; i++)
                {
                    if (i % agents[0].Capacity == 0)    // is a new Subroute

                    if (order[i] < RouteLength)    // is not a bus index
                    {
                        end = points[order[i]];
                    }
                    else                           // is a bus index so go back to depot
                    {
                        if (subRouteLengths.Count == 0)
                        {
                            subRouteLengths.Add(distance);
                        }
                        else
                        {
                            for (int j = 0; j < subRouteLengths.Count; j++)
                            {
                                if (distance > subRouteLengths[j])
                                {
                                    subRouteLengths.Insert(j, distance);
                                    goto LoopEnd;
                                }
                            }
                            subRouteLengths.Add(distance);
                        LoopEnd:;
                        }

                        distance = 0;
                        end = new Vector2(0, 0);
                    }

                    nextPointDistance = Vector2.Distance(start, end);

                    distance += nextPointDistance;

                    start = end;
                }

                return subRouteLengths;
            }
        }

        public List<float> SubRoutes
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

                subRouteDistances.Sort();
                subRouteDistances.Reverse();

                //  string log = "Sorting\n";
                //  for (int i = 0; i < subRouteDistances.Count; i++)
                //  {
                //      log += subRouteDistances[i] + " ";
                //  }
                //  Debug.Log(log);

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

        public List<float> WeightedSubRoutes
        {
            get
            {
                List<float> subRouteDistances = new List<float>();
                List<List<Vector3>> routes = GetSubroutes();
                for (int i = 0; i < routes.Count; i++)
                {
                    float subDistance = 0;
                    Vector3 start = new Vector3(0, 0, 0);   // need to add the proper start coordinates to this later
                    
                    for (int j = 0; j < routes[i].Count; j++)
                    {
                        Vector3 next = routes[i][j];
                        float rawDistance = Vector3.Distance(start, next);
                        float passengerWeight = (float)Math.Sqrt(routes[i].Count - j);
                        subDistance += rawDistance * passengerWeight;
                        start = next;
                    }
                    subDistance += Vector3.Distance(start, Vector3.zero);
                    subRouteDistances.Add(subDistance);
                }

                subRouteDistances.Sort();
                subRouteDistances.Reverse();

                //  string log = "Sorting\n";
                //  for (int i = 0; i < subRouteDistances.Count; i++)
                //  {
                //      log += subRouteDistances[i] + " ";
                //  }
                //  Debug.Log(log);

                return subRouteDistances;
            }
        }

        public float WeightedDistance      // longest subroute + half second subroute, third third etc
        {
            get
            {
                //  return WeightedSubRoutes.Sum();
                return TotalDistance;
                List<float> subRouteLengths = SubRoutes;
                for (int i = 0; i < subRouteLengths.Count; i++)
                {
                    subRouteLengths[i] /= (i + 1);
                }
                return subRouteLengths.Sum();
            }
        }

        public int RouteLength
        {
            get
            {
                return points.Count;
            }
        }

        public GeneticRoute(Vector3 _start, List<Vector3> _points, List<int> _order, System.Random r, List<TransportAgentIntroductionMessage> agentsWithCapacities)
        {
            generation = 0;
            depotPoint = _start;
            agents = agentsWithCapacities;
            points = _points;

            order = new List<int>(_order);

            //  CreateRouteOrder(_order);

            RandomiseOrder(r);
        }

        // Deprecated
        //  public GeneticRoute(List<Vector3> _points, List<int> _order, string _name)
        //  {
        //      // used to copy
        //      name = _name;
        //      points = _points;
        //      order = new List<int>(_order);
        //      //  CreateRouteOrder(_order);
        //  }


        void RandomiseOrder(System.Random r)
        {
            for (int i = 0; i < 50; i++)    // randomise initial routes
            {
                order = Mutate(r);
            }
        }

        List<int> Mutate(System.Random r)
        {
            for (int i = 0; i < Math.Max(order.Count / 10, 1); i++)
            {
                //  Swaps the position of two points in the route order
                //  make sure to preserve index 0
                int gene = r.Next(0, order.Count - 1);
                int location = r.Next(0, order.Count - 2);
                //Debug.Log("Swapping genes: " + firstGene + "/" + secondGene);

                int geneValue = order[gene];

                int temp = geneValue;
                order.Remove(geneValue);
                order.Insert(location, geneValue);
                //  order[firstGene] = order[secondGene];
                //  order[secondGene] = temp;
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
        void OverwriteGenes(List<int> segment)
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

        void RemoveGarbage()
        {
            List<int> trailing = new List<int>();
            
            for (int i = TotalAgentCapacity; i < order.Count; i++)
            {
                trailing.Add(order[i]);
            }

            string log = "Generation " + generation +
                "\nOrder Before: ";
            for (int k = 0; k < order.Count; k++)
            {
                log += (order[k] + " ");
            }

            order.RemoveRange(TotalAgentCapacity, trailing.Count);
            log += "\nOrder Cut: ";
            for (int k = 0; k < order.Count; k++)
            {
                log += (order[k] + " ");
            }


            int j = 0;


            for (int i = 0; i < trailing.Count; i++)
            {
                while (order[j % order.Count] != -1 && j < 99)
                {
                    j++;
                }
                if (j < 99) order[j % order.Count] = trailing[i];
            }

            //if (j >= 99)
            //{
            //    log += "\nTrailing: ";
            //    for (int k = 0; k < trailing.Count; k++)
            //    {
            //        log += (trailing[k] + " ");
            //    }

            //}
            //log += "\nOrder Final: ";
            //for (int k = 0; k < order.Count; k++)
            //{
            //    log += (order[k] + " ");
            //}
            //Debug.Log(log);

            //for (int i = 0; i < order.Count; i++)
            //{
            //    if (order[i] == -1)
            //    {
            //        while (trailing.Count > trailingCount && trailing[trailingCount] == -1)
            //        {
            //            trailingCount++;
            //        }

            //        if (trailing.Count > trailingCount)
            //        {
            //            order[i] = trailing[trailingCount];
            //            trailingCount++;
            //        }
            //        else break;
            //    }
            //}

        }

        // create route from parents
        public GeneticRoute(List<Vector3> _points, GeneticRoute mother, GeneticRoute father, System.Random _r, List<TransportAgentIntroductionMessage> agentsWithCapacities)
        {
            // this is the constructor for a child
            // ordered crossover https://towardsdatascience.com/evolution-of-a-salesman-a-complete-genetic-algorithm-tutorial-for-python-6fe5d2b3ca35

            generation = mother.generation + 1;
            agents = agentsWithCapacities;
            points = _points;
            order = new List<int>(mother.order);



            string logSegments = "M: " + GetGeneString();
            logSegments += "F: " + father.GetGeneString();
            List<int> segment = CreateSegment(father, _r);
            int insertIndex = segment[0];
            segment.RemoveAt(0);

            logSegments += "Segment: ";
            foreach (int s in segment)
            {
                logSegments += s + " ";
            }
            logSegments += "\n";

            OverwriteGenes(segment);
            logSegments += "Ov: " + GetGeneString();

            InsertSegment(insertIndex, segment);
            logSegments += "In: " + GetGeneString();

            RemoveGarbage();
            logSegments += "Rm: " + GetGeneString();

            //  Debug.Log(logSegments);



            //  int j = 1;
            //  for (int i = 1; i < order.Count - 1; i++)
            //  {
            //      if (i >= startGene && i <= endGene)
            //      {
            //          continue; // don't overwrite father's segment
            //      }
            //      while (segment.Contains(mother.order[j]) && mother.order[j] != 0)
            //      {
            //          j++; // if we already have a gene from the father skip it
            //      }
            //  
            //      order[i] = mother.order[j];
            //      j++;
            //  }


            Mutate(_r);
            //  log += GetGeneString();

            PrintBreeding(mother, father, insertIndex, insertIndex + segment.Count, logSegments);
            //  Debug.Log(log);
            //  PrintDistance();
        }

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
            //  father.PrintDistance();
            // father.PrintGenes();
            //Debug.Log("Child:  ");
            log += GetGeneString();
            //  PrintDistance();
            CheckDuplicates();
            string points = CheckPoints();
            if (points.Length > 0)
            {
                log += points + " Printing Segment Log\n ";
                log += segmentLog;
            }
            Debug.LogWarning(log);  // put it here so it doesn't clog things up
        }

        string CheckPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (!order.Contains(i))
                {
                    Debug.LogError("Trailing: Chromosome is missing a gene: " + i + " in generation: " + generation);
                    return "Chromosome is missing a gene: " + i;
                }
            }
            return "";
        }

        void CheckDuplicates()
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

        Stack<Vector3> GetSubroute(int agentNumber)
        {
            Stack<Vector3> subRoute = new Stack<Vector3>();
            int routeNumber = 0;
            for (int i = 0; i < agentNumber + 1; i++)
            {
                routeNumber += agents[i].Capacity;
            }
            return subRoute;
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
                //  if (g >= RouteLength)
                //  {
                //      geneString += 0 + "  ";
                //  }
                //  else
                geneString += g + " ";
                if (g < 10 && g > -1)
                {
                    geneString += " "; // want spaces between genes the same
                }
            }
            geneString += "\n";
            //  geneString += "    Weighted Distance: " + WeightedDistance + "[" + SubRouteStrings + "]\n";

            return geneString;
        }

        public string SubRouteStrings
        {
            get
            {
                string routeStrings = "";
                List<float> subRouteLengths = SubRouteDistance;

                for (int i = 0; i < subRouteLengths.Count; i++)
                {
                    routeStrings += subRouteLengths[i] + ", ";
                }

                if (routeStrings.Length > 3)
                {
                    routeStrings = routeStrings.Substring(0, routeStrings.Length - 2);
                }
                return routeStrings;
            }
        }
    }
}