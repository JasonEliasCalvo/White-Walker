using System.Collections.Generic;
using UnityEngine;

public enum TipoPuñosPatadas
{
    GolpeRapido1, GolpeRapido2, GolpeFuerte,
    PatadaBaja, Pisoton, Pisoton2,
}

public enum TipoDaga
{
    CorteRapido, Estocada, CorteLateral,
    Apuñalar,
}

public enum TipoDagaEspecial
{ 
    DagaGiratoria, EstocadaPesada 
}

public enum TipoPistola { DisparoNormal, DisparoFuerte }

public enum TipoRemate{ RematePuñosPatadas, RemateDaga}

public enum TipoProvocacion{ Provocacion1, Provocacion2 }

public class DatosCombate : MonoBehaviour
{
    public List<AtaquePuñosPatadas> ataquesPuñosPatadas;
    public List<AtaqueDaga> ataquesDaga;
    public List<AtaqueDagaEspecial> ataquesDagaEspeciales;
    public List<AtaquePistola> disparosPistola;
    public List<Remate> remate;
    public List<Provocacion> provocaciones;

    private Dictionary<TipoPuñosPatadas, AtaquePuñosPatadas> ataquePuñosPatadasDict;
    private Dictionary<TipoDaga, AtaqueDaga> ataqueDagaDict;
    private Dictionary<TipoDagaEspecial, AtaqueDagaEspecial> ataqueDagaEspecialDict;
    private Dictionary<TipoPistola, AtaquePistola> ataquePistolaDict;
    private Dictionary<TipoRemate, Remate> ataqueRemateDict;

    void Awake()
    {
        ataquePuñosPatadasDict = CrearDiccionario<TipoPuñosPatadas, AtaquePuñosPatadas>(ataquesPuñosPatadas);
        ataqueDagaDict = CrearDiccionario<TipoDaga, AtaqueDaga>(ataquesDaga);
        ataqueDagaEspecialDict = CrearDiccionario<TipoDagaEspecial, AtaqueDagaEspecial>(ataquesDagaEspeciales);
        ataquePistolaDict = CrearDiccionario<TipoPistola, AtaquePistola>(disparosPistola);
        ataqueRemateDict = CrearDiccionario<TipoRemate, Remate>(remate);
    }

    private Dictionary<TEnum, T> CrearDiccionario<TEnum, T>(List<T> lista) where T : Combate<TEnum>
    {
        var diccionario = new Dictionary<TEnum, T>();
        foreach (var ataque in lista)
        {
            diccionario[ataque.tipo] = ataque;
        }
        return diccionario;
    }

    public AtaquePuñosPatadas ObtenerAtaque(TipoPuñosPatadas tipo) => ataquePuñosPatadasDict.TryGetValue(tipo, out var ataque) ? ataque : null;
    public AtaqueDaga ObtenerAtaque(TipoDaga tipo) => ataqueDagaDict.TryGetValue(tipo, out var ataque) ? ataque : null;
    public AtaqueDagaEspecial ObtenerAtaque(TipoDagaEspecial tipo) => ataqueDagaEspecialDict.TryGetValue(tipo, out var ataque) ? ataque : null;
    public AtaquePistola ObtenerAtaque(TipoPistola tipo) => ataquePistolaDict.TryGetValue(tipo, out var ataque) ? ataque : null;
}
