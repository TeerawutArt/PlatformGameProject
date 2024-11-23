using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBallMove : MonoBehaviour
{
    public float swingSpeed = 60f; // ความเร็วในการแกว่ง
    public float maxAngle = 70f; // มุมสูงสุดในการแกว่ง
    public float updateStep = 1f; // จำนวนองศาที่หมุนต่อเฟรม

    private float currentAngle = 0f; // มุมปัจจุบัน
    private int direction = 1; // ทิศทางการหมุน: 1 = ขวา, -1 = ซ้าย
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // คำนวณมุมปัจจุบัน
        currentAngle += updateStep * direction * swingSpeed * Time.deltaTime;

        // ตรวจสอบว่าถึงมุมสูงสุดหรือยัง
        if (Mathf.Abs(currentAngle) >= maxAngle)
        {
            currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle); // จำกัดไม่ให้เกินมุมสูงสุด
            direction *= -1; // เปลี่ยนทิศทาง
        }

        // อัปเดตการหมุนแกน Z
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

    }
}
