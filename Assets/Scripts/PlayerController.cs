using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; private set; }

    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private GameInput gameInput;

    private Rigidbody rb;
    private Light[] lights;

    public float gravity = -9.81f;
    [SerializeField] private float groundRaycastDistance = 0.55f;


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance!");
        }
        Instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        UpdateLightSources();
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleJump();
        
        //HandleInteractions();
    }

    private void HandleMovement() {

        if (IsGrounded()) {
                Vector2 inputVector = gameInput.GetMovementVectorNotmalized();

            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

            float moveDistance = moveSpeed * Time.fixedDeltaTime;
            float playerRadius = .7f;
            float playerHeight = 2f;
            bool canMove = true;
            //bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
            //Debug.Log("canMove: " + canMove);

            if (!canMove) {
                // Cannot move towards moveDirection

                // Attempt only X direction
                Vector3 moveDirectionX = new Vector3(moveDir.x, 0f, 0f).normalized;
                canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionX, moveDistance);

                if (canMove) {
                    moveDir = moveDirectionX;
                }
                else {
                    // Cannot move on X, attempt only Z direction
                    Vector3 moveDirectionZ = new Vector3(0f, 0f, moveDir.z).normalized;
                    canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, moveDistance);

                    if (canMove) {
                        moveDir = moveDirectionZ;
                    }
                    else {
                        // Cannot move in any direction
                    }
                }
            }

            if (canMove) {
                //transform.position += moveDir * moveDistance;
                rb.AddForce(moveDir * moveDistance, ForceMode.Impulse);
            }

            //isWalking = moveDir != Vector3.zero;

            //float rotateSpeed = 10f;
            //transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
        }
    }

    private void HandleJump() {
        if (gameInput.Jump() && IsGrounded()) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos() {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastDistance);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastDistance, Color.yellow);
    }
    void UpdateLightSources() {
        lights = FindObjectsOfType<Light>(); // Get all light sources in the scene
    }

    public bool IsDetectable() {
        if (IsInLight() && IsGrounded()) {
            return true;
        }
        return false;
    }

    private bool IsInLight() {
        foreach (Light lightSource in lights) {
            if (IsIlluminatedByLight(lightSource)) {
                DrawLineToLight(lightSource, Color.red);
                return true; // If illuminated by any light, return true
            } else {
                DrawLineToLight(lightSource, Color.gray);
            }
        }
        return false; // Not illuminated by any light
    }

    bool IsGrounded() {
        // Perform a raycast downwards to detect ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundRaycastDistance)) {
            // Check if the distance to the ground is less than or equal to the raycast distance
            if (hit.distance <= groundRaycastDistance) {
                rb.drag = 3.0f;
                return true;
            }
        }
        rb.drag = 0.2f;
        return false;
    }

    bool IsIlluminatedByLight(Light lightSource) {
        // Calculate direction from light source to the game object
        Vector3 directionToLight = (transform.position - lightSource.transform.position).normalized;

        // Calculate the distance to the light source
        float distanceToLight = Vector3.Distance(transform.position, lightSource.transform.position);
        //Debug.Log("distanceToLight: " + distanceToLight);

        // Check if there's a direct line of sight between the light source and the game object
        if (Physics.Raycast(lightSource.transform.position, directionToLight, out RaycastHit hit, distanceToLight)) {
            if (hit.collider.gameObject == gameObject) {
                return true; // No obstruction, the object is in the light
            }
        }

        return false; // There is an obstruction, the object is in the shadow
        /*

        // Calculate the direction opposite to the light direction
        Vector3 lightDirection = -lightSource.transform.forward;

        // Cast a ray from the player's position in the direction opposite to the light
        RaycastHit2D hit = Physics2D.Raycast(transform.position, lightDirection);

        // If the ray hits something, check if it has a renderer and is not transparent
        if (hit.collider != null) {
            return false;

            Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
            if (hitRenderer != null && hitRenderer.enabled) {
                // Check if the material's alpha value indicates it's opaque

                return false; // Player is in shadow
            }
        }

        return true; // No shadow found*/
    }
    /*
    bool IsIlluminatedByLight(Light lightSource) {
        // Directional light check
        if (lightSource.type == LightType.Directional) {
            Vector3 directionToLight = -lightSource.transform.forward;
            if (Physics.Raycast(transform.position, directionToLight, out RaycastHit hit)) {
                return hit.collider.gameObject == gameObject;
            }
        }
        else {
            // Point/Spot light check
            Vector3 directionToLight = (lightSource.transform.position - transform.position).normalized;
            float distanceToLight = Vector3.Distance(transform.position, lightSource.transform.position);
            if (Physics.Raycast(transform.position, directionToLight, out RaycastHit hit, distanceToLight)) {
                return hit.collider.gameObject == gameObject;
            }
        }
        return false; // There is an obstruction, the object is in the shadow
    }/**/

    /********************************************************************************************/
    void DrawLineToLight(Light lightSource, Color color) {
        // Draw a line from the light source to the game object
        Debug.DrawLine(lightSource.transform.position, transform.position, color);
    }

    /********************************************************************************************/
    public void KnockBack(float knockBackForce, Vector3 direction) {
        rb.AddForce(direction * knockBackForce, ForceMode.Impulse);
        //rb.AddForce(direction * knockBackForce, ForceMode.VelocityChange);
    }
    /********************************************************************************************/
}
