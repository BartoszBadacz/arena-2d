using System;
using UnityEngine;

namespace Arena.Data
{
    // Jeden „sygnał dźwiękowy” gry (np. strzał): kilka wariantów do losowania, głośność i rozrzut wysokości tonu,
    // żeby powtarzany dźwięk nie brzmiał mechanicznie. Pusta lista klipów = cisza (bez błędu).
    [Serializable]
    public class SoundCue
    {
        [SerializeField] private AudioClip[] clips = Array.Empty<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [Tooltip("Losowe odchylenie wysokości tonu w obie strony, np. 0.08 = ±8%.")]
        [SerializeField, Range(0f, 0.5f)] private float pitchJitter;

        public AudioClip[] Clips => clips;
        public float Volume => volume;
        public float PitchJitter => pitchJitter;
    }
}
