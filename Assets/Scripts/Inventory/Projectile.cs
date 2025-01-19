using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particlePrefabOnHitVFX;

    private void Update() {
        MoveProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        Indestructible indestructible= other.gameObject.GetComponent<Indestructible>();

        if (!other.isTrigger && (enemyHealth || indestructible)) {
            Instantiate(particlePrefabOnHitVFX, transform.position, transform.rotation);

            enemyHealth?.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    private void MoveProjectile() {
        this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
}
