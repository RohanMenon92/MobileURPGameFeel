using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;

public class MisslePanelUI : MonoBehaviour
{
    public int missleIndex;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ChangeMissleType(GetComponentsInChildren<TMP_Dropdown>()[0]);
        ChangeMaterialType(GetComponentsInChildren<TMP_Dropdown>()[1]);
    }

    public void ChangeMissleType(TMP_Dropdown change)
    {
        gameManager.MissleDropdownValueChanged(missleIndex, change.value);
    }

    public void ChangeMaterialType(TMP_Dropdown change)
    {
        GetComponent<Image>().DOColor(gameManager.GetColor((ProjectileColour)change.value), GameConstants.colorTransition);
        gameManager.MaterialDropdownValueChanged(missleIndex, change.value);
    }

    public void FireAway()
    {
        gameManager.FireAway(missleIndex);
    }
}
