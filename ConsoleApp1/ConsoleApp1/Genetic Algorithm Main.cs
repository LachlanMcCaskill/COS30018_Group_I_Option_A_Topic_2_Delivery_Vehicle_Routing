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
        const int selectionSize = 20;   // takes a random selection of population then finds fittest member of that selection
        const int defaultPoints = 20;   // takes a random selection of population then finds fittest member of that selection
        const int defaultPopulation = 40;   // takes a random selection of population then finds fittest member of that selection
        const int defaultGenerations = 18;   // takes a random selection of population then finds fittest member of that selection
        const int defaultBuses = 4;   // takes a random selection of population then finds fittest member of that selection

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
                    rarry[i][j] = select;
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
            int numberBuses = vars[3];
            List<string> subroutes = new List<string>();
            List<float> averages = new List<float>();
            List<float> best = new List<float>();

            Random r = new Random();    // create this here because of how c# random works, pass this if you need a random variable

            List<int> baseOrder = CreateOrder(numberPoints, numberBuses);
            //  List<int> baseOrder = CreateOrder(numberPoints, defaultBuses);    //  create an array of ints 0 to maxpoints
            Vector2[] points = GeneratePoints(numberPoints, r); /// generate random points
            Route[] routes = new Route[numberPopulation];   //  create empty array of routes

            int currentGeneration = 0;  // generationloop compares the current and max generations till max is reached

            //  Generate initial (randomised) routes
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = new Route(points, baseOrder, i.ToString(), r);
                //   = routes[i].order;
            }

            // Get First Generation Information
            string shortestSubRoutes = ShortestRoute(routes).SubRouteStrings;
            float currentShortest = ShortestRoute(routes).WeightedDistance;    // shortest from CURRENTGENERATION not the currently fastest generated route
            float currentAverage = AverageDistance(routes);                 // see above

            PrintInitialGeneration(routes, currentGeneration, numberPopulation, numberPoints, maxGeneration, currentAverage, currentShortest);

            float firstRoute = currentShortest; // fastest route from initial random generation (if you can't beat this then the optimisation is not so good)
            float fastestRoute = firstRoute;         // best route so far
            string fastestSubRoutes = ShortestRoute(routes).SubRouteStrings;
            int fastestGeneration = 0;      // generation of best route
            float[] nGenerationAverages = new float[maxGeneration];   // where n is some division of max generations (ie average of each 100 generations)

            // Generation Loop
            while (currentGeneration < maxGeneration)
            {
                averages.Add(currentAverage);
                best.Add(currentShortest);
                subroutes.Add(shortestSubRoutes);
                float oldShortest = currentShortest;
                float oldAverage = currentAverage;

                currentGeneration++;
                RunNextGeneration(routes, r);

                shortestSubRoutes = ShortestRoute(routes).SubRouteStrings;
                currentShortest = ShortestRoute(routes).WeightedDistance;
                currentAverage = AverageDistance(routes);

                PrintSubsequentGeneration(currentGeneration, numberPopulation, numberPoints, maxGeneration, currentAverage, currentShortest, oldAverage, oldShortest);

                if (currentShortest < fastestRoute)
                {
                    fastestSubRoutes = ShortestRoute(routes).SubRouteStrings;
                    fastestRoute = currentShortest;
                    fastestGeneration = currentGeneration;
                }
            }

            PrintFinalResults(firstRoute, fastestRoute, fastestGeneration, fastestSubRoutes, nGenerationAverages, currentShortest, averages, best, subroutes);

            int.Parse(Console.ReadLine());
        }

        static void PrintFinalResults(float firstRoute, float fastestRoute, int fastestGeneration, string fastestSubRoutes, float[] generationAverages, float finalRoute, List<float> averages, List<float> bests, List<string>subroutes)
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
            Console.WriteLine("The fastest route was " + fastestRoute + "[" + fastestSubRoutes + "] in generation " + fastestGeneration);

            if (fastestGeneration == 0)
            {
                Console.WriteLine("Woops the random generation beat your optimisation algorithm...");
            }
            else
            {
                Console.WriteLine("Which was " + (firstRoute - fastestRoute) + " faster than the initial fastest route ");
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
            Console.WriteLine(log);
        }

        static List<int> CreateOrder(int pointCount, int busCount)
        {
            // just generates an array of sequential ints (this is basically the chromosome)
            List<int> order = new List<int>();
            int[] numberPoints = new int[pointCount + busCount];

            for (int i = 0; i < numberPoints.Length; i++)
            {
                order.Add(i);
            }

            return order;
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
            int numberPoints = defaultPoints;
            int numberGenerations = defaultGenerations;
            int numberPopulation = defaultPopulation;
            int numberBuses = defaultBuses;
            int[] vars = new int[] { numberPoints, numberPopulation, numberGenerations, numberBuses };
            Console.WriteLine("Enter number of points: Enter 0 for defaults (Points: " + defaultPoints + ", Population: " + defaultPopulation +
                ", Generations: " + defaultGenerations + ", Buses: " + defaultBuses + ", Selection size: " + selectionSize + ")");

           //   const int selectionSize = 20;   // takes a random selection of population then finds fittest member of that selection
           //   const int defaultPoints = 40;   // takes a random selection of population then finds fittest member of that selection
           //   const int defaultPopulation = 100;   // takes a random selection of population then finds fittest member of that selection
           //   const int defaultGenerations = 30;   // takes a random selection of population then finds fittest member of that selection
           //   const int defaultBuses = 5;   // takes 

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

            while (input == 0)
            {
                Console.WriteLine("Buses number:");
                input = int.Parse(Console.ReadLine());
            }

            numberBuses = input;

            vars = new int[] { numberPoints, numberPopulation, numberGenerations, numberBuses };

            return vars;
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

        static void PrintInitialGeneration(Route[] routes, int currentGeneration, int numberPopulation, int numberPoints, int numberGenerations, float currentAverage, float currentShortest)
        {
            Console.WriteLine("Generation " + currentGeneration + " (Population: " + numberPopulation + ", Points " + numberPoints + ", Generations: " + numberGenerations + ")\nAverage Distance: " + currentAverage
                + "\nShortest Route: " + currentShortest);
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i].PrintGenes();
            }
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
            { distance += r.WeightedDistance; }
            return distance / routes.Length;
        }

        static Route GetFittestRouteFromRandomSelection(Route[] routes, int[] selection)
        {
            Route fittest = routes[selection[0]];
            for (int i = 1; i < selection.Length; i++)
            {
                if (routes[selection[i]].WeightedDistance < fittest.WeightedDistance)
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
                if (routes[i].WeightedDistance < shortest.WeightedDistance)
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
                if (routes[i].WeightedDistance < shortest.WeightedDistance)
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
            //Console.WriteLine("Route " + name + " distance: " +  WeightedDistance);
        }
        public void PrintGenes()
        {
            Console.WriteLine(GetGeneStrng());
        }

        public String GetGeneStrng()
        {
            String geneString = "";

            foreach (int g in order)
            {
                if (g >= RouteLength)
                {
                    geneString += 0 + "  ";
                }
                else
                {
                    geneString += g + " ";
                    if (g < 10)
                    {
                        geneString += " "; // want spaces between genes the same
                    }
                }
            }
            geneString += "    Weighted Distance: " + WeightedDistance + "[" + SubRouteStrings + "]\n";

            return geneString;
        }

        public List<float> SubRouteDistance
        {
            get
            {

                float distance = 0;
                float nextPointDistance;
                //float currentLongestSubroute = 0;
                //float totalDistance = 0;
                List<float> subRouteLengths = new List<float>();

                Vector2 start = new Vector2(0, 0);
                Vector2 end = new Vector2(0, 0);

                for (int i = 0; i < order.Count; i++)
                {
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

        public string SubRouteStrings
        {
            get
            {
                string routeStrings = "";
                List<float> subRouteLengths = SubRouteDistance;
                
                for (int i= 0; i < subRouteLengths.Count; i++)
                {
                    routeStrings += subRouteLengths[i] + ", ";
                }
                routeStrings = routeStrings.Substring(0, routeStrings.Length - 2);
                return routeStrings;
            }
        }

        public float WeightedDistance      // longest subroute + half second subroute, third third etc
        {
            get
            {
                List<float> subRouteLengths = SubRouteDistance;
                for (int i = 0; i < subRouteLengths.Count; i++)
                {
                    subRouteLengths[i] /= (i + 2);
                }
                return subRouteLengths.Sum();
            }
        }

        public int RouteLength
        {
            get
            {
                return points.Length;
            }
        }

        public string name;
        public List<int> order;
        public Vector2[] points;   // this is just the points in no specific order, change the order variable only

        public Route(Vector2[] _points, List<int> _order, string _name, Random r)
        {
            //  initial routes used for first generation, just random order
            name = _name;
            points = _points;

            order = new List<int>(_order);

            //  CreateRouteOrder(_order);

            RandomiseOrder(r);
        }

        //  void CreateRouteOrder(List<int> _order)
        //  {
        //      string log = "Before: ";
        //      string log2 = "After:  ";
        //  
        //      order = new List<int>();
        //      for (int i = 0; i < _order.Count; i++)
        //      {
        //          order.Add(i);
        //          for (int j = 0; j < _order[i].Count; j++)
        //          {
        //              order[i].Add(_order[i][j]);
        //              log += _order[i][j] + " ";
        //              log2 += order[i][j] + " ";
        //          }
        //      }
        //  
        //      Console.WriteLine(log);
        //      Console.WriteLine(log2);
        //  }

        public Route(Vector2[] _points, List<int> _order, string _name)
        {
            // used to copy
            name = _name;
            points = _points;
            order = new List<int>(_order);
            //  CreateRouteOrder(_order);
        }

        void RandomiseOrder(Random r)
        {
            for (int i = 0; i < 50; i++)    // randomise initial routes
            {
                order = Mutate(r);
            }
        }

        List<int> Mutate(Random r)
        {
            for (int i = 0; i < Math.Max(order.Count / 10, 1); i++)
            {

                //  Swaps the position of two points in the route order
                //  make sure to preserve index 0
                int gene = r.Next(1, order.Count - 1);
                int location = r.Next(1, order.Count - 2);
                //Console.WriteLine("Swapping genes: " + firstGene + "/" + secondGene);

                int temp = order[gene];
                order.Remove(gene);
                order.Insert(location, gene);
                //  order[firstGene] = order[secondGene];
                //  order[secondGene] = temp;
            }
            return order;
        }

        public void CreateChild(Route mother, Route father, Random r)
        {
            String log = "Breeding: Initial, Mother, Father, Purebreed, Mutation \n";
            log += GetGeneStrng();
            // this is the constructor for a child
            // ordered crossover https://towardsdatascience.com/evolution-of-a-salesman-a-complete-genetic-algorithm-tutorial-for-python-6fe5d2b3ca35

            //  name = (mother.name + father.name).Substring(0, ;
            //points = mother.points;
            //  order = new int[points.Length];
            //  Console.WriteLine("mother length " + mother.order.Length);
            //Console.WriteLine("\nRoute " + name);


            order = new List<int>(mother.order);

            int gene1 = r.Next(1, order.Count -1 );    // start at 1, index 0 will always be depot (0,0), and end has to be depot to
            int gene2 = r.Next(1, order.Count -1);
            int startGene = Math.Min(gene1, gene2);
            int endGene = Math.Max(gene1, gene2);

            List<int> segment = new List<int>();
            //  new int[endGene - startGene + 1];
            int segmentLengthCount = endGene - startGene + 1;
            for (int i = 0; i < segmentLengthCount; i++)
            {
                segment.Add(father.order[startGene + i]);
                //  segment[i] = father.order[startGene + i];
                order[startGene + i] = father.order[startGene + i];
            }

            int j = 1;
            for (int i = 1; i < order.Count - 1; i++)
            {
                if (i >= startGene && i <= endGene)
                {
                    continue; // don't overwrite father's segment
                }
                while (segment.Contains(mother.order[j]) && mother.order[j] != 0)
                {
                    j++; // if we already have a gene from the father skip it
                }

                order[i] = mother.order[j];
                j++;
            }

            log += mother.GetGeneStrng();
            
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
            log += "   Weighted Distance: " + father.WeightedDistance + "[" + father.SubRouteStrings + "]\n";
            //  father.PrintDistance();
            // father.PrintGenes();
            //Console.Write("Child:  ");
            log += GetGeneStrng();
            //  PrintDistance();
            CheckDuplicates();
            //Console.Write("Child (After Mutation):  ");
            Mutate(r);
            log += GetGeneStrng();
            Console.WriteLine(log);
            //  PrintDistance();
        }

        void PrintBreeding()
        {

        }

        void CheckDuplicates()
        {
            for (int i = 0; i < order.Count; i++)
            {
                for (int j = i + 1; j < order.Count; j++)
                {
                    if (order[i] == order[j] && order[i] != 0)
                    {
                        Console.WriteLine("duplicate found " + i + " = " + j);
                    }
                }
            }
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
