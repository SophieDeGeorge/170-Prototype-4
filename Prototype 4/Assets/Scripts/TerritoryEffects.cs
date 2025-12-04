using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryEffects : MonoBehaviour
{

    [SerializeField] private GameObject canvas;
    [SerializeField] private Q_Vignette_Single vignette;
    [SerializeField] private float neutral = 0.0f;
    [SerializeField] private float warning = 1.0f;
    [SerializeField] private float danger = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NeutralTerritory()
    {
        //camera.ResetReplacementShader();
        vignette.mainScale = neutral;
    }
    public void WarningTerritory()
    {
        //camera.RenderWithShader();
        vignette.mainScale = warning;
    }
    public void DangerTerritory()
    {
        //camera.RenderWithShader();
        vignette.mainScale = danger;
    }
}
