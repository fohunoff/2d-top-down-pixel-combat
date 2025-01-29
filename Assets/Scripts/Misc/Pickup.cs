using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    [SerializeField] private float pickupDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float accelartionRange = 0.4f;

    private Vector3 moveDirection;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector3 playerPosition = PlayerController.Instance.transform.position;

        if (Vector3.Distance(transform.position, playerPosition) < pickupDistance) {
            moveDirection = (playerPosition - transform.position).normalized;
            moveSpeed += accelartionRange;
        } else {
            moveDirection = Vector3.zero;
            moveSpeed = 0f;
        }
    }

    private void FixedUpdate() {
        rb.velocity = moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<PlayerController>()) {
            Destroy(gameObject);
        }
    }
}
