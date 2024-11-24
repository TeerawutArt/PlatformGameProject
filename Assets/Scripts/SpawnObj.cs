using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObj : MonoBehaviour
{
    public BoxCollider[] spawnCoin;
    public BoxCollider[] spawnPower;
    public float spawnOffsetX = 1.5f; // ระยะห่างขั้นต่ำระหว่างเหรียญ
    private int coinAmount;
    private int powerUpAmount;
    private int randomIndex;
    private BoxCollider selectedBox;
    private List<Vector3> usedPositions = new List<Vector3>();

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
        // เก็บตำแหน่งที่ใช้แล้ว


        // Spawn coins
        for (int i = 0; i < coinAmount; i++)
        {
            GameObject coin = ObjectPooling.SharedInstance.GetPooledObject("coin");
            if (coin != null)
            {
                Vector3 spawnPosition = GetValidSpawnPosition(spawnCoin, usedPositions);
                if (spawnPosition != Vector3.zero)
                {
                    SpawnCoinPos(coin, spawnPosition);
                }
            }
        }


        for (int i = 0; i < powerUpAmount; i++)
        {
            GameObject powerUp = ObjectPooling.SharedInstance.GetPooledObject("powerUp");
            if (powerUp != null && i < spawnPower.Length) // Ensure index is within bounds
            {
                SpawnPowerUpPos(powerUp, spawnPower[i].transform.position);
            }
        }
    }

    private Vector3 GetValidSpawnPosition(BoxCollider[] spawnAreas, List<Vector3> usedPositions)
    {
        // พยายามสุ่มตำแหน่งใหม่ในพื้นที่ที่เลือก
        for (int attempt = 0; attempt < 10; attempt++) // ลองสุ่มไม่เกิน 10 ครั้ง
        {
            randomIndex = Random.Range(0, spawnAreas.Length);
            selectedBox = spawnAreas[randomIndex];
            Vector3 randomPosition = GetRandomPositionInBox(selectedBox);

            // ตรวจสอบว่าตำแหน่งในแกน x ไม่ใกล้กับตำแหน่งที่ใช้ไปแล้ว
            bool isValid = true;
            foreach (Vector3 usedPosition in usedPositions)
            {
                if (Mathf.Abs(randomPosition.x - usedPosition.x) < spawnOffsetX) // เช็คเฉพาะแกน x
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                usedPositions.Add(randomPosition); // บันทึกตำแหน่งที่ใช้
                return randomPosition;
            }
        }

        // ถ้าหาตำแหน่งที่เหมาะสมไม่ได้ ให้คืนค่า Vector3.zero
        return Vector3.zero;
    }

    private Vector3 GetRandomPositionInBox(BoxCollider box)
    {

        float randomX = Random.Range(box.bounds.min.x, box.bounds.max.x); //สุ่มในขอบเขตทั้งหมดของ box
        float randomY = Random.Range(box.bounds.min.y, box.bounds.max.y);
        float conZ = box.transform.position.z; //ไม่ rand

        return new Vector3(randomX, randomY, conZ);
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
