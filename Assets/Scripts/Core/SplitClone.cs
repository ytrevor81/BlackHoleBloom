using System.Collections.Generic;
using UnityEngine;

public class SplitClone : MonoBehaviour
{
    private GameManager GM;
    [SerializeField] private GameObject gravityArea;

    private List<CelestialBody> objectsInOrbit = new List<CelestialBody>();
    private HashSet<GameObject> objectsInOrbitGameObjects = new HashSet<GameObject>();

    void Start()
    {
        GM = GameManager.Instance;
    }

    public void AddObjectToOrbitingList(Collider2D _collider)
    {
        if (objectsInOrbitGameObjects.Contains(_collider.gameObject))
            return;

        objectsInOrbitGameObjects.Add(_collider.gameObject);

        CelestialBody celestialBody = _collider.GetComponent<CelestialBody>();
        celestialBody.EnterOrbitOfClone(_targetOrbit: transform);
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
