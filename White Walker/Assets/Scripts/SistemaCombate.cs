using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmaEquipada
{
    PuñosPatadas,
    Daga
}

public class SistemaCombate : MonoBehaviour
{
    public DatosCombate datosCombate;

    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton1Normal;
    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton2Normal;
    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton1SiEstaDerribado;
    public List<TipoPuñosPatadas> ComboPuñosPatadasBoton2SiEstaDerribado;

    public List<TipoDagaNormal> ComboDagaNormal;
    public List<TipoDagaNormal> ComboDagaNormalSiEstaDerribado;

    public List<TipoDagaEspecial> ComboDagaEspecial;
    public List<TipoDagaEspecial> ComboDagaEspecialSiEstaDerribado;

    private int comboContador = 0;
    private float tiempoUltimoGolpe;
    public const float tiempoMaxCombo = 1.5f;

    private bool enemigoEnElSuelo = false;
    private ArmaEquipada armaActual = ArmaEquipada.PuñosPatadas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) CambiarArma(ArmaEquipada.PuñosPatadas);
        if (Input.GetKeyDown(KeyCode.E)) CambiarArma(ArmaEquipada.Daga);

        if (Input.GetKeyDown(KeyCode.J))
        {
            EjecutarCombo(SeleccionarCombo(1));
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            EjecutarCombo(SeleccionarCombo(2));
        }
    }

    private void CambiarArma(ArmaEquipada nuevaArma)
    {
        armaActual = nuevaArma;
        comboContador = 0;
        Debug.Log($"Arma actual: {armaActual}");
    }

    private IList SeleccionarCombo(int boton)
    {
        switch (armaActual)
        {
            case ArmaEquipada.PuñosPatadas:
                if (enemigoEnElSuelo)
                {
                    return boton == 1 ? ComboPuñosPatadasBoton1SiEstaDerribado : ComboPuñosPatadasBoton2SiEstaDerribado;
                }
                return boton == 1 ? ComboPuñosPatadasBoton1Normal : ComboPuñosPatadasBoton2Normal;

            case ArmaEquipada.Daga:
                if (enemigoEnElSuelo)
                {
                    return boton == 1 ? ComboDagaNormalSiEstaDerribado : ComboDagaEspecialSiEstaDerribado;
                }
                return boton == 1 ? ComboDagaNormal : ComboDagaEspecial;

            default:
                return new List<TipoPuñosPatadas>();
        }
    }

    private void EjecutarCombo(IList comboActual)
    {
        if (Time.time - tiempoUltimoGolpe > tiempoMaxCombo)
        {
            comboContador = 0;
        }

        if (comboContador >= comboActual.Count)
        {
            comboContador = 0;
        }

        if (comboContador < comboActual.Count)
        {
            switch (armaActual)
            {
                case ArmaEquipada.PuñosPatadas:
                    if (comboActual is List<TipoPuñosPatadas> comboPuños)
                    {
                        var ataque = datosCombate.ObtenerAtaque(comboPuños[comboContador]);
                        EjecutarAtaque(ataque);
                    }
                    break;

                case ArmaEquipada.Daga:
                    if (comboActual is List<TipoDagaNormal> comboDagaNormal)
                    {
                        var ataque = datosCombate.ObtenerAtaque(comboDagaNormal[comboContador]);
                        EjecutarAtaque(ataque);
                    }
                    else if (comboActual is List<TipoDagaEspecial> comboDagaEspecial)
                    {
                        var ataque = datosCombate.ObtenerAtaque(comboDagaEspecial[comboContador]);
                        EjecutarAtaque(ataque);
                    }
                    break;
            }

            comboContador++;
        }


        tiempoUltimoGolpe = Time.time;
    }

    public void EjecutarAtaque(AtaquePuñosPatadas ataque)
    {
        if (ataque == null) return;

        if (ataque.derriba && PuedeSerDerribado())
        {
            Debug.Log($"Ejecutando: {ataque.nombre}, Derriba enemigo, Daño: {ataque.daño}");
            enemigoEnElSuelo = true;

        }
        else
        {
            Debug.Log($"Ejecutando: {ataque.nombre}, Daño: {ataque.daño}");
        }
    }

    public void EjecutarAtaque(AtaqueDagaNormal ataque)
    {
        if (ataque == null) return;

        Debug.Log($"Daga: {ataque.nombre}, Daño: {ataque.daño}");
    }
    private void EjecutarAtaque(AtaqueDagaEspecial ataque)
    {
        if (ataque == null) return;
        Debug.Log($"Ataque Daga Especial: {ataque.nombre}, Daño: {ataque.daño}");
    }

    private bool PuedeSerDerribado()
    {
        return Random.value > 0.5f;
    }

    public void ActualizarEstadoEnemigo(bool enAire, bool enSuelo)
    {
        enemigoEnElSuelo = enSuelo;
    }
}
