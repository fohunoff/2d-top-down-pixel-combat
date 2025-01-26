using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamingChangeDirectionTime = 2f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    private bool canAttack = true;

    private enum State {
        Roaming,
        Attacking,
    }

    private Vector2 roamPosition;
    private float timeRoaming = 0f;

    private State state;
    private EnemyPath enemyPath;

    private void Awake() {
        enemyPath = GetComponent<EnemyPath>();
        state = State.Roaming;
    }


    private void Start() {
        roamPosition = GetRoamingPosition();
    }

    private void Update() {
        MovementStateControl();
    }

    private void MovementStateControl() {
        switch (state) {
            default: 
            case State.Roaming:
                Roaming();
            break;

            case State.Attacking:
                Attacking();
            break;
        }
    }

    private void Roaming() {
        timeRoaming += Time.deltaTime;

        enemyPath.MoveTo(roamPosition);

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange) {
            state = State.Attacking;
        }

        if (timeRoaming > roamingChangeDirectionTime) {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking() {
         if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange) {
            state = State.Roaming;
        }

        if (attackRange != 0 && canAttack) {
            canAttack = false;
            (enemyType as IEnemy).Attack();

            if (stopMovingWhileAttacking) {
                enemyPath.StopMoving();
            } else {
                enemyPath.MoveTo(roamPosition);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector2 GetRoamingPosition() {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
