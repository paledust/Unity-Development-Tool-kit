using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSaveSystem;

public class Tester : SaveableBehavior
{
    [SerializeField]
    private float saveData = 1;
    [SerializeField]
    private string sceneFrom;
    [SerializeField]
    private string sceneTo;
    void Awake(){
        SimpleSaveSystem.SaveManager.LoadGameState();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            GameManager.Instance.SwitchingScene(sceneFrom, sceneTo);
        }
        if(Input.GetKeyDown(KeyCode.S)){
            SimpleSaveSystem.SaveManager.SaveGameState();
        }
    }
    public override object CaptureState()
    {
        return saveData;
    }
    public override void RestoreState(object state)
    {
        saveData = (float)System.Convert.ToDouble(state);
    }
}
