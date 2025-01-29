using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoinPrefab;

    public void DropItems() {
        Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);
    }
}
