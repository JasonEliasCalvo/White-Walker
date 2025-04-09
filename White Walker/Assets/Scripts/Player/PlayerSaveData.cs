using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSaveData
{
    public PlayerUnlockData unlockData = new();
    public PlayerComboData comboData = new();
    public PlayerStatsData playerStats = new();
    public SettingsData settingsData = new();
}

[Serializable]
public class PlayerStatsData
{
    public int health = 100;
    public int level = 1;
    public int experience = 0;
    public int money = 200;
}

[Serializable]
public class PlayerAttackState
{
    public AttackBase attack;
    public bool unlocked;
    public bool owned;
}

[Serializable]
public class PlayerComboData
{
    public Dictionary<WeaponType, List<int>> equippedCombos = new();
}

public class PlayerUnlockData
{
    public Dictionary<WeaponType, List<int>> unlockedAttacks = new ();
}

[Serializable]
public class SettingsData
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
}
