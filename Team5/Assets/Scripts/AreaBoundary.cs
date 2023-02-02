using UnityEngine;
using System.Collections;

public class AreaBoundary : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        ShipCombat ship = other.GetComponent<ShipCombat>();
        if (ship && ship.IsAlive())
        {
            ship.OutOfBounds = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ShipCombat ship = other.GetComponent<ShipCombat>();
        if (ship)
        {
            ship.OutOfBounds = false;
        }
    }
}
