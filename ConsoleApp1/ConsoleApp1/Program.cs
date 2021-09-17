using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GeneticAlgorithmMTSP
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector2[] points = new Vector2[12];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(-1, -5);
            points[2] = new Vector2(-5, -8);
            points[3] = new Vector2(-3, 6);
            points[4] = new Vector2(-7, 3);
            points[5] = new Vector2(-3, 6);
            points[6] = new Vector2(-9, 9);
            points[7] = new Vector2(1, 6);
            points[8] = new Vector2(5, 5);
            points[9] = new Vector2(8, 6);
            points[10] = new Vector2(4, -5);
            points[11] = new Vector2(8, -3);

            //
            Route[] routes = new Route[2];

            Random r = new Random();

            //  Initial 10 routes
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = new Route(points, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, i.ToString(), r);
            }

            //console.WriteLine("Route Distances: ");
            int currentGeneration = 0;
            int maxGeneration = 100;


            float currentShortest = ShortestRoute(routes).TotalDistance;
            float currentAverage = AverageDistance(routes);

            Console.WriteLine("Generation " + currentGeneration + " Population: " + routes.Length + "\nAverage Distance: " + currentAverage
                + "\nShortest Route: " + currentShortest);

            while (currentGeneration < maxGeneration)
            {
                float oldShortest = currentShortest;
                float oldAverage = currentAverage;

                currentShortest = ShortestRoute(routes).TotalDistance;
                currentAverage = AverageDistance(routes);

                currentGeneration++;
                RunNextGeneration(routes, r);

                Console.Write("\nGeneration " + currentGeneration + " Population: " + routes.Length + "\nAverage Distance: " + currentAverage);
                if (oldAverage < currentAverage)
                {
                    Console.WriteLine(" [" + (oldAverage - currentAverage) + "] slower");
                }
                else
                {
                    Console.WriteLine(" [" + (oldAverage - currentAverage) + "] faster");
                }

                Console.Write("Shortest Route: " + currentShortest);

                if (oldShortest < currentShortest)
                {
                    Console.WriteLine(" [" + (oldShortest - currentShortest) + "] slower");
                }
                else
                {
                    Console.WriteLine(" [" + (oldShortest - currentShortest) + "] faster");
                }
            }
        }
        static void RunNextGeneration(Route[] routes, Random r)
        {
            Route[] temp = CopyRoutes(routes);

            GetNextGeneration(routes, temp, r);

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].name = null;
                temp[i].order = null;
                temp[i].points = null;
                temp[i] = null;
            }
            temp = null;
        }
        static Route[] CopyRoutes(Route[] source)
        {
            Route[] newRoutes = new Route[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                newRoutes[i] = new Route(source[i].points, source[i].order, source[i].name);
            }
            return newRoutes;
        }
        static Route[] GetParents(Route[] routes, Random r)
        {
            int[][] selection = PopulationSelection(0, routes.Length, 20, r);

            Route mother = GetFittestRouteFromRandomSelection(routes, selection[0]);
            Route father = GetFittestRouteFromRandomSelection(routes, selection[1]); ;

            return new Route[] { mother, father };
        }
        static int[][] PopulationSelection(int min, int max, int size, Random r)
        {
            int select;
            int[][] rarry = new int[2][];
            rarry[0] = new int[size];
            rarry[1] = new int[size];

            for (int i = 0; i < rarry.Length; i++)
            {
                for (int j = 0; j < rarry[i].Length; j++)
                {
                    select = r.Next(min, max);
                    while (rarry[0].Contains(select) || rarry[1].Contains(select))
                    {
                        select = (select + 1) % max;
                    }
                }
            }
            return rarry;
        }
        static void GetNextGeneration(Route[] routes, Route[] temp, Random r)
        {
            for (int i = 0; i < routes.Length; i++)
            {
                // totally random, no lfitness test
                Route[] parents = GetParents(temp, r);
                routes[i].CreateChild(parents[0], parents[1], r); //overwrite
                //routes[i] = new Route(parents[0], parents[1], r);
            }
        }
        static float AverageDistance(Route[] routes)
        {
            float distance = 0;
            foreach (Route r in routes)
            { distance += r.TotalDistance; }
            return distance / routes.Length;
        }
        static Route GetFittestRouteFromRandomSelection(Route[] routes, int[] selection)
        {
            Route fittest = routes[selection[0]];
            for (int i = 1; i < selection.Length; i++)
            {
                if (routes[selection[i]].TotalDistance < fittest.TotalDistance)
                {
                    fittest = routes[selection[i]];
                }
            }
            return fittest;
        }
        static Route ShortestRoute(Route[] routes)
        {
            Route shortest = routes[0];
            for (int i = 1; i < routes.Length; i++)
            {
                if (routes[i].TotalDistance < shortest.TotalDistance)
                {
                    shortest = routes[i];
                }
            }
            return shortest;
        }

        Route GetParent(Route[] routes)
        {
            Route shortest = routes[0];
            for (int i = 1; i < routes.Length; i++)
            {
                if (routes[i].TotalDistance < shortest.TotalDistance)
                {
                    shortest = routes[i];
                }
            }
            return shortest;
        }
    }


    class Route
    {
        public void PrintDistance()
        {
            //console.WriteLine("Route " + name + " distance: " +  TotalDistance);
        }
        public void PrintGenes()
        {
            foreach (int g in order)
            {
                //console.Write(g + " ");
                if (g < 10)
                {
                    //console.Write(" "); // want spaces between genes the same
                }
            }
            //console.WriteLine();
        }
        public float TotalDistance
        {
            get
            {
                float distance = 0;
                float nextPointDistance;
                for (int i = 0; i < order.Length - 1; i++)
                {
                    nextPointDistance = Vector2.Distance(points[order[i]], points[order[i + 1]]);
                    //  //console.WriteLine("Distance from point " + i + " to point " + (i + 1) + ": " + nextPointDistance);

                    distance += nextPointDistance;
                    //  //console.WriteLine("Current distance traveled: " + distance);
                }
                // go back to start
                nextPointDistance = Vector2.Distance(points[order.Length - 1], points[order[0]]);
                //  //console.WriteLine("Distance from point " + i + " to point " + (i + 1) + ": " + nextPointDistance);

                distance += nextPointDistance;

                return distance;
            }
        }
        public string name;
        public int[] order;
        public Vector2[] points;   // this is just the points in no specific order, change the order variable only
        public Route(Vector2[] _points, int[] _order, string _name, Random r)
        {
            //  initial routes used for first generation, just random order
            name = _name;
            points = _points;
            order = _order;

            RandomiseOrder(r);
        }
        public Route(Vector2[] _points, int[] _order, string _name)
        {
            // used to copy
            name = _name;
            points = _points;
            order = new int[_order.Length];
            _order.CopyTo(order, 0);
        }

        void RandomiseOrder(Random r)
        {
            for (int i = 0; i < 50; i++)    // randomise initial routes
            {
                order = Mutate(r);
            }
        }

        int[] Mutate(Random r)
        {
            //  Swaps the position of two points in the route order
            //  make sure to preserve index 0
            int firstGene = r.Next(1, order.Length);
            int secondGene = r.Next(firstGene, order.Length);
            //console.WriteLine("Swapping genes: " + firstGene + "/" + secondGene);

            int temp = order[firstGene];
            order[firstGene] = order[secondGene];
            order[secondGene] = temp;
            return order;
        }

        public void CreateChild(Route mother, Route father, Random r)
        {
            // this is the constructor for a child
            // ordered crossover https://towardsdatascience.com/evolution-of-a-salesman-a-complete-genetic-algorithm-tutorial-for-python-6fe5d2b3ca35

            name = mother.name + father.name;
            //points = mother.points;
            order = new int[points.Length];

            //  Console.WriteLine("mother length " + mother.order.Length);

            //console.WriteLine("\nRoute " + name);

            int startGene = r.Next(1, order.Length);    // start at 1, index 0 will always be depot (0,0)
            int endGene = r.Next(startGene, order.Length);

            int[] segment = new int[endGene - startGene + 1];
            for (int i = 0; i < segment.Length; i++)
            {
                segment[i] = father.order[startGene + i];
                order[startGene + i] = father.order[startGene + i];
            }

            int j = 0;
            for (int i = 0; i < order.Length; i++)
            {
                if (i >= startGene && i <= endGene)
                {
                    continue; // don't overwrite father's segment
                }
                while (segment.Contains(mother.order[j]))
                {
                    j++; // if we already have a gene from the father skip it
                }

                order[i] = mother.order[j];
                j++;
            }

            //console.Write("Mother: ");
            mother.PrintGenes();
            mother.PrintDistance();
            //console.Write("Father: ");
            for (int i = 0; i < order.Length; i++)
            {
                if (i < startGene)
                {
                    //console.Write("   ");
                }
                else if (i <= endGene)
                {
                    //console.Write(father.order[i] + " ");
                    if (father.order[i] < 10)
                    {
                        //console.Write(" ");
                    }
                }
            }
            //console.WriteLine();
            father.PrintDistance();
            //  father.PrintGenes();
            //console.Write("Child:  ");
            PrintGenes();
            PrintDistance();
            CheckDuplicates();
            //console.Write("Child (After Mutation):  ");
            Mutate(r);
            PrintGenes();
            PrintDistance();
        }

        void CheckDuplicates()
        {
            for (int i = 0; i < order.Length; i++)
            {
                for (int j = i + 1; j < order.Length; j++)
                {
                    if (order[i] == order[j])
                    {
                        //console.WriteLine("duplicate found " + i + " = " + j);
                    }
                }
            }
        }
    }

    class chromosome
    {
        chromosome()
        {

        }
    }

    //  class point
    //  {
    //      public float[] position
    //      public point(float[] _position)
    //      {
    //          position = _position;
    //      }
    //  }
}
