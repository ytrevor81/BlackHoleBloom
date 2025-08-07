using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenArrow : MonoBehaviour
{
    public static OffscreenArrow Instance { get; private set; }
    
    [SerializeField] private CometArrow[] offscreenArrows;
    private List<CometArrow> activeOffscreenArrows;

    [Space]

    [SerializeField] private float buffer;

    [SerializeField] private Camera mainCamera;

    [Header("Distance-Based Visual Effects")]
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float minAlpha;
    [SerializeField] private float maxAlpha;
    [SerializeField] private float maxDistance;

    private Vector3 viewportPos;
    private Vector3 screenPoint;
    private Vector3 targetDirection;
    private Vector3 edgePosition;
    private Vector3 screenBounds;

    private float angle;
    private float screenAspect;
    private float targetAspect;
    private float edgeX;
    private float edgeY;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        activeOffscreenArrows = new List<CometArrow>();
    }

    void Update()
    {
        if (activeOffscreenArrows.Count < 1) 
            return;

        MoveArrow();
    }

    public void InitializeTargetForOffscreenArrow(Transform _target)
    {
        for (int i = 0; i < offscreenArrows.Length; i++)
        {
            if (offscreenArrows[i].Used)
                continue;

            offscreenArrows[i].Used = true;
            offscreenArrows[i].Target = _target;
            activeOffscreenArrows.Add(offscreenArrows[i]);
            return;
        }        
    }

    public void RemoveTargetFromOffscreenArrow(Transform _target)
    {
        CometArrow arrow = offscreenArrows[0];

        for (int i = 0; i < activeOffscreenArrows.Count; i++)
        {
            if (activeOffscreenArrows[i].Target == _target)
            {
                arrow = activeOffscreenArrows[i];
                activeOffscreenArrows[i].ArrowContainer.gameObject.SetActive(false);
                activeOffscreenArrows.Remove(activeOffscreenArrows[i]);
                break;
            }
        }

        for (int i = 0; i < offscreenArrows.Length; i++)
        {
            if (offscreenArrows[i].ArrowContainer == arrow.ArrowContainer)
            {
                offscreenArrows[i].Used = false;
                break;
            }
        }               
    }

    private bool TargetIsOffscreen(CometArrow arrow)
    {
        screenBounds = new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, 0);
        viewportPos = mainCamera.WorldToViewportPoint(arrow.Target.position);
        return viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;
    }

    private void UpdateArrowRotation(CometArrow arrow)
    {
        targetDirection = arrow.Target.position - mainCamera.transform.position;
        targetDirection.z = 0;
        angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        arrow.ArrowContainer.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void UpdateArrowPosition(CometArrow arrow)
    {
        screenPoint = mainCamera.WorldToScreenPoint(arrow.Target.position);
        screenPoint -= screenBounds * 0.5f;
        screenAspect = screenBounds.x / screenBounds.y;
        targetAspect = Mathf.Abs(screenPoint.x / screenPoint.y);

        if (targetAspect > screenAspect)
        {
            edgeX = (screenPoint.x > 0 ? 0.5f : -0.5f) * screenBounds.x;
            edgeY = edgeX * screenPoint.y / screenPoint.x;
        }
        else
        {
            edgeY = (screenPoint.y > 0 ? 0.5f : -0.5f) * screenBounds.y;
            edgeX = edgeY * screenPoint.x / screenPoint.y;
        }

        edgePosition = new Vector3(edgeX, edgeY, 0) + screenBounds * 0.5f;
        edgePosition = new Vector3(Mathf.Clamp(edgePosition.x, buffer, screenBounds.x - buffer), Mathf.Clamp(edgePosition.y, buffer, screenBounds.y - buffer), 0);
        arrow.ArrowContainer.position = edgePosition;
    }

    private void UpdateArrowVisuals(CometArrow arrow)
    {
        float distance = Vector3.Distance(mainCamera.transform.position, arrow.Target.position);
        float scale = Mathf.Lerp(maxScale, minScale, Mathf.InverseLerp(0, maxDistance, distance));
        float alpha = Mathf.Lerp(maxAlpha, minAlpha, Mathf.InverseLerp(0, maxDistance, distance));

        arrow.ArrowContainer.localScale = new Vector3(scale, scale, 1);
        arrow.ArrowImage.color = new Color(1, 1, 1, alpha);
    }

    private void MoveArrow()
    {
        for (int i=0; i < activeOffscreenArrows.Count; i++)
        {
            CometArrow arrow = activeOffscreenArrows[i];

            if (TargetIsOffscreen(arrow))
            {
                UpdateArrowRotation(arrow);
                UpdateArrowPosition(arrow);
                UpdateArrowVisuals(arrow);

                if (!arrow.ArrowContainer.gameObject.activeInHierarchy)
                    arrow.ArrowContainer.gameObject.SetActive(true);

                continue;
            }

            if (arrow.ArrowContainer.gameObject.activeInHierarchy)
                arrow.ArrowContainer.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]

public struct CometArrow
{
    [field: SerializeField] public RectTransform ArrowContainer { get; private set; }
    [field: SerializeField] public Image ArrowImage { get; private set; }
    public Transform Target { get; set; }
    public bool Used { get; set; }
}

