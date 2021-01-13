using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Rigidbody rb;

    public LayerMask groundedMask;
    
    public float sensitivity = 100f;
    public float walkSpeed = 8f;
    public float jumpForce = 300f;
    public float runSpeed = 12f;

    public Transform cameraT;
    float verticalLookRotation;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    bool grounded = false;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update() => HandleMovement();
    
    void FixedUpdate() => PlayerGravity();

    void HandleMovement() {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);
        verticalLookRotation += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraT.localEulerAngles = verticalLookRotation * Vector3.left;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		
		grounded = Physics.Raycast(ray, out hit, 1 + .1f, groundedMask);

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
        Vector3 targetMoveAmount;
        if(Input.GetKey("left ctrl"))
            targetMoveAmount =  moveDir * runSpeed;
        else
            targetMoveAmount =  moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

            if (grounded) {
                if (Input.GetButtonDown("Jump")) {
                    rb.AddForce(transform.up * jumpForce, ForceMode.Acceleration);
                    grounded = false;
                }
                else
                    rb.AddForce(-transform.up * 7 * rb.mass);
            }
    }

    void PlayerGravity() {
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
        }
        
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }
}
