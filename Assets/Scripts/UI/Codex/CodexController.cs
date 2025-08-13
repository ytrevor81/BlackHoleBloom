using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexController : MonoBehaviour
{
    private CanvasGroup mainCanvasGroup;
    [SerializeField] private CanvasGroup itemListCanvasGroup;
    [SerializeField] private CodexItem[] codexItems;
    [SerializeField] private float menuFadeTime;

    [Space]

    [SerializeField] private CanvasGroup codexEntryProfilePage;
    [SerializeField] private Image codexEntryProfilePageImage;
    [SerializeField] private TMP_Text codexEntryProfileNameText;
    [SerializeField] private TMP_Text codexEntryProfileNameDescription;
    private IEnumerator currentCoroutine;
    private bool canInteract;
    public CodexEntry CurrentCodexEntry { get; set; }

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

        if (CurrentCodexEntry != null)
            ViewCodexEntryProfileOnEnable();

        else
        {
            codexEntryProfilePage.gameObject.SetActive(false);
            itemListCanvasGroup.alpha = 1f;
            itemListCanvasGroup.gameObject.SetActive(true);
        }
        
        currentCoroutine = FadeInMenu(mainCanvasGroup);
        StartCoroutine(currentCoroutine);
    }

    void OnDisable()
    {
        CurrentCodexEntry = null;
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
    private IEnumerator FadeOutAndInMenu(CanvasGroup _canvasGroupToFadeOut, CanvasGroup _canvasGroupToFadeIn)
    {
        float currentAlpha = _canvasGroupToFadeOut.alpha;
        float elapsedTime = 0;

        while (elapsedTime < menuFadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroupToFadeOut.alpha = Mathf.Lerp(currentAlpha, 0, elapsedTime / menuFadeTime);
            yield return null;
        }

        _canvasGroupToFadeOut.alpha = 0;
        _canvasGroupToFadeOut.gameObject.SetActive(false);

        _canvasGroupToFadeIn.alpha = 0;
        _canvasGroupToFadeIn.gameObject.SetActive(true);
        elapsedTime = 0;

        while (elapsedTime < menuFadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroupToFadeIn.alpha = Mathf.Lerp(0, 1, elapsedTime / menuFadeTime);
            yield return null;
        }

        canInteract = true;
    }

    public void ViewCodexEntryProfile(CodexEntry _entryData)
    {
        canInteract = false;
        StopCurrentCoroutine();

        CurrentCodexEntry = _entryData;
        codexEntryProfilePageImage.sprite = _entryData.Icon;
        codexEntryProfileNameText.text = _entryData.Title;
        codexEntryProfileNameDescription.text = _entryData.Description;

        currentCoroutine = FadeOutAndInMenu(itemListCanvasGroup, codexEntryProfilePage);
        StartCoroutine(currentCoroutine);
    }
    private void ViewCodexEntryProfileOnEnable()
    {
        canInteract = false;

        codexEntryProfilePageImage.sprite = CurrentCodexEntry.Icon;
        codexEntryProfileNameText.text = CurrentCodexEntry.Title;
        codexEntryProfileNameDescription.text = CurrentCodexEntry.Description;

        itemListCanvasGroup.alpha = 0;
        itemListCanvasGroup.gameObject.SetActive(false);

        codexEntryProfilePage.alpha = 1f;
        codexEntryProfilePage.gameObject.SetActive(true);
    }
    public void BackButton_Event()
    {
        if (canInteract)
        {
            StopCurrentCoroutine();

            if (codexEntryProfilePage.gameObject.activeInHierarchy)
            {
                canInteract = false;
                currentCoroutine = FadeOutAndInMenu(codexEntryProfilePage, itemListCanvasGroup);
                StartCoroutine(currentCoroutine);
            }
            else
            {
                canInteract = false;
                currentCoroutine = FadeOutMenu(mainCanvasGroup);
                StartCoroutine(currentCoroutine);
                Time.timeScale = 1;
            }
        }
    }
}
