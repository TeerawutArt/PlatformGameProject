using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling SharedInstance;
    public Dictionary<string, List<GameObject>> pooledObjects;
    public GameObject coinPrefab;
    public GameObject powerUpPrefab;
    public GameObject collectableParent;
    public int coinAmountToPool = 10; //ไปตั้งเพิ่มเอา
    public int powerUpAmountToPool = 0;
    public bool isInitialized = false;
    private SpawnObj sc;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        sc = GetComponent<SpawnObj>();
        powerUpAmountToPool = sc.spawnPower.Length;
        pooledObjects = new Dictionary<string, List<GameObject>>();
        InitializePool("coin", coinPrefab, coinAmountToPool);
        InitializePool("powerUp", powerUpPrefab, powerUpAmountToPool);
        isInitialized = true;
    }

    // Method to initialize pools for each object type
    private void InitializePool(string key, GameObject prefab, int amount)
    {
        List<GameObject> objectPool = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            GameObject tmp = Instantiate(prefab, collectableParent.transform);
            ObjectController tmpObj = tmp.AddComponent<ObjectController>();
            tmpObj.index = i;
            tmp.SetActive(false);
            objectPool.Add(tmp);
        }
        pooledObjects[key] = objectPool;
    }

    public GameObject GetPooledObject(string key)
    {
        if (pooledObjects.ContainsKey(key))
        {
            foreach (GameObject obj in pooledObjects[key])
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
        }
        return null;
    }

    //ส่วนของ respawn collect ต่างๆ (ตอนนี้เหรียญไม่ได้ให้เกิดใหม่)
    public void ResetObject(GameObject obj, string key, int spawnIndex)
    {
        StartCoroutine(WaitAndReactivateObject(obj, key, spawnIndex));
    }

    private IEnumerator WaitAndReactivateObject(GameObject obj, string key, int spawnIndex)
    {
        yield return new WaitForSeconds(3);

        if (key == "coin")
        {
            if (spawnIndex >= 0 && spawnIndex < sc.spawnCoin.Length)
            {
                Vector3 spawnPosition = sc.spawnCoin[spawnIndex].transform.position;
                sc.SpawnCoinPos(obj, spawnPosition); // ส่งตำแหน่งให้ SpawnCoinPos
            }
        }
        else if (key == "powerUp")
        {
            if (spawnIndex >= 0 && spawnIndex < sc.spawnPower.Length)
            {
                Vector3 spawnPosition = sc.spawnPower[spawnIndex].transform.position;
                sc.SpawnPowerUpPos(obj, spawnPosition); // ส่งตำแหน่งให้ SpawnPowerUpPos
            }
        }
    }


}
