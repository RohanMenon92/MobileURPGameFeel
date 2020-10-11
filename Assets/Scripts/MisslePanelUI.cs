using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MisslePanelUI : MonoBehaviour
{
    public int missleIndex;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void ChangeMissleType(TMP_Dropdown change)
    {
        gameManager.MissleDropdownValueChanged(missleIndex, change.value);
    }

    public void ChangeMaterialType(TMP_Dropdown change)
    {
        gameManager.MaterialDropdownValueChanged(missleIndex, change.value);
    }

    public void FireAway()
    {
        gameManager.FireAway(missleIndex);
    }
}
