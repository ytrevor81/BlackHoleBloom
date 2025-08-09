using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PresetManager : MonoBehaviour
{

    [Header("Real Game Startup")]
    [Space]
    [SerializeField] private GameObject blackScreen;

    [Header("Main Menu Viewer")]
    [Space]
    [SerializeField] private GameObject[] mainMenuAssets;

    [Header("Room 1 Viewer")]
    [Space]
    [SerializeField] private GameObject[] room1Assets;

    [SerializeField] private GameObject playerStar;
    [SerializeField] private GameObject playerBlackHole;
    [SerializeField] private CanvasGroup uiCanvasGroup;
    [SerializeField] private GameObject gravityArea;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject[] nebulaBG;
#if UNITY_EDITOR
    public void RealGameStartup_Editor()
    {
        blackScreen.SetActive(true);
        EditorUtility.SetDirty(blackScreen);
        MainMenuViewer_Editor(blackScreenOff: false);
    }

    public void MainMenuViewer_Editor(bool blackScreenOff)
    {
        if (blackScreenOff)
        {
            blackScreen.SetActive(false);
            EditorUtility.SetDirty(blackScreen);
        }

        for (int i = 0; i < mainMenuAssets.Length; i++)
        {
            mainMenuAssets[i].SetActive(true);
            EditorUtility.SetDirty(mainMenuAssets[i]);
        }

        for (int i = 0; i < room1Assets.Length; i++)
        {
            room1Assets[i].SetActive(false);
            EditorUtility.SetDirty(room1Assets[i]);
        }

        uiCanvasGroup.alpha = 0;
        EditorUtility.SetDirty(uiCanvasGroup);

        playerStar.SetActive(true);
        EditorUtility.SetDirty(playerStar);
        playerBlackHole.SetActive(false);
        EditorUtility.SetDirty(playerBlackHole);
        gravityArea.SetActive(false);
        EditorUtility.SetDirty(gravityArea);

        audioManager.TrackToPlayOnStart = AudioManager.MusicTrack.Nebula;
        EditorUtility.SetDirty(audioManager);

        for (int i = 0; i < nebulaBG.Length; i++)
        {
            nebulaBG[i].SetActive(false);
            EditorUtility.SetDirty(nebulaBG[i]);
        }
    }
    public void Room1Viewer_Editor()
    {
        blackScreen.SetActive(false);
        EditorUtility.SetDirty(blackScreen);

        for (int i = 0; i < mainMenuAssets.Length; i++)
        {
            mainMenuAssets[i].SetActive(false);
            EditorUtility.SetDirty(mainMenuAssets[i]);
        }

        for (int i = 0; i < room1Assets.Length; i++)
        {
            room1Assets[i].SetActive(true);
            EditorUtility.SetDirty(room1Assets[i]);
        }

        uiCanvasGroup.alpha = 1;
        EditorUtility.SetDirty(uiCanvasGroup);

        playerStar.SetActive(false);
        EditorUtility.SetDirty(playerStar);
        playerBlackHole.SetActive(true);
        EditorUtility.SetDirty(playerBlackHole);
        gravityArea.SetActive(true);
        EditorUtility.SetDirty(gravityArea);
        
        audioManager.TrackToPlayOnStart = AudioManager.MusicTrack.Nebula;
        EditorUtility.SetDirty(audioManager);

        for (int i = 0; i < nebulaBG.Length; i++)
        {
            nebulaBG[i].SetActive(true);
            EditorUtility.SetDirty(nebulaBG[i]);
        }
    }
#endif
}