using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public DatosCombate datosCombate;
    public enum ArmaEquipada{ PuñosPatadas,Daga }

    private readonly Dictionary<(ArmaEquipada, int, bool), IList> combos = new();

    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton1Normal;
    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton2Normal;
    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton1SiEstaDerribado;
    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton2SiEstaDerribado;

    public List<TipoDaga> ComboDagaNormal;
    public List<TipoDaga> ComboDagaNormalSiEstaDerribado;

    public List<TipoDagaEspecial> ComboDagaEspecial;
    public List<TipoDagaEspecial> ComboDagaEspecialSiEstaDerribado;

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
        combos[(ArmaEquipada.Daga, 2, false)] = ComboDagaEspecial;
        combos[(ArmaEquipada.Daga, 1, true)] = ComboDagaNormalSiEstaDerribado;
        combos[(ArmaEquipada.Daga, 2, true)] = ComboDagaEspecialSiEstaDerribado;
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
            case TipoPuñosPatadas puños:
                EjecutarAtaque(datosCombate.ObtenerAtaque(puños));
                break;

            case TipoDaga daga:
                EjecutarAtaque(datosCombate.ObtenerAtaque(daga));
                break;

            case TipoDagaEspecial dagaEspecial:
                EjecutarAtaque(datosCombate.ObtenerAtaque(dagaEspecial));
                break;
        }
    }

    public void EjecutarAtaque(AtaquePuñosPatadas ataque)
    {
        if (ataque == null) return;

        Debug.Log($"Ejecutando: {ataque.nombre}, Daño: {ataque.daño}");
        if (ataque.derriba && PuedeSerDerribado())
        {
            enemigoEnElSuelo = true;
            Debug.Log("Enemigo derribado");
        }
    }

    public void EjecutarAtaque(AtaqueDaga ataque)
    {
        if (ataque == null) return;
        Debug.Log($"Ejecutando: {ataque.nombre}, Daño: {ataque.daño}");
    }

    public void EjecutarAtaque(AtaqueDagaEspecial ataque)
    {
        if (ataque == null) return;
        Debug.Log($"Ejecutando Daga Especial: {ataque.nombre}, Daño: {ataque.daño}");
    }

    private bool PuedeSerDerribado() => Random.value > 0.5f;

    public void ActualizarEstadoEnemigo(bool enSuelo)
    {
        enemigoEnElSuelo = enSuelo;
    }

}
