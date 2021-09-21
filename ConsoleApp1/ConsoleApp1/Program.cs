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
        const int selectionSize = 50;   // takes a random selection of population then finds fittest member of that selection

        //  this is the fitness test, these are the important parts (assuming the rest works)
        static Route[] GetParents(Route[] routes, Random r)
        {
            int size = Math.Min(selectionSize, routes.Length / 5);
            int[][] selection = PopulationSelection(0, routes.Length, size, r);

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
        // End fitness test

        static void Main(string[] args)
        {
            //  Setting Varaibles
            int[] vars = GetVariables();    //  user can input variables or use default variables
            int numberPoints = vars[0];
            int numberPopulation = vars[1];
            int maxGeneration = vars[2];

            Random r = new Random();    // create this here because of how c# random works, pass this if you need a random variable
            int[] baseOrder = CreateOrder(numberPoints);    //  create an array of ints 0 to maxpoints
            Vector2[] points = GeneratePoints(numberPoints, r); /// generate random points
            Route[] routes = new Route[numberPopulation];   //  create empty array of routes

            int currentGeneration = 0;  // generationloop compares the current and max generations till max is reached

            //  Generate initial (randomised) routes
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = new Route(points, baseOrder, i.ToString(), r);
            }

            // Get First Generation Information
            float currentShortest = ShortestRoute(routes).TotalDistance;    // shortest from CURRENTGENERATION not the currently fastest generated route
            float currentAverage = AverageDistance(routes);                 // see above

            PrintInitialGeneration(currentGeneration, numberPopulation, numberPoints, maxGeneration, currentAverage, currentShortest);

            float firstRoute = currentShortest; // fastest route from initial random generation (if you can't beat this then the optimisation is not so good)
            float fastestRoute = firstRoute;         // best route so far
            int fastestGeneration = 0;      // generation of best route
            float[] nGenerationAverages = new float[maxGeneration];   // where n is some division of max generations (ie average of each 100 generations)

            // Generation Loop
            while (currentGeneration < maxGeneration)
            {
                float oldShortest = currentShortest;
                float oldAverage = currentAverage;

                currentGeneration++;
                RunNextGeneration(routes, r);

                currentShortest = ShortestRoute(routes).TotalDistance;
                currentAverage = AverageDistance(routes);

                PrintSubsequentGeneration(currentGeneration, numberPopulation, numberPoints, maxGeneration, currentAverage, currentShortest, oldAverage, oldShortest);

                if (currentShortest < fastestRoute)
                {
                    fastestRoute = currentShortest;
                    fastestGeneration = currentGeneration;
                }
            }

            PrintFinalResults(firstRoute, fastestRoute, fastestGeneration, nGenerationAverages, currentShortest);

            int.Parse(Console.ReadLine());
        }

        static void PrintFinalResults(float firstRoute, float fastestRoute, int fastestGeneration, float[] generationAverages, float finalRoute)
        {
            Console.WriteLine("Evolution complete.");
            Console.WriteLine("The best route in the initial generation was " + firstRoute);
            Console.WriteLine("The best route in the final generation was " + finalRoute);

            if (finalRoute < firstRoute)
            {
                Console.WriteLine("Which was " + (firstRoute - finalRoute) + " faster than the initial fastest route ");
            }
            else
            {
                Console.WriteLine("Which was " + (finalRoute - firstRoute) + " slower than the initial fastest route :(");
            }

            Console.WriteLine();
            Console.WriteLine("The fastest route was " + fastestRoute + " in generation " + fastestGeneration);

            if (fastestGeneration == 0)
            {
                Console.WriteLine("Woops the random generation beat your optimisation algorithm...");
            }
            else
            {
                Console.WriteLine("Which was " + (firstRoute - fastestRoute) + " faster than the initial fastest route ");
            }
        }

        static int[] CreateOrder(int count)
        {
            // just generates an array of sequential ints (this is basically the chromosome)
            int[] numberPoints = new int[count];
            for (int i = 0; i < count; i++)
            {
                numberPoints[i] = i;
            }
            return numberPoints;
        }

        static Vector2[] GeneratePoints(int count, Random r)
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

        static public int[] GetVariables()
        {
            //  points, population, generation 
            int input = 0;
            int numberPoints = 100;
            int numberGenerations = 1000;
            int numberPopulation = 1000;
            int[] vars = new int[] { numberPoints, numberPopulation, numberGenerations };
            Console.WriteLine("Enter number of points: (Enter 0 for defaults)");
            input = int.Parse(Console.ReadLine());
            if (input == 0)
            {
                return vars;
            }
            else numberPoints = input;

            input = 0;  // reset for validation

            while (input == 0)
            {
                Console.WriteLine("Number of generations:");
                input = int.Parse(Console.ReadLine());
            }
            numberGenerations = input;
            input = 0;  // reset for validation

            while (input == 0)
            {
                Console.WriteLine("Population number:");
                input = int.Parse(Console.ReadLine());
            }
            numberPopulation = input;
            input = 0;  // reset for validation

            vars = new int[] { numberPoints, numberPopulation, numberGenerations };

            return vars;
        }

        static void GenerationLoop()
        {

        }

        static void PrintSubsequentGeneration(int currentGeneration, int numberPopulation, int numberPoints, int numberGenerations, float currentAverage, float currentShortest, float oldAverage, float oldShortest)
        {
            Console.WriteLine("Generation " + currentGeneration + " (Population: " + numberPopulation + ", Points " + numberPoints + ", Generations: " + numberGenerations + ")");

            Console.Write("Average Route: " + currentAverage);
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

            Console.WriteLine();
            //Console.WriteLine("Generation " + currentGeneration + " (Population: " + numberPopulation + ", Points " + numberPoints + ", Generations: + " + numberGenerations + ")\nAverage Distance: " + currentAverage
            //      + "\nShortest Route: " + currentShortest);
        }

        static void PrintInitialGeneration(int currentGeneration, int numberPopulation, int numberPoints, int numberGenerations, float currentAverage, float currentShortest)
        {
            Console.WriteLine("Generation " + currentGeneration + " (Population: " + numberPopulation + ", Points " + numberPoints + ", Generations: " + numberGenerations + ")\nAverage Distance: " + currentAverage
                + "\nShortest Route: " + currentShortest);
            Console.WriteLine();
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
            Console.WriteLine(name);
            foreach (int g in order)
            {
                Console.Write(g + " ");
                if (g < 10)
                {
                    Console.Write(" "); // want spaces between genes the same
                }
            }
            Console.WriteLine();
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

            //  name = (mother.name + father.name).Substring(0, ;
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
            //  mother.PrintGenes();
            //  mother.PrintDistance();
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
            //  father.PrintDistance();
            //  father.PrintGenes();
            //console.Write("Child:  ");
            //  PrintGenes();
            //  PrintDistance();
            CheckDuplicates();
            //console.Write("Child (After Mutation):  ");
            Mutate(r);
            //  PrintGenes();
            //  PrintDistance();
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
