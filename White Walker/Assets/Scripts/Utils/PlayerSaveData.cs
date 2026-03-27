using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public PlayerUnlockData unlockData = new();
    public PlayerOwnedData ownedData = new();
    public PlayerComboData comboData = new();
    public PlayerStatsData playerStats = new();
    public SettingsData settingsData = new();
}

[Serializable]
public class PlayerStatsData
{
    public int health = 100;
    public int level = 1;
    public int points = 200;
}

[Serializable]
public class PlayerComboData
{
    public List<ComboSet> equippedCombos = new();
}

[Serializable]
public class ComboSet
{
    public ComboSequence weaponType;
    public List<int> attackIDs = new();
}
[Serializable]
public class PlayerOwnedData
{
}

public class PlayerUnlockData
{
}

[Serializable]
public class SettingsData
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
}
