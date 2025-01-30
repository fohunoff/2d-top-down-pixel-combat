using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth>
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private int currentHealth;
    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;

    protected override void Awake() {
        base.Awake();
        
        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
    }

    private void Start() {
        currentHealth = maxHealth;
    }

    private void OnCollisionStay2D(Collision2D other) {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy) {
            // damage
            TakeDamage(1, other.gameObject.transform);
        }
    }

    public void Heal(int value = 1) {
        currentHealth += value;
    }

    public void TakeDamage(int damage, Transform hitTransform) {
        if (!canTakeDamage) { return; }

        ScreenShakeManager.Instance.ShakeScreen();


        knockback.GetKnockBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());

        canTakeDamage = false;
        currentHealth -= damage;

        StartCoroutine(DamageRecoveryRoutine());
    }

    private IEnumerator DamageRecoveryRoutine() {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true; 
    }
}
