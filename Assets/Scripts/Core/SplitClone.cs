using System.Collections.Generic;
using UnityEngine;

public class SplitClone : MonoBehaviour
{
    private GameManager GM;
    private HUDController HUD;
    [SerializeField] private GameObject gravityArea;

    private List<CelestialBody> objectsInOrbit = new List<CelestialBody>();
    private HashSet<GameObject> objectsInOrbitGameObjects = new HashSet<GameObject>();

    void Start()
    {
        GM = GameManager.Instance;
        HUD = HUDController.Instance;
    }

    private void CacheHUDIfNull()
    {
        if (HUD != null)
            return;

        HUD = HUDController.Instance;
    }

    public void AddWhiteHoleMass(int mass, int numOfObjectsAbsorbed)
    {
        if (GM.CutscenePlaying)
            return;

        CacheHUDIfNull();
        GM.Mass += mass;
        GM.NumOfObjectsAbsorbed += numOfObjectsAbsorbed;
        HUD.UpdateHUDNumbers(mass);
        GM.ChangeLevelIfValid();
    }

    public void AddMass(int mass)
    {
        if (GM.CutscenePlaying)
            return;

        CacheHUDIfNull();
        GM.Mass += mass;
        GM.NumOfObjectsAbsorbed += 1;
        HUD.UpdateHUDNumbers(mass);
        GM.ChangeLevelIfValid();
    }

    public void AddObjectToOrbitingList(Collider2D _collider)
    {
        if (objectsInOrbitGameObjects.Contains(_collider.gameObject))
            return;

        objectsInOrbitGameObjects.Add(_collider.gameObject);

        CelestialBody celestialBody = _collider.GetComponent<CelestialBody>();
        celestialBody.EnterOrbitOfPlayer(isRealPlayer: false);
        objectsInOrbit.Add(celestialBody);
    }
    public void RemoveObjectFromOrbitingList(CelestialBody _celestialBody)
    {
        if (!objectsInOrbitGameObjects.Contains(_celestialBody.gameObject))
            return;

        objectsInOrbitGameObjects.Remove(_celestialBody.gameObject);
        objectsInOrbit.Remove(_celestialBody);
    }
}
