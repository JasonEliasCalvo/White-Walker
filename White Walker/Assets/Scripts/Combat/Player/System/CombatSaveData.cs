using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombatSaveData
{
    public List<int> unlockedFistKick = new();
    public List<int> unlockedDagger = new();
    public List<int> unlockedSpecialDagger = new();
    public List<int> unlockedGun = new();
    public List<int> unlockedFinishers = new();
}
