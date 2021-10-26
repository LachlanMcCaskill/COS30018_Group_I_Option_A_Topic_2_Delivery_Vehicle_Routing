using System.Collections.Generic;
using UnityEngine;

namespace RouteSolver
{
	public interface IRouteSolver
	{
		public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, List<TransportAgentIntroductionMessage> agentsWithCapacities, List<DestinationMessage> destinations);
	}
}
