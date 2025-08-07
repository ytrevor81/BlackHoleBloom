using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpawnedNumberManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnedNumberPrefab;
    [SerializeField] private int numOfNumbers;
    [SerializeField] private float spawnRate;
    private float timer;
    private const string PLUS = "+";
    private StringBuilder stringBuilder = new StringBuilder();
    private List<int> massNumbersToBeSpawned = new List<int>();
    private List<SpawnedNumber> numbersToSpawn = new List<SpawnedNumber>();
    private
    void Awake()
    {
        for (int i = 0; i < numOfNumbers; i++)
        {
            GameObject spawnedNumber = Instantiate(spawnedNumberPrefab, transform.position, Quaternion.identity, transform);
            numbersToSpawn.Add(spawnedNumber.GetComponent<SpawnedNumber>());

            spawnedNumber.SetActive(false);
        }
    }

    void Update()
    {
        if (massNumbersToBeSpawned.Count > 0)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                SpawnNumber();
            }
        }        
    }

    private SpawnedNumber GetSpawnedNumber()
    {
        SpawnedNumber spawnedNumber = null;
        for (int i=0; i < numbersToSpawn.Count; i++)
        {
            if (!numbersToSpawn[i].gameObject.activeInHierarchy)
            {
                spawnedNumber = numbersToSpawn[i];
                break;
            }
        }

        return spawnedNumber;
    }

    public void SpawnNumber()
    {
        timer = spawnRate;
        SpawnedNumber spawnedNumber = GetSpawnedNumber();

        int massNum = massNumbersToBeSpawned[0];
        massNumbersToBeSpawned.RemoveAt(0);

        stringBuilder.Clear();
        stringBuilder.Append(PLUS);
        stringBuilder.Append(massNum.ToString("N0"));
        spawnedNumber.NumberText.text = stringBuilder.ToString();

        spawnedNumber.transform.position = transform.position;
        spawnedNumber.gameObject.SetActive(true);
    }

    public void AddNumToQueue(int number)
    {
        if (massNumbersToBeSpawned.Count > 30)
            return;
        
        else if (massNumbersToBeSpawned.Count > 20)
            number = number * 4;

        else if (massNumbersToBeSpawned.Count > 10)
            number = number * 3;

        else if (massNumbersToBeSpawned.Count > 5)
            number = number * 2;

        massNumbersToBeSpawned.Add(number);
    }    
}
