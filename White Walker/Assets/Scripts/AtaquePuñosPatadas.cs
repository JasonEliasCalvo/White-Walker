using UnityEngine;

[CreateAssetMenu(fileName = "AtaquePuñosPatadas", menuName = "Ataques/Nuevo Ataque Puños y Patadas")]
public class AtaquePuñosPatadas : Ataque
{
    [Space(2)]
    [Header("Tipo de ataque puños y patadas")]
    public TipoPuñosPatadas tipo;

    private void OnEnable()
    {
        categoria = CategoriaAtaque.PuñosPatadas;
    }
}
