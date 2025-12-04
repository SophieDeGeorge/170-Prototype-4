// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class StaminaBar : MonoBehaviour 
// {
//     [Header("References")]
//     public PlayerMotor player;
//     public RectTransform uiBar;

//     private void Update()
//     {
//         if (player == null || uiBar == null)
//             return;

//         float t = Mathf.Clamp01(player.GetStaminaPercent());

//         Vector2 anchorMax = uiBar.anchorMax;
//         anchorMax.x = t;
//         uiBar.anchorMax = anchorMax;
//     }
// }


//     [Range(0, 4000)]
//     public int stamina;
//     public int maxStamina = 2000;

//     public RectTransform uiBar;

//     float percentUnit;
//     float staminaPercentUnit;

//     private void Update(){

//         if (stamina > maxStamina) stamina = maxStamina;
//         else if (stamina < 0) stamina = 0;

//         float currentStaminaPercent = stamina * staminaPercentUnit;
    
//         uiBar.anchorMax = new Vector2((currentStaminaPercent * percentUnit) / 100f, uiBar, anchorMax.y);
//     }

//     private void OnValidate() {
//         if (stamina > maxStamina) stamina = maxStamina;
//         else if (stamina < 0) stamina = 0;
//     }
// }