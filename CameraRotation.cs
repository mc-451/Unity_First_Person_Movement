// First attempt at a 3D FPS movement system for unity by mc.
// Use in conjunction with FPSPlayerPhysics.cs
// Designed for a player as a capsule of scale (1,1.75,1) using a physic material with 0 friction.
// Last update: 03/02/2022
 
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform player;
	public float mouseSensitivity = 2f;
    public float clampAngle = 85;
    public Vector3 cameraOffset; // For capsule of scale (1,1.75,1) use (0.1.25,0)

    private float x;
    private float y;
    private Vector3 rotateValue;
    private float xrot;
    private Vector3 rotateTemp;
    private float temp;

    void Update()
    {
        y = Input.GetAxis("Mouse X") * mouseSensitivity;
        x = Input.GetAxis("Mouse Y") * mouseSensitivity;
	   
		rotateValue = new Vector3(x, -y, 0);
		rotateTemp  = transform.eulerAngles - rotateValue;
		temp = Mathf.Sin(rotateTemp.x*Mathf.PI/180);
		temp = Mathf.Clamp(temp,-Mathf.Sin(clampAngle*Mathf.PI/180),Mathf.Sin(clampAngle*Mathf.PI/180));
		temp= 180*Mathf.Asin(temp)/Mathf.PI;

		if (temp<0)
		{
			temp=temp+360;
		}
		rotateTemp.x =temp;
		transform.eulerAngles=rotateTemp;
		transform.position = player.transform.position + cameraOffset;
    }
}