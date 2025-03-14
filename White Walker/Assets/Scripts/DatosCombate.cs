using System.Collections.Generic;
using UnityEngine;

public class DatosCombate : MonoBehaviour
{
    public List<AtaquePuñosPatadas> ataquesPuñosPatadas;
    public List<AtaqueDagaNormal> ataquesDagaNormales;
    public List<AtaqueDagaEspecial> ataquesDagaEspeciales;
    public List<AtaquePistola> disparosPistola;
    public List<Remate> remates;
    public List<Provocacion> provocaciones;

    private Dictionary<TipoPuñosPatadas, AtaquePuñosPatadas> ataquePuñosPatadasDict;
    private Dictionary<TipoDagaNormal, AtaqueDagaNormal> ataqueDagaNormalDict;
    private Dictionary<TipoDagaEspecial, AtaqueDagaEspecial> ataqueDagaEspecialDict;
    private Dictionary<TipoPistola, AtaquePistola> ataquePistolaDict;

    void Awake()
    {
        ataquePuñosPatadasDict = new Dictionary<TipoPuñosPatadas, AtaquePuñosPatadas>();
        ataqueDagaNormalDict = new Dictionary<TipoDagaNormal, AtaqueDagaNormal> ();
        ataqueDagaEspecialDict = new Dictionary<TipoDagaEspecial, AtaqueDagaEspecial>();
        ataquePistolaDict = new Dictionary<TipoPistola, AtaquePistola> ();

        foreach (var ataque in ataquesPuñosPatadas)
            ataquePuñosPatadasDict[ataque.tipo] = ataque;

        foreach (var ataque in ataquesDagaNormales)
            ataqueDagaNormalDict[ataque.tipo] = ataque;

        foreach (var ataque in ataquesDagaEspeciales)
            ataqueDagaEspecialDict[ataque.tipo] = ataque;

        foreach (var disparo in disparosPistola)
            ataquePistolaDict[disparo.tipo] = disparo;
    }

    public AtaquePuñosPatadas ObtenerAtaque(TipoPuñosPatadas tipo)
    {
        return ataquePuñosPatadasDict.ContainsKey(tipo) ? ataquePuñosPatadasDict[tipo] : null;
    }

    public AtaqueDagaNormal ObtenerAtaque(TipoDagaNormal tipo)
    {
        return ataqueDagaNormalDict.ContainsKey(tipo) ? ataqueDagaNormalDict[tipo] : null;
    }

    public AtaqueDagaEspecial ObtenerAtaque(TipoDagaEspecial tipo)
    {
        return ataqueDagaEspecialDict.ContainsKey(tipo) ? ataqueDagaEspecialDict[tipo] : null;
    }

    public AtaquePistola ObtenerAtaque(TipoPistola tipo)
    {
        return ataquePistolaDict.ContainsKey(tipo) ? ataquePistolaDict[tipo] : null;
    }
}
