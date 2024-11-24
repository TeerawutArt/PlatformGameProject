using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthImgController : MonoBehaviour
{
    private UnityEngine.UI.Image[] hp;
    public static HealthImgController SharedInstance;
    // Start is called before the first frame update

    void Awake()
    {
        SharedInstance = this;
    }
    void Start()
    {
        hp = GetComponentsInChildren<Image>();
        Debug.Log(hp.Last());
    }

    // Update is called once per frame

    public void RemoveHeart()
    {
        if (hp.Length > 0)
        {
            hp.Last().enabled = false;
            hp = hp.Take(hp.Length - 1).ToArray();
        }

    }

}
