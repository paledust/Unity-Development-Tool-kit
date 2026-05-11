using SimpleSaveSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameBasic.Debugger
{
    public class GameDebugger : MonoBehaviour
    {
        [SerializeField] private InputActionMap debugActions;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            debugActions.Enable();

            debugActions["restart"].performed += Debug_RestartLevel;
            debugActions["save"].performed += Debug_Save;
            debugActions["load"].performed += Debug_Load;
        #endif
        }
        void OnDestroy()
        {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            debugActions["restart"].performed -= Debug_RestartLevel;
            debugActions["save"].performed -= Debug_Save;
            debugActions["load"].performed -= Debug_Load;

            if(debugActions.enabled)debugActions.Disable();
        #endif
        }
    #region DEBUG ACTION
        void Debug_RestartLevel(InputAction.CallbackContext callback){
            if(callback.ReadValueAsButton()){
                Debug.Log("Test Restart Level");
                GameManager.Instance.RestartLevel();
            }
        }
        void Debug_Save(InputAction.CallbackContext callback)=>SaveManager.SaveGameState(0);
        void Debug_Load(InputAction.CallbackContext callback)=>SaveManager.LoadGameState(0);
    #endregion
    }
}
