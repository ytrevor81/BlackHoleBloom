using System.Numerics;
using UnityEngine;

public interface IGravityInteract
{
    public void EnterOrbitOfPlayer(bool isRealPlayer);
    public void EnterOrbitOfOtherCelestialBody(CelestialBody celestialBody, Collider2D _collider);
}

public interface IBarrierInteract
{
    public void HitBarrier(UnityEngine.Vector2 contactPoint);
}
