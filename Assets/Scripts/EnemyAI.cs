using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    private enum State {
        Roaming,
    }

    private State state;
    private EnemyPath enemyPath;

    private void Start() {
        StartCoroutine(RoamingRoutine());
    }

    private void Awake() {
        enemyPath = GetComponent<EnemyPath>();
        state = State.Roaming;
    }

    private IEnumerator RoamingRoutine() {
        while (state == State.Roaming) {
            Vector2 roamPosition = GetRoamingPosition();
            enemyPath.MoveTo(roamPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition() {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
