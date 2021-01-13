using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipController : MonoBehaviour, IInteractable {

    [SerializeField] private TextMeshProUGUI tooltipText;
    public LayerMask groundedMask;
 
    private const float maxRange = 8f;
    public float MaxRange{ get => maxRange;}

    private bool isPiloting = false;

    [SerializeField] private Rigidbody rb;
    private PlayerController player;
    public Transform camViewPoint;

    public float speed = 30f;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
    private float acceleration = 3f;
    public float thrustMultiplier = 10f;

    Quaternion targetRot;
    Quaternion smoothedRot;
    float verticalLookRotation;

    int numCollisionTouches;

    public void OnStartHover() {
        string text = isPiloting ? "Press E To Eject" : "Press E To Pilot";
        tooltipText.SetText(text);
    }

    public void OnInteract() {
        isPiloting = !isPiloting;
        if(isPiloting) TeleportToVehicle();
        else ExitVehicle();
    }

    public void OnEndHover() {
        tooltipText.SetText("");
    }

    public void Awake() {
        targetRot = transform.rotation;
        smoothedRot = transform.rotation;
    }

    public void Start() {
        player = FindObjectOfType<PlayerController>();
    }

    public void Update() {
        if(isPiloting)
            HandleMovement();
    }

    public void ExitVehicle() {
        player.gameObject.SetActive(true);
        player.transform.position = camViewPoint.position;
        player.cameraT.parent = player.transform;
    }

    public void TeleportToVehicle() {
        player.cameraT.parent = camViewPoint;
        player.cameraT.localPosition = Vector3.zero;
        player.cameraT.localRotation = Quaternion.identity;
        player.gameObject.SetActive(false);
    }
    
    public void HandleMovement() {
        float toggledThrust = 1;
        if(Input.GetKey("left ctrl"))
            toggledThrust = thrustMultiplier;
        
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed,
            Input.GetAxisRaw("Vertical") * speed * toggledThrust,
            acceleration * Time.deltaTime * toggledThrust);
        
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed,
            Input.GetAxisRaw("Horizontal") * speed * toggledThrust,
            acceleration * Time.deltaTime * toggledThrust);
        
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed,
            Input.GetAxisRaw("Hover") * speed * toggledThrust,
            acceleration * Time.deltaTime * toggledThrust);

        transform.position += (transform.forward * activeForwardSpeed * Time.deltaTime) + 
            (transform.right * activeStrafeSpeed * Time.deltaTime) +
            (transform.up * activeHoverSpeed * Time.deltaTime);
        
        float yawInput = Input.GetAxisRaw ("Mouse X") * speed * Time.deltaTime;
        float pitchInput = Input.GetAxisRaw ("Mouse Y") * speed * Time.deltaTime;
        float rotationInput = GetInputAxis(KeyCode.Z, KeyCode.C) * speed * Time.deltaTime;

        if(numCollisionTouches == 0) {
            Quaternion yaw = Quaternion.AngleAxis(yawInput, transform.up);
            Quaternion pitch = Quaternion.AngleAxis(-pitchInput, transform.right);
            Quaternion roll = Quaternion.AngleAxis(-rotationInput, transform.forward);

            targetRot = yaw * pitch * roll * targetRot;
            transform.rotation = targetRot;
            smoothedRot = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);

            rb.MoveRotation(smoothedRot);
        }
        else {
            targetRot = transform.rotation;
            smoothedRot = transform.rotation;

            transform.Rotate(Vector3.up * yawInput);
            verticalLookRotation += pitchInput;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
            player.cameraT.localEulerAngles = verticalLookRotation * Vector3.left;
        }
    }

    int GetInputAxis (KeyCode negativeAxis, KeyCode positiveAxis) {
        int axis = 0;
        if (Input.GetKey(positiveAxis)) axis++;
        if (Input.GetKey(negativeAxis)) axis--;
        return axis;
    }

    void OnCollisionEnter (Collision other) {
        if (groundedMask == (groundedMask | (1 << other.gameObject.layer)))
            numCollisionTouches++;
    }

    void OnCollisionExit (Collision other) {
        if (groundedMask == (groundedMask | (1 << other.gameObject.layer)))
            numCollisionTouches--;
    }
}