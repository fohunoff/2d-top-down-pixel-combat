using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private void Update() {
        FaceMouse(); 
    }

    private void FaceMouse() {
        UnityEngine.Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        UnityEngine.Vector2 direction = transform.position - mousePosition; 

        transform.right = -direction;
    }
}
