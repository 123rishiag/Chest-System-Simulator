using ServiceLocator.Event;
using System;
using UnityEngine;

namespace ServiceLocator.Sound
{
    public class SoundService
    {
        // Private Variables
        private SoundConfig soundConfig;
        private AudioSource sfxSource;
        private AudioSource bgSource;

        // Private Services
        private EventService eventService;

        public SoundService(SoundConfig _soundConfig, AudioSource _sfxSource, AudioSource _bgSource,
            EventService _eventService)
        {
            // Setting Variables
            soundConfig = _soundConfig;
            sfxSource = _sfxSource;
            bgSource = _bgSource;

            // Setting Services
            eventService = _eventService;

            PlayBackgroundMusic(SoundType.BackgroundMusic, true);

            // Adding Listeners
            eventService.OnPlaySoundEffectEvent.AddListener(PlaySoundEffect);
        }

        ~SoundService()
        {
            // Removing Listeners
            eventService.OnPlaySoundEffectEvent.RemoveListener(PlaySoundEffect);
        }

        private void PlaySoundEffect(SoundType _soundType)
        {
            AudioClip clip = GetSoundClip(_soundType);
            if (clip != null)
            {
                sfxSource.clip = clip;
                sfxSource.PlayOneShot(clip);
            }
            else
                Debug.LogError("No Audio Clip selected.");
        }

        private void PlayBackgroundMusic(SoundType _soundType, bool _loopSound = true)
        {
            AudioClip clip = GetSoundClip(_soundType);
            if (clip != null)
            {
                bgSource.loop = _loopSound;
                bgSource.clip = clip;
                bgSource.Play();
            }
            else
                Debug.LogError("No Audio Clip selected.");
        }

        private AudioClip GetSoundClip(SoundType _soundType)
        {
            SoundData sound = Array.Find(soundConfig.soundList, item => item.soundType == _soundType);
            if (sound.soundClip != null)
                return sound.soundClip;
            return null;
        }
    }
}