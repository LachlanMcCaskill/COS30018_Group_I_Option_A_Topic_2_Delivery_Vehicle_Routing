using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour
{
    private List<Passenger> _passengers;

    public void addPassengers(List<Passenger> newPassengers)
    {
        _passengers = newPassengers;
    }
}
