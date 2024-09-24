using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceController : MonoBehaviour
{

    [SerializeField] private float speed = 1.0f; // Speed of the circular motion
    [SerializeField] private float radius = 10.0f; // Radius of the circle

    public Vector3 targetPoint = Vector3.zero; // The point the light will always face
    private Vector3 initialPosition; // The initial position of the light source

    private float angle = 0.0f; // Angle in radians

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position
        initialPosition = transform.position;

        // Calculate the initial angle based on the initial position
        Vector3 offset = initialPosition - targetPoint;
        angle = Mathf.Atan2(offset.z, offset.x);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new position of the light source
        float x = Mathf.Cos(angle) * radius + targetPoint.x;
        float z = Mathf.Sin(angle) * radius + targetPoint.z;

        Vector3 newPosition = new Vector3(x, initialPosition.y, z);

        // Update the position of the light source
        transform.position = newPosition;

        // Make the light source face the target point
        transform.LookAt(targetPoint);

        // Increment the angle based on the speed and time
        angle += speed * Time.deltaTime;

        // Keep the angle within 0 to 2*PI to avoid overflow (optional)
        if (angle >= 2 * Mathf.PI) {
            angle -= 2 * Mathf.PI;
        }
    }

}
