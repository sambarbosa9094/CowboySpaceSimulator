using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
    float MaxRange {get;}

    void OnStartHover();
    void OnInteract();
    void OnEndHover();
}
