using TMPro;
using UnityEngine;

public class SpawnedNumber : MonoBehaviour
{
    [field: SerializeField] public TMP_Text NumberText { get; private set; }
    [SerializeField] private RectTransform rect;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float fadeOutSpeed;
    [SerializeField] private float timeUntilFadeOut;
    private float alpha;
    private float timer;

    void OnEnable()
    {
        timer = timeUntilFadeOut;
        NumberText.alpha = 1;
        alpha = 1;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        rect.anchoredPosition += Vector2.up * lerpSpeed * Time.deltaTime;

        if (timer <= 0)
        {
            alpha -= fadeOutSpeed * Time.deltaTime;
            NumberText.alpha = alpha;

            if (alpha <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
