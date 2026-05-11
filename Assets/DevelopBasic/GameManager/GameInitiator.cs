using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameBasic
{
    public interface IGameInitiatable
    {
        UniTask Init();
    }
    public class GameInitiator : MonoBehaviour
    {
        public async UniTask GameInit()
        {
        //Initialize Components and Systems
            var initiatables = GameService.FindComponentsOfType<IGameInitiatable>(false);
            try
            {
                foreach(var initiatable in initiatables)
                {
                    await initiatable.Init();
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"Game initiation failed: {ex}");
            }
        }
    }
}
