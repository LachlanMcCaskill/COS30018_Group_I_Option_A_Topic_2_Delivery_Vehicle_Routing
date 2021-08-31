using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    private Stop _intendedDestination;

    public Passenger(Stop destinationToSet)
    {
        _intendedDestination = destinationToSet;
    }
}
