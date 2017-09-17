using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents
{
    public class ScoreEvent : VSGameEvent
    {
        public int[] Scores;

        public ScoreEvent(int[] scores)
        {
            Scores = scores;
        }
    }

    public class SetEvent : VSGameEvent
    {
        public int[] Sets;

        public SetEvent(int[] sets)
        {
            Sets = sets;
        }
    }

    public class ServingEvent : VSGameEvent
    {
        public int Side;

        public ServingEvent(int side)
        {
            Side = side;
        }
    }
}
