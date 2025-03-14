using UnityEngine;

public abstract class Ataque : ScriptableObject
{
    [Header("Datos del Ataque")]

    [HideInInspector]
    public CategoriaAtaque categoria;

    public string nombre;
    public string animacionAtacante;
    public string animacionDefensor;
    public float rango;
    public float daño;
    public bool derriba;
}
