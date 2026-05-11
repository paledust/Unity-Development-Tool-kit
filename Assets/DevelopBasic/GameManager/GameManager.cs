using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleSaveSystem;

namespace GameBasic
{
    using Event;
    //Please make sure "GameManager" is excuted before every custom script
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private int targetFrameRate = 60;
    [Header("Scene Transition")]
        [SerializeField] private CanvasGroup BlackScreenCanvasGroup;
        [SerializeField] private float transitionDuration = 1;

    [Header("Init")]
        [SerializeField] private string InitScene;
        [SerializeField] private GameInitiator gameInitiator;
        [SerializeField] private bool loadInitSceneFromGameManager = false;

        private static bool isPaused = false;

        public bool IsSwitchingScene{get; private set;} = false;
        public string lastScene{get; private set;} = string.Empty;
        public string currentScene{get; private set;} = string.Empty;

        protected override async void Awake(){
            base.Awake();

            //帧数设置
            Application.targetFrameRate = targetFrameRate;

            //游戏存档初始化
            SaveManager.Initialize();
            SaveManager.LoadGameState(0);

            //游戏组件初始化
            if(gameInitiator == null)
            {
                Debug.LogError("未检测到GameInitiator组件，无法进行游戏初始化！");
                return;
            }
            await gameInitiator.GameInit();
            
            //场景加载
        #if UNITY_EDITOR
            if(loadInitSceneFromGameManager){
                BlackScreenCanvasGroup.alpha = 1;
                SwitchingScene(currentScene, InitScene);
            }
            else {
                currentScene = SceneManager.GetActiveScene().name;
            }
        #else
            SwitchingScene(currentScene, InitScene);
        #endif
        }

    #region GAME BASIC
        public void PauseTheGame(){
            if(isPaused) return;
            
            Time.timeScale = 0;
            AudioListener.pause = true;
            isPaused = true;
        }
        public void ResumeTheGame(){
            if(!isPaused) return;

            AudioListener.pause = false;
            Time.timeScale = 1;
            isPaused = false;
        }
        public void EndGame(){
            string currentLevel = SceneManager.GetActiveScene().name;
            StartCoroutine(EndGameCoroutine(currentLevel));
        }
        public void RestartLevel(){
            string currentLevel = SceneManager.GetActiveScene().name;
            StartCoroutine(RestartLevel(currentLevel));
        }
        public async void NewGame()
        {
            await SaveManager.NewGameState(0);
            SaveManager.LoadGameState(0);
            RestartLevel();
        }
        void OnApplicationQuit()
        {
            Debug.Log("GameManager: Application is quitting. Saving game...");
            SaveManager.SaveGameState(0);
        }
        #endregion

        #region Scene Transition
        public void SwitchingScene(string to, bool autosaveAfterTransition = true){
            string from = SceneManager.GetActiveScene().name;
            SwitchingScene(from, to, autosaveAfterTransition);
        }
        void SwitchingScene(string from, string to, bool autosaveAfterTransition = true){
            if(!IsSwitchingScene) StartCoroutine(SwitchSceneCoroutine(from, to, autosaveAfterTransition));
        }
        IEnumerator EndGameCoroutine(string level){
            StartCoroutine(FadeInBlackScreen(1f));
            yield return new WaitForSeconds(1f);
            GameBasicEvent.Call_BeforeUnloadScene();
            yield return SceneManager.UnloadSceneAsync(level);
            yield return new WaitForSeconds(1f);
            Application.Quit();
        }
        IEnumerator RestartLevel(string level){
            yield return FadeInBlackScreen(transitionDuration);
            IsSwitchingScene = true;
            //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
            GameBasicEvent.Call_BeforeUnloadScene();

            yield return SceneManager.UnloadSceneAsync(level);
            yield return null;
            //TO DO: do something after the last scene is unloaded.
            yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(level));
            //TO DO: do something after the next scene is loaded. e.g: call event of loading
            yield return null;
            yield return FadeOutBlackScreen(transitionDuration);
            GameBasicEvent.Call_AfterLoadScene();
            
            IsSwitchingScene = false;
        }
        IEnumerator SwitchSceneCoroutine(string from, string to, bool autosaveAfterTransition){
            IsSwitchingScene = true;
            if(from != string.Empty){
            //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
                lastScene = from;
                
                GameBasicEvent.Call_BeforeUnloadScene();
                yield return FadeInBlackScreen(transitionDuration);
                yield return SceneManager.UnloadSceneAsync(from);
            }
        //TO DO: do something after the last scene is unloaded.
            yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(to));
            currentScene = to;

        //TO DO: do something after the next scene is loaded. e.g: call event of loading
            GameBasicEvent.Call_AfterLoadScene();
        //AutoSave Game when transition to New Scene
            if(autosaveAfterTransition) SaveManager.SaveGameState(0);

            yield return null;
            yield return FadeOutBlackScreen(transitionDuration);

            IsSwitchingScene = false;
        }
        IEnumerator FadeInBlackScreen(float fadeDuration){
            float initAlpha = BlackScreenCanvasGroup.alpha;
            yield return new WaitForLoop(fadeDuration, (t)=>{
                BlackScreenCanvasGroup.alpha = Mathf.Lerp(initAlpha, 1, EasingFunc.Easing.QuadEaseOut(t));
            });
        }
        IEnumerator FadeOutBlackScreen(float fadeDuration){
            float initAlpha = BlackScreenCanvasGroup.alpha;
            yield return new WaitForLoop(fadeDuration, (t)=>{
                BlackScreenCanvasGroup.alpha = Mathf.Lerp(initAlpha, 0, EasingFunc.Easing.QuadEaseIn(t));
            });
        }
    #endregion
    }
}
