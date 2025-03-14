using UnityEngine;

[CreateAssetMenu(fileName = "DisparoPistola", menuName = "Ataques/Nuevo Disparo Pistola")]
public class AtaquePistola : Ataque
{
    [Space(2)]
    [Header("Tipo de ataque pistola")]

    public TipoPistola tipo;
    private void OnEnable()
    {
        categoria = CategoriaAtaque.Pistola;
    }
}
