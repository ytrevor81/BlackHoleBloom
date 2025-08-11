using System.Collections;
using UnityEngine;

public class CodexController : MonoBehaviour
{
    private CanvasGroup mainCanvasGroup;
    [SerializeField] private CanvasGroup itemListCanvasGroup;
    [SerializeField] private CanvasGroup profileCanvasGroup;
    [SerializeField] private CodexItem[] codexItems;
    [SerializeField] private float menuFadeTime;
    private IEnumerator currentCoroutine;
    private bool canInteract;

    void Awake()
    {
        mainCanvasGroup = GetComponent<CanvasGroup>();
        mainCanvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        Time.timeScale = 0;

        for (int i = 0; i < codexItems.Length; i++)
        {
            if (codexItems[i].CodexEntry.IsDiscovered)
            {
                codexItems[i].gameObject.SetActive(true);
            }
        }

        currentCoroutine = FadeInMenu(mainCanvasGroup);
        StartCoroutine(currentCoroutine);
    }

    void OnDisable()
    {
        StopCurrentCoroutine();
        canInteract = false;
    }
    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    private IEnumerator FadeInMenu(CanvasGroup _canvasGroup)
    {
        float currentAlpha = _canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < menuFadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(currentAlpha, 1, elapsedTime / menuFadeTime);
            yield return null;
        }

        _canvasGroup.alpha = 1;
        canInteract = true;
    }

    private IEnumerator FadeOutMenu(CanvasGroup _canvasGroup)
    {
        float currentAlpha = _canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < menuFadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(currentAlpha, 0, elapsedTime / menuFadeTime);
            yield return null;
        }

        _canvasGroup.alpha = 0;

        if (_canvasGroup == mainCanvasGroup)
            gameObject.SetActive(false);
    }
    
    public void ResumeGame_Event()
    {
        if (canInteract)
        {
            StopCurrentCoroutine();

            currentCoroutine = FadeOutMenu(mainCanvasGroup);
            StartCoroutine(currentCoroutine);
            canInteract = false;
            Time.timeScale = 1;
        }
    }

}
