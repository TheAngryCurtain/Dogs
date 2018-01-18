using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents
{
    public class UpdateScoreEvent : VSGameEvent
    {
        public float TotalScore;

        public UpdateScoreEvent(float score)
        {
            TotalScore = score;
        }
    }

    public class UpdateMessageEvent : VSGameEvent
    {
        public string Message;
        public float DisplayTime;
        public InGameHudScreen.eMessageAlignment Alignment;

        public UpdateMessageEvent(string message, float time, InGameHudScreen.eMessageAlignment alignment)
        {
            Message = message;
            DisplayTime = time;
            Alignment = alignment;
        }
    }

    public class UpdateMissedCatchEvent : VSGameEvent
    {
        public int Misses;

        public UpdateMissedCatchEvent(int misses)
        {
            Misses = misses;
        }
    }

    public class UpdateHydrationEvent : VSGameEvent
    {
        public float HydrationPercent;

        public UpdateHydrationEvent(float percent)
        {
            HydrationPercent = percent;
        }
    }

    public class UpdateTimeRemainingEvent : VSGameEvent
    {
        public int SecondsRemaining;

        public UpdateTimeRemainingEvent(int secs)
        {
            SecondsRemaining = secs;
        }
    }
}
