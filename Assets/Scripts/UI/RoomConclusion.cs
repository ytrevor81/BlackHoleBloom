using System.Collections;
using UnityEngine;

public class RoomConclusion : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;
    [SerializeField] private float menuFadeInSpeed;
    [SerializeField] private float menuFadeOutSpeed;
    private Coroutine fadeCoroutine;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void OnEnable()
    {
        if (GameManager.Instance.CurrentLevel == GameManager.Level.Level10)
        {
            continueButton.SetActive(true);
            winText.SetActive(true);
            loseText.SetActive(false);
        }
        else
        {
            continueButton.SetActive(false);
            winText.SetActive(false);
            loseText.SetActive(true);
        }

        Time.timeScale = 0;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeInMenu());
    }

    private IEnumerator FadeInMenu()
    {
        float alpha = 0;

        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime * menuFadeInSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOutMenu()
    {
        float alpha = 1;

        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime * menuFadeOutSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void ContinueGame_Event()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutMenu());
        Time.timeScale = 1;
    }
}
