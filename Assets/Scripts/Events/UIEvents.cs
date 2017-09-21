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
        public int Remaining;

        public UpdateMissedCatchEvent(int misses)
        {
            Remaining = misses;
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
}
