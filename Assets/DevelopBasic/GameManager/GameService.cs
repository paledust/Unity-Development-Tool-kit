using UnityEngine;
using System.Collections.Generic;

namespace GameBasic
{
    public static class GameService
    {
        public static T[] FindComponentsOfType<T>(bool includeInactive = true){
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            var MatchObjects = new List<T> ();

            for(int i=0; i<sceneCount; i++){
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);
                
                var RootObjects = scene.GetRootGameObjects ();

                foreach (var obj in RootObjects) {
                    var Matches = obj.GetComponentsInChildren<T> (includeInactive);
                    MatchObjects.AddRange (Matches);
                }
            }

            return MatchObjects.ToArray ();
        }
    }
}
