using System.Collections.Generic;
using UnityEngine;

public class SplitClone : MonoBehaviour
{
    private GameManager GM;
    [SerializeField] private GameObject gravityArea;

    void Start()
    {
        GM = GameManager.Instance;
    }
}
