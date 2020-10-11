using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstants;
using DG.Tweening;
using System;

public enum GameState
{
    Select,
    WindUp,
    PerformAttack,
    AttackComplete
}
public class GameManager : MonoBehaviour
{
    public List<GameObject> missileObjects;
    public List<CanvasGroup> misslePanels;

    public List<Mesh> missileMeshes;
    public List<Material> materials;

    public GameObject ragDoll;
    public CanvasGroup scrollGroup;
    List<GameObject> ragDollPool;

    GameState currentState = GameState.Select;
    int currentFire;
    Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = FindObjectOfType<Camera>().transform.parent;
        CreateRagdollPool();
        cameraTransform.transform.position = GameConstants.selectPos;
        cameraTransform.transform.rotation =  Quaternion.Euler(GameConstants.selectRot);
    }

    void Update()
    {
        OnProcessState(currentState);
    }

    // FSM region
    public void SwitchState(GameState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case GameState.Select:
                {
                    switchAllowed = newState == GameState.WindUp;
                }
                break;
            case GameState.WindUp:
                {
                    switchAllowed = newState == GameState.PerformAttack;
                }
                break;
            case GameState.PerformAttack:
                {
                    switchAllowed = newState == GameState.AttackComplete;
                }
                break;
            case GameState.AttackComplete:
                {
                    switchAllowed = newState == GameState.Select;
                }
                break;
        }

        if (switchAllowed)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(currentState);
        }
    }

    // Check entry to stateEnter
    void OnEnterState(GameState stateEnter)
    {
        switch (stateEnter)
        {
            case GameState.Select:
                {
                    scrollGroup.interactable = true;
                    scrollGroup.DOFade(1f, GameConstants.scrollFade);
                    cameraTransform.DOMove(GameConstants.selectPos, GameConstants.scrollFade);
                    cameraTransform.DORotate(GameConstants.selectRot, GameConstants.scrollFade).OnComplete(() =>
                    {

                    });
                }
                break;
            case GameState.WindUp:
                {
                    cameraTransform.DOMove(GameConstants.windUpPos, GameConstants.windUpTransition);
                    cameraTransform.DORotate(GameConstants.windUpRot, GameConstants.windUpTransition).OnComplete(() => {
                        SwitchState(GameState.PerformAttack);
                    });
                }
                break;
            case GameState.PerformAttack:
                {
                    missileObjects[currentFire].GetComponent<Projectile>().FireMyself();
                }
                break;
            case GameState.AttackComplete:
                {
                    cameraTransform.DOMove(GameConstants.completeAPos, GameConstants.windUpTransition);
                    cameraTransform.DORotate(GameConstants.competeARot, GameConstants.windUpTransition).OnComplete(() => {
                        SwitchState(GameState.Select);
                    });
                }
                break;
        }
    }
    void OnExitState(GameState stateExit)
    {
        switch (stateExit)
        {
            case GameState.Select:
                {
                    scrollGroup.interactable = false;
                    misslePanels[currentFire].DOFade(0f, GameConstants.scrollFade / 2).OnComplete(() => {
                        scrollGroup.DOFade(0f, GameConstants.scrollFade/2);
                        misslePanels[currentFire].gameObject.SetActive(false);
                    });
                }
                break;
            case GameState.WindUp:
                {

                }
                break;
            case GameState.PerformAttack:
                {

                }
                break;
            case GameState.AttackComplete:
                {
                }
                break;
        }
    }
    void OnProcessState(GameState stateProcess)
    {

        switch (stateProcess)
        {
            case GameState.Select:
                {

                }
                break;
            case GameState.WindUp:
                {

                }
                break;
            case GameState.PerformAttack:
                {
                    if (missileObjects[currentFire].transform != null)
                    {
                        cameraTransform.LookAt(missileObjects[currentFire].transform);
                    }
                }
                break;
            case GameState.AttackComplete:
                {
                }
                break;
        }
    }
    // FSM end region

    void CreateRagdollPool()
    {
        ragDollPool = new List<GameObject>();
        // Create RagDollPool
        for (int i = 0; i < GameConstants.RagDollCount; i++)
        {
            GameObject rgInstance = GameObject.Instantiate(ragDoll);
            rgInstance.transform.position = new Vector3(1000f, 1000f, 1000f);
            ragDollPool.Add(rgInstance);
            rgInstance.SetActive(false);
        }
    }

    public GameObject GetRagdollInstance()
    {
        GameObject rgInstance = ragDollPool[0];
        ragDollPool.Remove(rgInstance);
        rgInstance.SetActive(true);
        return rgInstance;
    }

    public void MissleDropdownValueChanged(int index, int changeIndex)
    {
        missileObjects[index].GetComponent<Projectile>().ChangeProjectileType((ProjectileType)changeIndex);
    }

    public void MaterialDropdownValueChanged(int index, int changeIndex)
    {
        missileObjects[index].GetComponent<Projectile>().ChangeProjectileMaterial((ProjectileColour)changeIndex);
    }

    public void FireAway(int index)
    {
        Debug.Log(currentFire);
        currentFire = index;
        SwitchState(GameState.WindUp);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }

    internal Color GetColor(ProjectileColour newColor)
    {
        switch (newColor)
        {
            case ProjectileColour.Blue:
                return Color.blue;
                break;
            case ProjectileColour.Green:
                return Color.green;
                break;
            case ProjectileColour.Grey:
                return Color.gray;
                break;
            case ProjectileColour.Orange:
                return new Color(1f, 0.5f, 0f);
                break;
            case ProjectileColour.Pink:
                return new Color(1f, 0.08f, 0.58f);
                break;
            case ProjectileColour.Red:
                return Color.red;
                break;
            case ProjectileColour.Yellow:
                return Color.yellow;
                break;
        }
        return Color.gray;
    }
}
