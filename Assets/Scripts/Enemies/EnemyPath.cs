using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour {
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Knockback knockback;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (knockback.GettingKnockedBack) { return; }
        Move();

        
        if (moveDir.x < 0) {
            spriteRenderer.flipX = true;
        } else if (moveDir.x > 0) {
            spriteRenderer.flipX = false;
        }
    }

    public void MoveTo(Vector2 targetPosition) {
        moveDir = targetPosition;
    }

    public void StopMoving() {
        moveDir = Vector3.zero;
    }

    private void Move() {
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }
}
