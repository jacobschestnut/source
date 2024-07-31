using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    void Awake()
    {
        var rt = new RenderTexture(Screen.width, Screen.height, 24);
        Shader.SetGlobalTexture("_DimensionWindow", rt);
        _camera.targetTexture = rt;
    }
}
