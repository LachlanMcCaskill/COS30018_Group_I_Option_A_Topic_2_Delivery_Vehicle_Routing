using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
	public class KMeansClusterRouteSolver : IRouteSolver
	{
		private IRouteSolver _innerSolver;

		public KMeansClusterRouteSolver(IRouteSolver innerSolver)
		{
			_innerSolver = innerSolver;
		}

		public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, List<TransportAgentIntroductionMessage> introductionMessages)
		{
			var clusters = GroupPointsIntoClusters(points, introductionMessages.Count);
			var agents = introductionMessages.Select(agent => new List<TransportAgentIntroductionMessage>{agent});
			return clusters
				.Zip(agents, Tuple.Create) // pair each agent with a cluster
				.Select(pair => _innerSolver.Solve(start, pair.Item1, pair.Item2)[0]) // solve each agent-cluster pair individually
				.ToList();
		}

		private List<List<Vector3>> GroupPointsIntoClusters(List<Vector3> points, int clusterCount)
		{
			const int maximumIterations = 1000;
			const float stepPercentage = 0.1f;

			Vector3[] centroids = new Vector3[clusterCount];
			int[] pointCentroidIndex = new int[points.Count];
			Rect bounds = CalculateBounds(points);

			InitializeCentroids();
			
			int iteration;
			bool isMoving = true;
			for (iteration = 0; iteration < maximumIterations && isMoving; iteration++)
			{
				AssignPointsToCentroids();
				isMoving = MoveCentroids();
			}

			Log.Info($"KMeansClusterRouteSolver completed in {Log.Cyan(iteration)} iterations.");

			//  return CreateClusters();    //  added a redistribution method, it could use a lot of fine tuning but it seems to work for now

            List<List<Vector3>> createdClusters = CreateClusters();

            while (!ClustersEvenlyDistributed())
            {
                RedistributeClusters();
            }

            return createdClusters;

            Boolean ClustersEvenlyDistributed()
            {
                //  cannot have any clusters larger than than an even distribution of points, can change this for capacity later if you want
                for (int i = 0; i < createdClusters.Count; i++)
                {
                    int clusterSize = createdClusters[i].Count;
                    int expectedSize = (int)Math.Ceiling((double)points.Count / (double)createdClusters.Count); // awful, trying to round up, seems to work
                    Debug.Log("expected Size" + expectedSize);
                    if (clusterSize > expectedSize) return false;
                }

                return true;
            }

            void RedistributeClusters() // ClustersEvenlyDistributedy is alread finding clusters that are too large so change this later
            {
                int expectedSize = (int)Math.Ceiling((double)points.Count / (double)createdClusters.Count); // awful, trying to round up, seems to work

                List<Vector3> largestCluster = createdClusters[0];
                for (int i = 1; i < createdClusters.Count; i++)
                {
                    if (createdClusters[i].Count > largestCluster.Count)
                    {
                        largestCluster = createdClusters[i];
                    }
                }

                float distanceToCluster = 99999999;
                int pointToMove = 0;

                List<Vector3> newCluster = new List<Vector3>();

                for (int i = 0; i < createdClusters.Count; i++)
                {
                    int clusterSize = createdClusters[i].Count;

                    if (clusterSize < expectedSize)
                    {
                        Vector3 averageVectorNewCluster = AverageVectorOfCluster(createdClusters[i]);

                        for (int j = 0; j < largestCluster.Count; j++)
                        {
                            float newDistance = Vector3.Distance(largestCluster[j], averageVectorNewCluster);
                            if (newDistance < distanceToCluster)
                            {
                                newCluster = createdClusters[i];
                                distanceToCluster = newDistance;
                                pointToMove = j;
                            }
                        }
                    }
                }

                if (newCluster.Count > 0)
                {
                    newCluster.Add(largestCluster[pointToMove]);
                    largestCluster.RemoveAt(pointToMove);
                }
                else Debug.LogError("no new cluster found");//error
            }

            Vector3 AverageVectorOfCluster(List<Vector3> clusterToAverage)
            {
                Vector3 averageVector = Vector3.zero;
                for (int i = 0; i < clusterToAverage.Count; i++)
                {
                    averageVector += clusterToAverage[i];
                }

                averageVector /= clusterToAverage.Count;
                return averageVector;
            }

            void InitializeCentroids()
			{
				for (int i=0; i<centroids.Length; i++)
				{
					centroids[i] = new Vector3
					{
						x = UnityEngine.Random.Range(bounds.x, bounds.width),
						y = 0, 
						z = UnityEngine.Random.Range(bounds.y, bounds.height),
					};
				}
			}
	
			void AssignPointsToCentroids()
			{
				for (int i=0; i<points.Count; i++)
				{
					Vector3 point = points[i];
					pointCentroidIndex[i] = centroids
						.Zip(Enumerable.Range(0, centroids.Length), Tuple.Create)
						.Aggregate((acc, val) => Vector3.Distance(point, acc.Item1) < Vector3.Distance(point, val.Item1) ? acc : val)
						.Item2;
				}
			}

			bool MoveCentroids()
			{
				bool moved = false;
				Vector3[] sums = new Vector3[centroids.Length];
				int[] counts = new int[centroids.Length];
				
				// for each point, add itself to the sum of the centroid it belongs to.
				for (int i=0; i<points.Count; i++)
				{
					int centroidIndex = pointCentroidIndex[i];
					sums[centroidIndex] += points[i];
					counts[centroidIndex] += 1;
				}

				// divide each sum by the amount of points in that sum, giving the average.
				for (int i=0; i<sums.Length; i++)
				{
					// no points are assigned to this centroid
					if (counts[i] == 0)
					{
 						// so assign sum to random position within bounds
						sums[i] = new Vector3
						{
							x = UnityEngine.Random.Range(bounds.x, bounds.width),
							y = 0, 
							z = UnityEngine.Random.Range(bounds.y, bounds.height),
						};
					}
					sums[i] /= counts[i];
				}

				// if any of the averaged points are different from the current centroids.
				bool isEqual = !centroids.Except(sums).Any();
				if (!isEqual)
				{
					moved = true;
					for (int i=0; i<sums.Length; i++)
					{
						// move the centroids closer to the averaged points
						if (Vector3.Distance(centroids[i], sums[i]) > 0.001f)
						{
							Vector3 old = centroids[i];
							centroids[i] = Vector3.Lerp(centroids[i], sums[i], stepPercentage);
						}
						else
						{
							centroids[i] = sums[i];
						}
					}
				}

				return moved;
			}

			List<List<Vector3>> CreateClusters()
			{
				List<List<Vector3>> clusters = new List<List<Vector3>>();
				for (int i=0; i<centroids.Length; i++)
				{
					clusters.Add(new List<Vector3>());
					for (int j=0; j<points.Count; j++)
					{
						if (pointCentroidIndex[j] == i)
						{
							clusters[i].Add(points[j]);
						}
					}
				}
				return clusters;
			}
		}

		private Rect CalculateBounds(List<Vector3> points)
		{
			IEnumerable<float> xCoordinates = points.Select(p => p.x);
			IEnumerable<float> zCoordinates = points.Select(p => p.z);
			return new Rect
			{
				x = xCoordinates.Min(),
				y = zCoordinates.Min(),
				width = xCoordinates.Max(),
				height = zCoordinates.Max(),
			};
		}
	}
}
