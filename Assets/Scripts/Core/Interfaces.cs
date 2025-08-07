using System.Numerics;

public interface IGravityInteract
{
    public void EnterOrbit();
}

public interface IBarrierInteract
{
    public void HitBarrier(UnityEngine.Vector2 contactPoint);
}
