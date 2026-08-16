using System;
using UnityEngine;
using SimpleSaveSystem;

namespace SimpleAudioSystem
{
    public class AudioSaveHandler : MonoBehaviour, ISaveable
    {
        [SerializeField, ShowOnly] private string byteGuid = Guid.NewGuid().ToString();
        
        public Guid guid => new Guid(byteGuid);

        public void RestoreState(SaveData state)
        {
        }
        public void CaptureState(ref SaveData saveData)
        {
        }
    }
}
