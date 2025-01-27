using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    [Space(20)]

    [SerializeField] private float bulletMoveSpeed = 0f;
    [SerializeField] private float startingDistance = 0.1f;
    [SerializeField] private int burstCount;
    [SerializeField] private int projectilesPerBurst = 1;
    [SerializeField][Range(0, 359)] private float angleSpread;
    [Space(20)]

    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float restTime = 1f;
    [Space(10)]

    [Header("Some variants")]
    [SerializeField] private bool stagger;
    [Tooltip("Stagger has to be enable for oscillate to work properly.")]
    [SerializeField] private bool oscillate;

    private bool isShooting = false;

    private void OnValidate() {
        if (bulletMoveSpeed < 0f) { bulletMoveSpeed = 0.1f; }

        if (burstCount < 1) { burstCount = 1; }
        if (projectilesPerBurst < 1) { projectilesPerBurst = 1; }

        if (angleSpread == 0f) { projectilesPerBurst = 1; }

        if (startingDistance < 0.1f) { startingDistance = 0.1f; }
        if (timeBetweenBursts < 0.1f) { timeBetweenBursts = 0.1f; }
        if (restTime < 0.1f) { restTime = 0.1f; }


        if (oscillate) { stagger = true; }
        if (!oscillate) { stagger = false; }
    }

    public void Attack() {
        if (!isShooting) {
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;

        float startAngle, currentAngle, angleStep, endAngle;
        float timeBetweenProjectiles = 0f;

        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);

        if (stagger) {
            timeBetweenProjectiles = timeBetweenBursts / projectilesPerBurst;
        }

        for (int i = 0; i < burstCount; i++)
        {
            if (!oscillate) {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }
            
            if (oscillate && i % 2 != 0) {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            } else if (oscillate) {
                currentAngle = endAngle;

                endAngle = startAngle;
                startAngle = currentAngle;
                angleStep *= -1;
            }

            for (int j = 0; j < projectilesPerBurst; j++)
            {
                Vector2 position = FindBulletSpawnPosition(currentAngle);

                GameObject newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
                newBullet.transform.right = newBullet.transform.position - transform.position;

                if (newBullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.UpdateMoveSpeed(bulletMoveSpeed);
                }

                currentAngle += angleStep;

                if (stagger) {
                    yield return new WaitForSeconds(timeBetweenProjectiles);
                }
            }

            currentAngle = startAngle;

            if (!stagger) {
                yield return new WaitForSeconds(timeBetweenBursts);
            }
        }

        yield return new WaitForSeconds(restTime);
        isShooting = false;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - this.transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        startAngle = targetAngle;
        endAngle = targetAngle;
        currentAngle = targetAngle;
        angleStep = 0f;

        float halfAngleSpread = 0f;
        
        if (angleSpread != 0f) {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2f;

            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;

            currentAngle = startAngle;
        }
    }

    private Vector2 FindBulletSpawnPosition(float currentAngle) {
        float x = transform.position.x + startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 position = new Vector2(x, y);

        return position;
    }
}
