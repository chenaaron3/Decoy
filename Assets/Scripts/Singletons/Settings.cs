using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public float waterAnimationSpeed;

    [Header("Shader")]
    public Material DefaultMaterial;
    public Material FlashMaterial;

    [Header("Enemy Settings")]
    public Color unaggroColor;
    public Color aggroColor;
    public Color attackColor;

    [Header("Map Settings")]
    public Color landColor;
    public Color wallColor;
    public Color waterColor;
    public Color bushColor;
    public Color fogColor;
    public Color playerColor;
    public Color enemyColor;

    [Header("Prefabs")]
    public GameObject oceanPrefab;
    public GameObject groundPrefab;
    public GameObject waterPrefab;
    public GameObject wallPrefab;
    public GameObject bushPrefab;

    private void Awake()
    {
        instance = this;
    }


}
