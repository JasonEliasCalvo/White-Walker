using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public CombatData datosCombate;
    public enum ArmaEquipada{ PuñosPatadas,Daga }

    private readonly Dictionary<(ArmaEquipada, int, bool), IList> combos = new();

    public List<FistKickAttack> ComboPuñosPatadasBoton1Normal;
    public List<FistKickAttack> ComboPuñosPatadasBoton2Normal;
    public List<FistKickAttack> ComboPuñosPatadasBoton1SiEstaDerribado;
    public List<FistKickAttack> ComboPuñosPatadasBoton2SiEstaDerribado;

    public List<DaggerAttack> ComboDagaNormal;
    public List<DaggerAttack> ComboDagaNormalSiEstaDerribado;

    private int comboContador = 0;
    private float tiempoUltimoGolpe;
    public const float tiempoMaxCombo = 1.5f;

    private bool enemigoEnElSuelo = false;
    private ArmaEquipada armaActual = ArmaEquipada.PuñosPatadas;

    void Start()
    {
        RegistrarCombos();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) CambiarArma(ArmaEquipada.PuñosPatadas);
        if (Input.GetKeyDown(KeyCode.E)) CambiarArma(ArmaEquipada.Daga);


        if (Input.GetKeyDown(KeyCode.J)) EjecutarCombo(1);
        if (Input.GetKeyDown(KeyCode.K)) EjecutarCombo(2);
    }

    private void RegistrarCombos()
    {
        combos[(ArmaEquipada.PuñosPatadas, 1, false)] = ComboPuñosPatadasBoton1Normal;
        combos[(ArmaEquipada.PuñosPatadas, 2, false)] = ComboPuñosPatadasBoton2Normal;
        combos[(ArmaEquipada.PuñosPatadas, 1, true)] = ComboPuñosPatadasBoton1SiEstaDerribado;
        combos[(ArmaEquipada.PuñosPatadas, 2, true)] = ComboPuñosPatadasBoton2SiEstaDerribado;

        combos[(ArmaEquipada.Daga, 1, false)] = ComboDagaNormal;
        //combos[(ArmaEquipada.Daga, 2, false)] = ;
        combos[(ArmaEquipada.Daga, 1, true)] = ComboDagaNormalSiEstaDerribado;
        //combos[(ArmaEquipada.Daga, 2, true)] = ;
    }

    private void CambiarArma(ArmaEquipada nuevaArma)
    {
        armaActual = nuevaArma;
        comboContador = 0;
        Debug.Log($"Arma actual: {armaActual}");
    }

    private void EjecutarCombo(int boton)
    {
        if (Time.time - tiempoUltimoGolpe > tiempoMaxCombo)
        {
            comboContador = 0;
        }

        if (combos.TryGetValue((armaActual, boton, enemigoEnElSuelo), out var comboActual) && comboActual != null)
        {
            if (comboContador >= comboActual.Count)
            {
                comboContador = 0;
            }

            if (comboContador < comboActual.Count)
            {
                EjecutarAtaqueGenerico(comboActual, comboContador);
                comboContador++;
            }

            tiempoUltimoGolpe = Time.time;
        }
    }

    private void EjecutarAtaqueGenerico(IList combo, int indice)
    {
        if (combo == null || indice >= combo.Count) return;

        var tipo = combo[indice];

        switch (tipo)
        {
            case FistKickAttack ataquePuño:
                EjecutarAtaque(ataquePuño);
                break;

            case DaggerAttack ataqueDaga:
                EjecutarAtaque(ataqueDaga);
                break;
        }
    }

    public void EjecutarAtaque(FistKickAttack ataque)
    {
        if (ataque == null) return;

        Debug.Log($"Ejecutando: {ataque.attackName}, Daño: {ataque.damage}");
        //if (ataque.knockdown && PuedeSerDerribado())
        //{
        //    enemigoEnElSuelo = true;
        //    Debug.Log("Enemigo derribado");
        //}
    }

    public void EjecutarAtaque(DaggerAttack ataque)
    {
        if (ataque == null) return;
        Debug.Log($"Ejecutando: {ataque.attackName}, Daño: {ataque.damage}");
    }

    private bool PuedeSerDerribado() => Random.value > 0.5f;

    public void ActualizarEstadoEnemigo(bool enSuelo)
    {
        enemigoEnElSuelo = enSuelo;
    }

}
