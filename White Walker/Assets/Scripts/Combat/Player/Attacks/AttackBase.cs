using System;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Claw, Sword, Gun }

public abstract class AttackBase : ScriptableObject
{
    [Header("Attack Data")]
    public int attackID;
    public string attackName;             
    public float damage;              
    public float duration;              
    public int cost;                
    public WeaponType category;      
    public AnimationClip animation;   
    public List<EffectData> effects = new();
}
