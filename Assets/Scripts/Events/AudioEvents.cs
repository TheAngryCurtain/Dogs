using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvents
{
    public class RequestAudioEvent : VSGameEvent
    {
        public bool Play;

        public RequestAudioEvent(bool play)
        {
            Play = play;
        }
    }

    public class RequestLevelAudioEvent : RequestAudioEvent
    {
        public RequestLevelAudioEvent(bool play) : base(play)
        {

        }
    }

    public class RequestGameplayAudioEvent : RequestAudioEvent
    {
        public AudioManager.eGamePlayClip GameplayClip;

        public RequestGameplayAudioEvent(bool play, AudioManager.eGamePlayClip clip) : base(play)
        {
            GameplayClip = clip;
        }
    }

    public class RequestUIAudioEvent : RequestAudioEvent
    {
        public AudioManager.eUIClip UIClip;

        public RequestUIAudioEvent(bool play, AudioManager.eUIClip clip) : base(play)
        {
            UIClip = clip;
        }
    }
}
