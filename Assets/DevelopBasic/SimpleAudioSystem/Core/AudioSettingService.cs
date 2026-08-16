using UnityEngine;

namespace SimpleAudioSystem
{
    public enum AudioSettingType
    {
        Master,
        AMB,
        MUS,
        SFX
    }
    [System.Serializable]
    public class AudioSettingData
    {
        public float masterVolume;
        public float ambVolume;
        public float musVolume;
        public float sfxVolume;
        public static AudioSettingData DefaultSetting = new AudioSettingData();
        public AudioSettingData()
        {
            masterVolume = 1;
            ambVolume = 1;
            musVolume = 1f;
            sfxVolume = 1;
        }
    }
    public static class AudioSettingService
    {
        private static float _masterVolume = AudioSettingData.DefaultSetting.masterVolume;
        private static float _ambVolume = AudioSettingData.DefaultSetting.ambVolume;
        private static float _musVolume = AudioSettingData.DefaultSetting.musVolume;
        private static float _sfxVolume = AudioSettingData.DefaultSetting.sfxVolume;
        private const float MUS_SCALE = 0.6f;
        
        public static void SetAudioVolume(AudioSettingType audioSettingType, float volume)
        {
            if(AudioManager.Instance==null)
                return;
            volume = Mathf.Clamp01(volume);
            switch(audioSettingType)
            {
                case AudioSettingType.Master:
                    AudioManager.Instance.ChangeMasterVolume(volume);
                    _masterVolume = volume;
                    break;
                case AudioSettingType.AMB:
                    AudioManager.Instance.ChangeAMBVolume(volume);
                    _ambVolume = volume;
                    break;
                case AudioSettingType.MUS:
                    AudioManager.Instance.ChangeMUSVolume(volume*MUS_SCALE);
                    _musVolume = volume;
                    break;
                case AudioSettingType.SFX:
                    AudioManager.Instance.ChangeSFXVolume(volume);
                    _sfxVolume = volume;
                    break;
            }
        }
        public static float GetAudioVolume(AudioSettingType audioSettingType)
        {
            switch(audioSettingType)
            {
                case AudioSettingType.Master:
                    return _masterVolume;
                case AudioSettingType.AMB:
                    return _ambVolume;
                case AudioSettingType.MUS:
                    return _musVolume;
                case AudioSettingType.SFX:
                    return _sfxVolume;
            }
            return 1;
        }
    }
}
