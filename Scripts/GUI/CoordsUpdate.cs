using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoordsUpdate : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI tooltipText;
    private Transform playerPos;

    private Vector3 coords;
    private Vector3 cachedCoords;

    public static Vector3 gridCoords;

    void Start() {
        playerPos = Camera.main.transform;
        coords = playerPos.position;
    }

    // Update is called once per frame
    void Update() {
        UpdateCoords();
        tooltipText.SetText("Position: " + coords.ToString());
    }

    void UpdateCoords() {
        Vector3 relativeCoords = playerPos.position;
        if(relativeCoords.sqrMagnitude > 250000)
            cachedCoords = coords;
        coords = cachedCoords + relativeCoords;
        gridCoords = coords / Universe.lightyear;
    }
}
