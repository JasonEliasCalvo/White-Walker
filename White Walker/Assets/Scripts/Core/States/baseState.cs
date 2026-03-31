public abstract class BaseState
{
    protected FighterEntity fighter; // Referencia genérica (sirve para Player y Enemy)

    public int currentFrame { get; protected set; }

    public BaseState(FighterEntity fighter)
    {
        this.fighter = fighter;
    }

    public virtual bool CanBeInterrupted => true;

    public virtual void EnterState()
    {
        currentFrame = 0;
    }

    public virtual void UpdateState()
    {
        currentFrame++;
    }

    public virtual void ExitState()
        {
        // Por defecto, no hacemos nada al salir del estado.
        // Las clases hijas pueden sobreescribir esto para limpiar variables o resetear animaciones.
    }
}

