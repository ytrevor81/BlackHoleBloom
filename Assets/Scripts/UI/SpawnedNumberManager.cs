using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpawnedNumberManager : MonoBehaviour
{
    private enum SpawnedNumberType
    {
        Mass,
        Time
    }
    [SerializeField] private SpawnedNumberType spawnedNumberType;
    [SerializeField] private GameObject spawnedNumberPrefab;
    [SerializeField] private int numOfNumbers;
    [SerializeField] private float spawnRate;
    private float timer;
    private const string PLUS = "+";
    private StringBuilder stringBuilder = new StringBuilder();
    private List<int> numbersToBeSpawned = new List<int>();
    private List<SpawnedNumber> numbersToSpawn = new List<SpawnedNumber>();
    private TimeSpan timeSpan;

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
        if (spawnedNumberType == SpawnedNumberType.Time)
            return;
            
        if (numbersToBeSpawned.Count > 0)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                SpawnMassNumber();
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

    private void SpawnMassNumber()
    {
        timer = spawnRate;
        DetermineMassString();
        InitializeNextSpawnedNumber();
    }
    public void SpawnTimeAdditionNumber(float timeAddition)
    {
        DetermineTimeAdditionString(timeAddition);
        InitializeNextSpawnedNumber();
    }

    private void InitializeNextSpawnedNumber()
    {
        SpawnedNumber spawnedNumber = GetSpawnedNumber();
        spawnedNumber.NumberText.text = stringBuilder.ToString();
        spawnedNumber.transform.position = transform.position;
        spawnedNumber.gameObject.SetActive(true);
    }

    private void DetermineMassString()
    {
        stringBuilder.Clear();

        int massNum = numbersToBeSpawned[0];
        numbersToBeSpawned.RemoveAt(0);

        stringBuilder.Append(PLUS);
        stringBuilder.Append(massNum.ToString("N0"));
    }
    private void DetermineTimeAdditionString(float _timeAddition)
    {
        stringBuilder.Clear();

        timeSpan = TimeSpan.FromSeconds(_timeAddition);

        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        stringBuilder.Append(PLUS);
        stringBuilder.Append($"{minutes:D2}:{seconds:D2}");
    }

    public void AddNumToQueue(int number)
    {
        if (numbersToBeSpawned.Count > 30)
            return;

        else if (numbersToBeSpawned.Count > 20)
            number = number * 4;

        else if (numbersToBeSpawned.Count > 10)
            number = number * 3;

        else if (numbersToBeSpawned.Count > 5)
            number = number * 2;

        numbersToBeSpawned.Add(number);
    }    
}
