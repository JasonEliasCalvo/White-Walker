using UnityEngine;

[CreateAssetMenu(fileName = "AtaqueDagaEspecial", menuName = "Ataques/Nuevo Ataque Daga Especial")]
public class AtaqueDagaEspecial : Ataque
{
    [Header("Tipo de ataque daga especial")]

    public TipoDagaEspecial tipo;
    private void OnEnable()
    {
        categoria = CategoriaAtaque.DagaEspecial;
    }
}
