using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    [Header("Shader")]
    public Material DefaultMaterial;
    public Material FlashMaterial;

    [Header("Enemy Settings")]
    public Color unaggroColor;
    public Color aggroColor;
    public Color attackColor;

    private void Awake()
    {
        instance = this;
    }


}
