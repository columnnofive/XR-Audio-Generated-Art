using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonAR.SonicMapping
{
    public static class AudioSourceExtensions
    {
        /// <summary>
        /// Sets a more realistic rollof on an audio source than the logarithmic.
        /// Method found here: https://forum.unity.com/threads/audio-realistic-sound-rolloff-tool.543362/
        /// </summary>
        /// <param name="audioSource"></param>
        public static void setRealisticRolloff(this AudioSource audioSource)
        {
            var animCurve = new AnimationCurve(
            new Keyframe(audioSource.minDistance, 1f),
            new Keyframe(audioSource.minDistance + (audioSource.maxDistance - audioSource.minDistance) / 4f, .35f),
            new Keyframe(audioSource.maxDistance, 0f));

            audioSource.rolloffMode = AudioRolloffMode.Custom;
            animCurve.SmoothTangents(1, .025f);
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animCurve);

            audioSource.dopplerLevel = 0f;
            audioSource.spread = 60f;
        }
    }
}