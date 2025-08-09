using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private HUDController HUD;
    [SerializeField] private float menuFadeTime;
    [SerializeField] private CanvasGroup buttonsCanvasGroup;
    [SerializeField] private GameObject optionsPage;
    private CanvasGroup pauseMenuCanvasGroup;
    private IEnumerator currentCoroutine;
    private bool canInteract;

    void Awake()
    {
        pauseMenuCanvasGroup = GetComponent<CanvasGroup>();
        pauseMenuCanvasGroup.alpha = 0;
    }

    void OnEnable()
    {
        Time.timeScale = 0;

        currentCoroutine = FadeInMenu(pauseMenuCanvasGroup);
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
        if (_canvasGroup == buttonsCanvasGroup)
            buttonsCanvasGroup.gameObject.SetActive(true);

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

        if (_canvasGroup == pauseMenuCanvasGroup)
            gameObject.SetActive(false);

        else if (_canvasGroup == buttonsCanvasGroup)
        {
            buttonsCanvasGroup.gameObject.SetActive(false);
            optionsPage.SetActive(true);
        }
    }

    public void ResumeGame_Event()
    {
        if (!canInteract) return;

        StopCurrentCoroutine();

        currentCoroutine = FadeOutMenu(pauseMenuCanvasGroup);
        StartCoroutine(currentCoroutine);
        Time.timeScale = 1;
    }
    public void GoToOptionsPage_Event()
    {
        if (!canInteract) return;

        StopCurrentCoroutine();
        canInteract = false;

        currentCoroutine = FadeOutMenu(buttonsCanvasGroup);
        StartCoroutine(currentCoroutine);
    }
    public void BackToMainMenu_Event()
    {
        if (!canInteract) return;

        ResumeGame_Event();
        HUD.BackToMainMenu();
    }

    public void BackFromOptionsPage()
    {
        StopCurrentCoroutine();

        currentCoroutine = FadeInMenu(buttonsCanvasGroup);
        StartCoroutine(currentCoroutine);
    }
}
