using System.Collections.Generic;
using UnityEngine;

namespace RouteSolver
{
	interface IRouteSolver
	{
		public List<Route> Solve(Vector3 start, List<Vector3> points, int vehicleCount);
	}
}
