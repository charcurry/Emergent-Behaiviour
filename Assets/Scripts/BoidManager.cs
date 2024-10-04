using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoidManager : MonoBehaviour
{
    public Boid boidPrefab;  // Prefab of the boid
    public int boidCount = 50;  // Number of boids to spawn
    public float spawnRadius = 10.0f;  // Radius in which boids are spawned

    public float speed = 2.0f;  // Speed of the boid
    public float rotationSpeed = 4.0f;  // Rotation speed
    public float neighborDistance = 3.0f;  // Distance to consider other boids as neighbors
    public float avoidanceDistance = 1.0f;  // Minimum distance to avoid other boids
    public float maxForce = 0.5f;  // Maximum steering force
    
    // Sliders to change the parameters during the simulation
    public Slider speedSlider;
    public Slider rotationSlider;
    public Slider neighborSlider;
    public Slider avoidanceSlider;
    public Slider forceSlider;
    public Slider countSlider;

    public bool trails;  // Toggle trails on/off

    public bool showGizmo = false; // Toggle the visibility of the gizmo

    public List<Boid> boids = new List<Boid>();  // List of all boids

    void Start()
    {
        SpawnBoids();
        trails = true;
    }

    // Spawn boids at the start of the simulation
    void SpawnBoids()
    {
        // Spawn boids in a circle around the manager
        for (int i = 0; i < boidCount; i++)
        {
            // Random position within the spawn radius
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Boid newBoid = Instantiate(boidPrefab, spawnPosition, Quaternion.identity);
            boids.Add(newBoid);

            // Set the parameters for the boid trail
            TrailRenderer trail = newBoid.GetComponentInChildren<TrailRenderer>();
            if (trail != null)
            {
                trail.enabled = trails;
            }
        }
    }

    // Despawn all boids
    void DespawnBoids()
    {
        foreach (Boid boid in boids)
        {
            Destroy(boid.gameObject);
        }
        boids.Clear();
    }

    // Toggle trails on/off
    public void ToggleTrails()
    {
        trails = !trails;

        foreach (Boid boid in boids)
        {
            // Enable/disable the trail renderer
            TrailRenderer trail = boid.GetComponentInChildren<TrailRenderer>();
            if (trail != null)
            {
                trail.enabled = trails;
            }
        }
    }

    //Slider functions to change the parameters
    public void SpeedSlider()
    {
        speed = speedSlider.value * 10;
    }

    public void RotationSlider()
    {
        rotationSpeed = rotationSlider.value * 10;
    }
    public void NeighborSlider()
    {
        neighborDistance = neighborSlider.value * 10;
    }

    public void AvoidanceSlider()
    {
        avoidanceDistance = avoidanceSlider.value * 10;
    }
    public void ForceSlider()
    {
        maxForce = forceSlider.value * 10;
    }

    public void CountSlider()
    {
        boidCount = (int)countSlider.value;
        DespawnBoids();
        SpawnBoids();
    }

    // Reset the boids by despawning and respawning them
    public void ResetBoids()
    {
        DespawnBoids();
        SpawnBoids();
    }
    
    // Toggle the visibility of the gizmo
    public void ToggleGizmo()
    {
        showGizmo = !showGizmo;
    }
}