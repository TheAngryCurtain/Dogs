using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEvents
{
    public class OnPlayerSpawnedEvent : VSGameEvent
    {
        public GameObject PlayerObj;

        public OnPlayerSpawnedEvent(GameObject obj)
        {
            PlayerObj = obj;
        }
    }

    public class OnDiscSpawnedEvent : VSGameEvent
    {
        public GameObject DiscObj;

        public OnDiscSpawnedEvent(GameObject obj)
        {
            DiscObj = obj;
        }
    }

    public class OnGameStartEvent : VSGameEvent
    {
        public ModesScreen.eMode GameMode;
        public ModesScreen.eSubMode GameSubMode;

        public OnGameStartEvent(ModesScreen.eMode mode, ModesScreen.eSubMode subMode)
        {
            GameMode = mode;
            GameSubMode = subMode;
        }
    }

    public class OnGameModeResetEvent : VSGameEvent
    {
        public OnGameModeResetEvent()
        {

        }
    }

    public class OnLevelLoadedEvent : VSGameEvent
    {
        public int SceneID;

        public OnLevelLoadedEvent(int id)
        {
            SceneID = id;
        }
    }

    public class RequestSceneChangeEvent : VSGameEvent
    {
        public int SceneIndex;

        public RequestSceneChangeEvent(int index)
        {
            SceneIndex = index;
        }
    }

    public class DogCatchDiscEvent : VSGameEvent
    {
        public IDog.eActionType ActionType;

        public DogCatchDiscEvent(IDog.eActionType type)
        {
            ActionType = type;
        }
    }

    public class DogTouchGroundEvent : VSGameEvent
    {
        public bool HasDisc;
        public Vector3 LandPosition;

        public DogTouchGroundEvent(bool hasDisc, Vector3 pos)
        {
            HasDisc = hasDisc;
            LandPosition = pos;
        }
    }

    public class DiscThrownEvent : VSGameEvent
    {
        public DiscThrownEvent()
        {

        }
    }

    public class LaunchDiscEvent : VSGameEvent
    {
        public float MinDiscForce;
        public float MaxDiscForce;
        public float MaxDiscCurve;

        public LaunchDiscEvent(float minForce, float maxForce, float curve)
        {
            MinDiscForce = minForce;
            MaxDiscForce = maxForce;
            MaxDiscCurve = curve;
        }
    }

    public class ResetDiscEvent : VSGameEvent
    {
        public ResetDiscEvent()
        {

        }
    }

    public class DiscTouchGroundEvent : VSGameEvent
    {
        public DiscTouchGroundEvent()
        {

        }
    }
}
