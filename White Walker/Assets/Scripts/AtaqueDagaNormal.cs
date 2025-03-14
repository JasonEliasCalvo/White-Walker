using UnityEngine;


[CreateAssetMenu(fileName = "AtaqueDagaNormal", menuName = "Ataques/Nuevo Ataque Daga Normal")]
public class AtaqueDagaNormal : Ataque
{
    [Header("Tipo de ataque daga normal")]
    public TipoDagaNormal tipo;

    private void OnEnable()
    {
        categoria = CategoriaAtaque.DagaNormal;
    }
}
