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

		public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, int vehicleCount)
		{
			List<List<Vector3>> clusters = GroupPointsIntoClusters(points, vehicleCount);
			return clusters.Select(cluster => _innerSolver.Solve(start, cluster, 1)[0]).ToList();
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

			return CreateClusters();

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
