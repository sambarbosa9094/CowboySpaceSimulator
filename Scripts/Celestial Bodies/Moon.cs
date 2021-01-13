using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : Attractor {
    public float distanceFromPlanet;
    public float lunarMass;
    public float lunarRadius;

    public void SetPlanetMeasurements() {
        radius = lunarRadius * Universe.lunarRadi;
        surfaceGravity = 10.125f * lunarMass / (lunarRadius * lunarRadius);
    }
    
    void Start() {
        obj.SetActive(false);
        SetPlanetMeasurements();
        rb.mass = surfaceGravity * radius * radius / G;
        body.localScale = Vector3.one * radius;
        rb.AddForce(initalVelocity, ForceMode.Acceleration);
    }
}
