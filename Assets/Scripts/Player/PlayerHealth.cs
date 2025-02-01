using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool IsDead { get; private set; }

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private Slider healthSlider;

    private int currentHealth;
    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;

    const string HEALTH_SLIDER_REF = "Health Slider";
    const string TOWN_SCENE_REF = "Town";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake() {
        base.Awake();

        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
    }

    private void Start() {
        IsDead = false;
        currentHealth = maxHealth;

        UpdateHealthSlider();
    }

    private void OnCollisionStay2D(Collision2D other) {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy) {
            // damage
            TakeDamage(1, other.gameObject.transform);
        }
    }

    public void Heal(int value = 1) {
        if (currentHealth < maxHealth) {
            currentHealth += value;
            UpdateHealthSlider();
        }

    }

    public void TakeDamage(int damage, Transform hitTransform) {
        if (!canTakeDamage) { return; }

        ScreenShakeManager.Instance.ShakeScreen();


        knockback.GetKnockBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());

        canTakeDamage = false;
        currentHealth -= damage;
        
        StartCoroutine(DamageRecoveryRoutine());

        CheckIfPlayerDeath();
        UpdateHealthSlider();
    }
    
    private void CheckIfPlayerDeath() {
        if (currentHealth <= 0 && !IsDead) {
            IsDead = true;
            currentHealth = 0;

            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            Destroy(ActiveWeapon.Instance.gameObject);

            StartCoroutine(DeathLoadSceneRoutine());
        }
    }

    private IEnumerator DeathLoadSceneRoutine() {
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
        Stamina.Instance.ReplenishStaminaOnDeath();
        
        SceneManager.LoadScene(TOWN_SCENE_REF);
    }

    private IEnumerator DamageRecoveryRoutine() {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateHealthSlider() {
        if (healthSlider == null) {
            healthSlider = GameObject.Find(HEALTH_SLIDER_REF).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
