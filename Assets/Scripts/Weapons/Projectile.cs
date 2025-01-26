using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particlePrefabOnHitVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;

    private UnityEngine.Vector3 startPosition;

    private void Start() {
        startPosition = this.transform.position; 
    }

    private void Update() {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateProjectileRange(float projectileRange) {
        this.projectileRange = projectileRange;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        Indestructible indestructible= other.gameObject.GetComponent<Indestructible>();

        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

        if (!other.isTrigger && (enemyHealth || indestructible || playerHealth)) {
            if ((playerHealth && isEnemyProjectile) || (enemyHealth && !isEnemyProjectile)) {
                playerHealth?.TakeDamage(1, transform);

                Instantiate(particlePrefabOnHitVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            } else if (!other.isTrigger && indestructible) {
                Instantiate(particlePrefabOnHitVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    private void DetectFireDistance() {
        if (UnityEngine.Vector3.Distance(startPosition, transform.position) > this.projectileRange) {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile() {
        this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
}
