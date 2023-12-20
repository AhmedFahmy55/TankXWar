using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum Map
{
    Default
}

[System.Serializable]
public enum GameMode
{
    Default
}

[System.Serializable]
public enum GameQueue
{
    Solo,
    Team
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerAuthID;
    public int PlayerScore;
    public GameData PlayerGamePreferences;
}

[System.Serializable]
public class GameData
{
    public Map map;
    public GameMode gameMode;
    public GameQueue queue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}
