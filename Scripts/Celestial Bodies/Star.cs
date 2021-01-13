using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Attractor {
    public Planet[] planets;

    public Vector3 gridCoords;
    public Planet planetPrefab;

    public float solarLuminosity;
    public float solarLifetime;
    public float solarTemperature;
    public float solarMass;
    public float solarRadius;

    public Vector2 orbitalLimits;
    public Vector2 habitalZone;
    public float frostLine;

    void Start() {
        obj.SetActive(false);
        SetSolarMeasurements();
        rb.mass = surfaceGravity * radius * radius / G;
        body.localScale = Vector3.one * radius;
        CreateStarSystem();
        SetPlanetDistances();
        rb.AddForce(initalVelocity, ForceMode.Acceleration);
    }

    public void SetSolarMeasurements() {
        radius = Universe.solarRadi * solarRadius;
        surfaceGravity = 112.5f  * solarMass / (solarRadius * solarRadius);
        solarLuminosity = Mathf.Pow(solarMass, 3.5f);
        solarLifetime = solarMass / solarLuminosity;
        solarTemperature = Mathf.Pow(solarLuminosity / solarRadius / solarRadius, .25f);
        orbitalLimits = new Vector2(.1f * solarMass, 40f * solarMass);
        habitalZone = new Vector2(Mathf.Sqrt(solarLuminosity / 1.1f), Mathf.Sqrt(solarLuminosity / .53f));
        frostLine = 4.85f * Mathf.Sqrt(solarLuminosity);
    }

    public void SetPlanetDistances() {
        if(planets == null) return;
         for(int i = 0; i < planets.Length; i++) {
            planets[i].body.position = body.position + new Vector3(0, 0, planets[i].distanceFromStar);
            planets[i].initalVelocity = new Vector3(radius * Mathf.Sqrt(surfaceGravity / (radius + planets[i].distanceFromStar)), 0, 0);
        }
    }
    
    void CreateStarSystem() {
        planets = new Planet[Random.Range(0, 9)];
        float distFromStar = orbitalLimits.x;
        for(int i = 0; i < planets.Length; i++) {
            float mass = 1;
            float radius = 1;
            Planet planet = Instantiate(planetPrefab);
            planet.planetMass = mass;
            planet.planetRadius = radius;
            planet.distanceFromStar = distFromStar;
            distFromStar *= Random.Range(1.4f, 2f);
            planets[i] = planet;
        }
    }
}
