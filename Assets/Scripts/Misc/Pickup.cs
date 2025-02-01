using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pickups : MonoBehaviour
{

    private enum PickupType {
        GoldCoin,
        HealthGlobe,
        StaminaGlobe
    }

    [SerializeField] private PickupType pickupType;

    [SerializeField] private float pickupDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float accelartionRange = 0.4f;

    [Header("Spawn Settings")]
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float heightY = 1f;
    [SerializeField] private float popDuration = 1f;

    private Vector3 moveDirection;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        StartCoroutine(AnimationCurveSpawnRoutine());
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
            DetectPickupType();
            Destroy(gameObject);
        }
    }

    private IEnumerator AnimationCurveSpawnRoutine() {
        float randomX = transform.position.x + Random.Range(-1f, 1f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);

        Vector2 startPoint = transform.position;
        Vector2 endPoint = new Vector2(randomX, randomY);

        float timePassed = 0f;

        while (timePassed < popDuration) {
            timePassed += Time.deltaTime;

            float linearT = timePassed / popDuration; // от 0 до 1 -- коэффициент интерполяции
            float heightT = animationCurve.Evaluate(linearT); // высота, зависящая от кривой
            float height = Mathf.Lerp(0f, heightY, heightT); // от желаемой высоты до высоты в коэффициенте

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);
            yield return null;
        }
    }

    private void DetectPickupType() {
        switch (pickupType)
        {
            case PickupType.GoldCoin:
                EconomyManager.Instance.UpdateCurrentGold();
                break;

            case PickupType.HealthGlobe:
                PlayerHealth.Instance.Heal();
                break;

            case PickupType.StaminaGlobe:
                Stamina.Instance.RefreshStamina();
                break;
        }
    }
}
