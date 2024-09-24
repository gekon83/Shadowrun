using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocController : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] public float patrolRadius = 5f;  // Radius around the initial position where the enemy will patrol
    [SerializeField] public float speed = 3f;         // Speed of the enemy's movement
    [SerializeField] public float stopDistance = 0.1f; // Distance to the target position at which the enemy will stop
    [SerializeField] public Vector2 areaMin;            // Minimum bounds of the area (x, y)
    [SerializeField] public Vector2 areaMax;            // Maximum bounds of the area (x, y)
    [SerializeField] public float impulseMultiplier = 10.0f; // Adjust this value to control the strength of the impulse

    private Rigidbody rb;

    private Vector3 initialPosition; // Initial position around which the enemy patrols
    private Vector3 targetPosition;  // Current target position

    private Vector3 lastPosition;
    private Vector3 directionOfMovement;
    [SerializeField] private float knockBackForce = 15f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        initialPosition = transform.position;
        lastPosition = transform.position;

        //target = null;

        SetNewTarget();
    }

    void FixedUpdate() {
        FindTarget();
        MoveTowardsTarget(targetPosition);
        DrawLineToTarget(targetPosition);

        Vector3 currentPosition = transform.position;
        directionOfMovement = (currentPosition - lastPosition).normalized;
        lastPosition = currentPosition;
    }

    private void FindTarget() {
        if (player != null && player.IsDetectable()) {
            targetPosition = player.transform.position;
        }
        else {
            if (Vector3.Distance(transform.position, targetPosition) < stopDistance) {
                SetNewTarget();
            }
        }
    }

    void SetNewTarget() {
        // Randomize a point within the patrol radius
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        //targetPosition = initialPosition + new Vector3(randomPoint.x, 0, randomPoint.y);
        targetPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

        targetPosition.x = Mathf.Clamp(targetPosition.x, areaMin.x, areaMax.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, areaMin.y, areaMax.y);
    }

    void MoveTowardsTarget(Vector3 position) {
        // Calculate direction and move towards the target position
        Vector3 direction = (position - transform.position).normalized;
        //transform.position += direction * speed * Time.deltaTime;

        float moveDistance = moveSpeed * Time.deltaTime;
        //Debug.Log(moveDistance);
        rb.AddForce(direction * moveDistance, ForceMode.Impulse);
    }

    void DrawLineToTarget(Vector3 pos) {
        // Draw a line from the light source to the game object
        Debug.DrawLine(pos, transform.position, Color.red);
    }

    private void OnTriggerEnter(UnityEngine.Collider other) {
        Debug.Log("Trigger entered by: " + other.name);
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null) {

            // Calculate the opposite impulse
            Vector3 oppositeImpulse = -rb.velocity * rb.mass * impulseMultiplier;

            // Apply the impulse
            rb.AddForce(oppositeImpulse, ForceMode.Impulse);

            player.KnockBack(knockBackForce, directionOfMovement);
            //player.ChangeHealth(-1);

            //Destroy(gameObject);
        }
    }

    /* //zigzag
    public float speed = 5f;           // Speed of the movement
    public float changeInterval = 1f;  // Time interval for direction change
    public float zigzagAmplitude = 1f; // Amplitude of the zigzag
    public Vector2 areaMin;            // Minimum bounds of the area (x, y)
    public Vector2 areaMax;            // Maximum bounds of the area (x, y)

    private Vector3 direction;         // Current movement direction
    private float timeSinceLastChange; // Time since the last direction change
    private Vector3 zigzagDirection;   // Zigzag direction component

    void Start() {
        ChangeDirection();
        zigzagDirection = Vector3.right; // Initial zigzag direction
    }

    void Update() {
        // Update time
        timeSinceLastChange += Time.deltaTime;

        // Change direction at regular intervals
        if (timeSinceLastChange >= changeInterval) {
            ChangeDirection();
            timeSinceLastChange = 0f;
        }

        // Apply zigzag movement
        Vector3 zigzag = zigzagDirection * Mathf.Sin(Time.time * speed) * zigzagAmplitude;
        Vector3 movement = (direction + zigzag) * speed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;

        // Constrain the new position within the specified area
        newPosition.x = Mathf.Clamp(newPosition.x, areaMin.x, areaMax.x);
        //newPosition.z = Mathf.Clamp(newPosition.y, areaMin.y, areaMax.y);
        newPosition.z = Mathf.Clamp(newPosition.z, areaMin.y, areaMax.y);

        // Update the position
        transform.position = newPosition;
    }

    void ChangeDirection() {
        // Randomize main movement direction
        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // Randomize zigzag direction
        zigzagDirection = new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0, 2) * 2 - 1).normalized;
    }/**/
}
