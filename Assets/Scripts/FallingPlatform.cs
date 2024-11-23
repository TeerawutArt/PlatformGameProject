using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f; // เวลาที่ใช้รอก่อนพื้นเริ่มร่วง
    public float fallSpeed = 3f; // ความเร็วในการร่วง
    public float resetDelay = 1f; // เวลาที่ใช้รอเพื่อรีเซ็ตพื้นกลับมา
    public Vector3 fallOffset = new Vector3(0, -12, 0); // ระยะที่พื้นจะร่วงลงไป

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isTriggered = false;

    void Start()
    {
        // เก็บตำแหน่งเริ่มต้นของพื้น
        originalPosition = transform.position;
        targetPosition = originalPosition + fallOffset; // คำนวณตำแหน่งเป้าหมาย
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(HandlePlatform());
        }
    }

    IEnumerator HandlePlatform()
    {

        yield return new WaitForSeconds(fallDelay);
        yield return StartCoroutine(FallDown());
        yield return new WaitForSeconds(resetDelay);
        transform.position = originalPosition;
        isTriggered = false;
    }

    IEnumerator FallDown()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null; // รอเฟรมถัดไป
        }
        transform.position = targetPosition;
    }
}
