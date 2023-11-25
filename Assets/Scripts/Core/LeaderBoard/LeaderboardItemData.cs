using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct LeaderboardItemData : INetworkSerializable, IEquatable<LeaderboardItemData>
{

    public ulong ClientID;
    public FixedString32Bytes PlayerName;
    public int Score;


    public bool Equals(LeaderboardItemData other)
    {
        return ClientID == other.ClientID && PlayerName.Equals(other.PlayerName) && Score == other.Score;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Score);
    }
}
