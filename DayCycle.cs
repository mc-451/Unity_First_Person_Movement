// Customisable day-night cycle for unity by mc.
// Starting time of day given in 24 hour time.
// Positive x is north and positive z is west
// Last update: 03/02/2022

using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public Transform yourLightSource;
    public float realLifeTimeLengthOfCycleInMinutes = 10.0f;
    public float startingTimeHours = 8.0f;
    public float startingTimeMinutes = 30f;

	private float time = 0.0f;
    private Vector3 rotateValue;
    void Update()
    {
        time += Time.deltaTime;
        rotateValue = new Vector3(  ((startingTimeHours-6)%24)*15+(startingTimeMinutes%60)/4+(time*6f/(realLifeTimeLengthOfCycleInMinutes)), 0, 0);
        transform.eulerAngles = rotateValue;
    }
}
