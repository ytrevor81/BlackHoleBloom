using UnityEngine;

public class CodexPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private CodexPopupList controller;
    [SerializeField] private float timeVisible;
    [SerializeField] private float fadeOutTime;

    public CodexEntry EntryData { get; set; }
    private bool isFadingIn;
    private bool isFadingOut;
    private float timer;
    private bool canInteract;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    void OnEnable()
    {
        isFadingIn = true;
        isFadingOut = false;
        canInteract = true;
    }

    void Update()
    {
        if (isFadingIn)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeOutTime);

            if (timer >= fadeOutTime)
            {
                isFadingIn = false;
                timer = timeVisible;
            }
        }
        else if (isFadingOut)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);

            if (timer >= fadeOutTime)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                isFadingOut = true;
                timer = 0f; // Reset timer to prevent negative values
            }
        }
    }

    public void PopupPressed_Event()
    {
        if (!canInteract)
            return;

        canInteract = false;
        controller.GoToCodexEntryPageFromPopup(EntryData);
    }
}
