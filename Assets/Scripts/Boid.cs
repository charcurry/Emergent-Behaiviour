using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector2 velocity;
    [SerializeField] private BoidManager boidManager;

    [SerializeField] private float speed;  // Speed of the boid
    [SerializeField] private float rotationSpeed;  // Rotation speed
    [SerializeField] private float neighborDistance;  // Distance to consider other boids as neighbors
    [SerializeField] private float avoidanceDistance;  // Minimum distance to avoid other boids
    [SerializeField] private float maxForce;  // Maximum steering force

    void Start()
    {
        // Set random initial velocity
        velocity = transform.up * boidManager.speed;
        boidManager = FindObjectOfType<BoidManager>();  // Get reference to BoidManager
    }

    void Update()
    {
        speed = boidManager.speed;
        rotationSpeed = boidManager.rotationSpeed;
        neighborDistance = boidManager.neighborDistance;
        avoidanceDistance = boidManager.avoidanceDistance;
        maxForce = boidManager.maxForce;

        ApplyFlockingRules();
        Move();
        BounceOffBorders();
    }

    // Move the boid according to its velocity
    void Move()
    {
        // Limit the speed
        velocity = Vector2.ClampMagnitude(velocity, speed);

        // Update position and rotation
        transform.position += (Vector3)velocity * Time.deltaTime;
        if (velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Apply flocking rules: Separation, Alignment, Cohesion
    void ApplyFlockingRules()
    {
        Vector2 separation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        int neighborCount = 0;

        foreach (Boid otherBoid in boidManager.boids)
        {
            if (otherBoid != this)
            {
                float distance = Vector2.Distance(otherBoid.transform.position, transform.position);

                // Separation - steer to avoid crowding local flockmates
                if (distance < avoidanceDistance)
                {
                    separation -= (Vector2)(otherBoid.transform.position - transform.position);
                }

                // Alignment and Cohesion
                if (distance < neighborDistance)
                {
                    alignment += otherBoid.velocity;
                    cohesion += (Vector2)otherBoid.transform.position;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            alignment = Vector2.ClampMagnitude(alignment, maxForce);

            cohesion /= neighborCount;
            cohesion = (cohesion - (Vector2)transform.position).normalized;
            cohesion = Vector2.ClampMagnitude(cohesion, maxForce);
        }

        // Apply the calculated steering forces
        velocity += (separation.normalized + alignment + cohesion) * Time.deltaTime;
    }

    void BounceOffBorders()
    {
        Camera camera = Camera.main;
        Vector2 screenMin = camera.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        Vector2 newPosition = transform.position;
        Vector2 newVelocity = velocity;

        if (transform.position.x < screenMin.x || transform.position.x > screenMax.x)
        {
            newVelocity.x = -newVelocity.x;
        }

        if (transform.position.y < screenMin.y || transform.position.y > screenMax.y)
        {
            newVelocity.y = -newVelocity.y;
        }

        velocity = newVelocity;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, neighborDistance);  // Neighbor detection radius

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, avoidanceDistance);  // Avoidance distance
    //}
}
