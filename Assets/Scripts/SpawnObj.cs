using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObj : MonoBehaviour
{
    public BoxCollider[] spawnCoin;
    public BoxCollider[] spawnPower;
    private int coinAmount;
    private int powerUpAmount;

    void Start()
    {
        StartCoroutine(WaitForPoolAndSpawn());
    }

    IEnumerator WaitForPoolAndSpawn()
    {
        while (!ObjectPooling.SharedInstance.isInitialized)
        {
            yield return null;
        }

        coinAmount = ObjectPooling.SharedInstance.coinAmountToPool;
        powerUpAmount = ObjectPooling.SharedInstance.powerUpAmountToPool;
        SpawnItems();
    }

    public void SpawnItems()
    {
        // Spawn coins
        for (int i = 0; i < coinAmount; i++)
        {
            GameObject coin = ObjectPooling.SharedInstance.GetPooledObject("coin");
            if (coin != null && i < spawnCoin.Length) // Ensure index is within bounds
            {
                SpawnCoinPos(coin, spawnCoin[i].transform.position);
            }
        }

        // Spawn power-ups
        for (int i = 0; i < powerUpAmount; i++)
        {
            GameObject powerUp = ObjectPooling.SharedInstance.GetPooledObject("powerUp");
            if (powerUp != null && i < spawnPower.Length) // Ensure index is within bounds
            {
                SpawnPowerUpPos(powerUp, spawnPower[i].transform.position);
            }
        }
    }

    public void SpawnCoinPos(GameObject _gameObject, Vector3 spawnPosition)
    {
        _gameObject.transform.position = spawnPosition;
        _gameObject.SetActive(true);
    }

    public void SpawnPowerUpPos(GameObject _gameObject, Vector3 spawnPosition)
    {
        _gameObject.transform.position = spawnPosition;
        _gameObject.SetActive(true);
    }



}