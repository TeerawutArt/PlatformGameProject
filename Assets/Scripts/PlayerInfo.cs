using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public float maxHp = 3;
    private PowerUpImage pu;
    public float point = 0;
    public bool doubleJump = false;
    // Start is called before the first frame update
    void Start()
    {
        pu = PowerUpImage.SharedInstance;
        health = maxHp;

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
}
