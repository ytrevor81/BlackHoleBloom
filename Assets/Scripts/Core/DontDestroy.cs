using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField] private string objectName;
    
    private void Awake()
    {
        if (GameObject.Find(objectName))
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.name = objectName;
    }
}
