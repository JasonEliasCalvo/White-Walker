using UnityEngine;

public abstract class Combate<TEnum>: ScriptableObject
{
    [Header("Datos del Ataque")]

    public TEnum tipo;
    public string nombre;
    public string animacionAtacante;
    public string animacionDefensor;
    public float rango;
    public float daño;
    public bool derriba;
}
