using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpImage : MonoBehaviour
{

    public UnityEngine.UI.Image PowerUp;
    public static PowerUpImage SharedInstance;
    // Start is called before the first frame update
    void Awake()
    {
        SharedInstance = this;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdatePowerUp(bool state)
    {
        PowerUp.enabled = state;
    }
}
