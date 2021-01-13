using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Attractor {
    private Moon[] moons;

    public float distanceFromStar;
    public float planetMass;
    public float planetRadius;

    public void SetPlanetMeasurements() {
        radius = Universe.planetRadi * planetRadius;
        surfaceGravity = 10.125f * planetMass / (planetRadius * planetRadius);
    }

    public void SetMoonDistances() {
        if(moons == null) return;
        for(int i = 0; i < moons.Length; i++) {
            moons[i].body.position = body.position + new Vector3(0, 0, moons[i].distanceFromPlanet);
            moons[i].initalVelocity = new Vector3(radius * Mathf.Sqrt(surfaceGravity / (radius + moons[i].distanceFromPlanet)), 0, 0);
        }
    }
    
    void Start() {
        obj.SetActive(false);
        SetPlanetMeasurements();
        rb.mass = surfaceGravity * radius * radius / G;
        body.localScale = Vector3.one * radius;
        SetMoonDistances();
        rb.AddForce(initalVelocity, ForceMode.Acceleration);
    }

    void Update() {
        if(obj.activeSelf)
            for(int i = 0; i < moons.Length; i++)
                moons[i].obj.SetActive(true);
    }
}
