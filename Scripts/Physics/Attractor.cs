using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {

    public float radius;
    public float surfaceGravity;
    public static float G = 1e-4f;
    public static List<Attractor> Attractors;
    public Vector3 initalVelocity;

    public Rigidbody rb;
    public Transform body;
    public GameObject obj;

    void FixedUpdate () {
		foreach (Attractor attractor in Attractors)
			if (attractor != this)
				Attract(attractor);
	}

    void OnEnable() {
        if (Attractors == null)
            Attractors = new List<Attractor>();
        Attractors.Add(this);
    }

    void OnDisable() {
        Attractors.Remove(this);
    }
    
    void Start() {
        obj.SetActive(false);
        rb.AddForce(initalVelocity, ForceMode.Acceleration);
    }

    void OnValidate() {
        rb.mass = surfaceGravity * radius * radius / G;
        body.localScale = Vector3.one * radius;
    }

    void Attract(Attractor objToAttract) {
        Rigidbody rbToAttract = objToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.sqrMagnitude;
        
        if(distance == 0f)
            return;
        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / distance;
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }
}
