using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSPDispaly : MonoBehaviour
{
    private float fps;
    public TMPro.TextMeshProUGUI fpsText;
    void Start()
    {
        InvokeRepeating("GetFps", 0f, 1f);
    }

    void GetFps(){
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps.ToString();
    }

}
