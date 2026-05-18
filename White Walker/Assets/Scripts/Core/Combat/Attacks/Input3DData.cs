using UnityEngine;

public struct Input3DData
{
    public int direction; // 5 = Neutral, 6 = Adelante, 4 = Atrás, 8 = Arriba/Lados si los necesitas
    public bool attackPressed;
    public bool dodgePressed;

    // Genera el código alfanumérico estilo Small Fight (Ej: "5a", "6a", "4a")
    public string Code
    {
        get
        {
            string output = $"{direction}";
            if (attackPressed) output += "a";
            return output;
        }
    }
}