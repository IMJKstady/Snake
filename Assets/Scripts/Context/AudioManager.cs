using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;

namespace Snake
{
    public class AudioManager : MonoBehaviour, IRegulator
    {        
        public AudioSource audioSource;
        public AudioClip gameBgm;
        public AudioClip butttonEffect;
        public AudioClip victory;
        
        public float GetSoundEffect()
        {
            return PlayerPrefs.GetFloat("SoundEffect");
        }
        public void SetSoundEffect(float v)
        {
            PlayerPrefs.SetFloat("SoundEffect", v);
        }
        public float GetSoundBGM()
        {
            return PlayerPrefs.GetFloat("SoundBGM");
        }
        public void SetSoundBGM(float v)
        {
            PlayerPrefs.SetFloat("SoundBGM", v);
        }

        public void PlayBgm()
        {
            audioSource.volume = GetSoundEffect();
            audioSource.clip = gameBgm;
            audioSource.Play();
        }

        public void StopBgm()
        {
            if (audioSource.isPlaying && audioSource.clip == gameBgm)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
        
        public void PlayVictory()
        {
            audioSource.volume = GetSoundEffect();
            audioSource.clip = victory;
            audioSource.Play();
        }

        public void StopVictory()
        {
            if (audioSource.isPlaying && audioSource.clip == victory)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        public void PlayButtonEffect()
        {
            audioSource.volume = GetSoundEffect();
            audioSource.clip = butttonEffect;
            audioSource.Play();
        }

        public void Init()
        {

        }
    }
}