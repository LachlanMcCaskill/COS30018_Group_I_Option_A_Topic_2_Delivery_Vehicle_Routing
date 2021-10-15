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

			//  return CreateClusters();

            List<List<Vector3>> createdClusters = CreateClusters();

            //  should probably create a field limiting the size of clusters based on agent capacities, for now i'm just making it so none of the clusters are
            //  larger than the number of points / the number of clusters rounded up (so cluster size 6 for 15 points and 3 clusters etc)
            int expectedSize = (int)Math.Ceiling((double)points.Count / (double)createdClusters.Count); // definitely a better way to do this

            while (!ClustersEvenlyDistributed());

            return createdClusters;

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

            Boolean ClustersEvenlyDistributed()
            {
                //  cannot have any clusters larger than than an even distribution of points, can change this for capacity later if you want
                for (int i = 0; i < createdClusters.Count; i++)
                {
                    if (createdClusters[i].Count > expectedSize)
                    {
                        RedistributeCluster(createdClusters[i]);
                        return false;   // return false to repeat the distribution check
                    }
                }

                return true;
            }

            void RedistributeCluster(List<Vector3> sendingCluster)
            {
                float distanceToCluster = float.MaxValue;
                int pointToMove = 0;

                List<Vector3> receivingCluster = new List<Vector3>();

                for (int i = 0; i < createdClusters.Count; i++)
                {
                    if (createdClusters[i].Count < expectedSize)    // find a cluster with space
                    {
                        Vector3 averageVectorNewCluster = AverageVectorOfCluster(createdClusters[i]);   // get the 'center' of the cluster so we can determine a distance

                        for (int j = 0; j < sendingCluster.Count; j++)
                        {
                            float newDistance = Vector3.Distance(sendingCluster[j], averageVectorNewCluster);   // find distance between points in the sendingCluster(which has too many points)

                            if (newDistance < distanceToCluster)    //  compare the distance of all points in the sending cluster to find the closest point to the receiving cluster
                            {
                                receivingCluster = createdClusters[i];
                                distanceToCluster = newDistance;
                                pointToMove = j;
                            }
                        }
                    }
                }

                if (receivingCluster.Count > 0) // check this isnt our empty list
                {
                    receivingCluster.Add(sendingCluster[pointToMove]);  // move the point from the sending to the receivng cluster
                    sendingCluster.RemoveAt(pointToMove);
                }
                else Debug.LogError("no receiving cluster found");
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
