using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particlePrefabOnHitVFX;

    private WeaponInfo weaponInfo;
    private UnityEngine.Vector3 startPosition;

    public void UpdateWeaponIno(WeaponInfo weaponInfo) {
        this.weaponInfo = weaponInfo;
    }

    private void Start() {
        startPosition = this.transform.position; 
    }

    private void Update() {
        MoveProjectile();
        DetectFireDistance();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        Indestructible indestructible= other.gameObject.GetComponent<Indestructible>();

        if (!other.isTrigger && (enemyHealth || indestructible)) {
            enemyHealth?.TakeDamage(weaponInfo.weaponDamage);
            Instantiate(particlePrefabOnHitVFX, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    private void DetectFireDistance() {
        if (UnityEngine.Vector3.Distance(startPosition, transform.position) > this.weaponInfo.weaponRange) {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile() {
        this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
}
