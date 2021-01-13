using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravity : MonoBehaviour{
    public Rigidbody rb;

    void FixedUpdate() => Gravity();

    void Gravity() {
        if(Attractor.Attractors == null) return;
        Vector3 strongestForce = Vector3.one;
        foreach(Attractor attractor in Attractor.Attractors) {
            Rigidbody attractingBody = attractor.rb;
 
            Vector3 direction = attractingBody.position - rb.position;
            float distance = direction.sqrMagnitude;

            float forceMagnitude = Attractor.G * (rb.mass * attractingBody.mass) / distance;
            Vector3 force = direction.normalized * forceMagnitude;
            if(force.sqrMagnitude > strongestForce.sqrMagnitude)
                strongestForce = force;
            rb.AddForce(force);
        }
        if(strongestForce.sqrMagnitude > 2 * rb.mass * rb.mass) {
            Vector3 gravityUp = -strongestForce.normalized;
            rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;
            rb.AddForce(-transform.up * 3 * rb.mass);
        }
    }
}
