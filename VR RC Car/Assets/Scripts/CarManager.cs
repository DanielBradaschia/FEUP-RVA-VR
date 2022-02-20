using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CarManager : MonoBehaviour
{
    GameObject currentCar;

    //Get all cars
    void Start()
    {
        currentCar = GameObject.FindGameObjectsWithTag("Car")[0];
        currentCar.GetComponent<CarControl>().enabled = true;
    }

    public void SwapCar(GameObject car)
    {
        currentCar.GetComponent<CarControl>().enabled = false;

        currentCar = car;

        currentCar.GetComponent<CarControl>().enabled = true;
    }
    
}
