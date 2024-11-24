using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public float maxHp = 3;
    private PowerUpImage pu;
    private HealthImgController hc;
    public float point = 0;
    public bool doubleJump = false;
    // Start is called before the first frame update
    void Start()
    {
        pu = PowerUpImage.SharedInstance;
        health = maxHp;
        hc = HealthImgController.SharedInstance;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DoubleJumpState(bool _state)
    {
        if (_state == true) doubleJump = true;

        else if (_state == false) doubleJump = false;
        pu.UpdatePowerUp(_state);

    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        hc.RemoveHeart();
    }
}
