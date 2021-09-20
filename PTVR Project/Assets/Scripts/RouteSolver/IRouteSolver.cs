using System.Collections.Generic;
using UnityEngine;

namespace RouteSolver
{
	interface IRouteSolver
	{
		public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, int vehicleCount);
	}
}
