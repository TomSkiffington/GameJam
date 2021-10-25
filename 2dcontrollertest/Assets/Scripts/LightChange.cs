using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightChange : MonoBehaviour
{
   float duration = 2.0f;
    Color color0 = Color.magenta;
    Color color1 = Color.blue;
    
    Light2D lt;

    void Start()
    {
        lt = GetComponent<Light2D>();
    }

    void Update()
    {
        // set light color
        

        float t = Mathf.PingPong(Time.time, duration) / duration;
        lt.color = Color.Lerp(color0,color1,t);
        lt.intensity =Mathf.Lerp(0.25f,0.5f,t);
    }
}