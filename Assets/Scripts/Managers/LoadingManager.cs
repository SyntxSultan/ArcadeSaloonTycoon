using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.AI.Navigation;

public class LoadingManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private int sceneBuildIndex = 1;

    [Header("UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI percentText;

    [Header("Options")]
    [SerializeField] private bool unloadLoadingSceneAfter = true;
    [SerializeField] private float minimumLoadTime = 5f;
    [SerializeField] private float interpolationSpeed = 2f;

    private float currentProgress = 0f;
    private float targetProgress = 0f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSequence());
        StartCoroutine(InterpolateProgress());
    }

    private IEnumerator LoadSequence()
    {
        float startTime = Time.time;
        SetTargetProgress(0.1f);
        
        // Phase 1: Scene Loading with time-based progress simulation
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        loadOperation.allowSceneActivation = false;

        float loadingTime = 0f;
        float targetLoadTime = 2f; // Simulate 2 seconds of loading time

        while (loadingTime < targetLoadTime || loadOperation.progress < 0.9f)
        {
            loadingTime += Time.deltaTime;
            
            // Combine actual loading progress with time-based progress
            float timeProgress = Mathf.Clamp01(loadingTime / targetLoadTime);
            float realProgress = loadOperation.progress / 0.9f;
            float combinedProgress = Mathf.Max(timeProgress, realProgress);
            
            SetTargetProgress(0.1f + (combinedProgress * 0.8f)); // Scale to 10-90%
            yield return null;
        }

        SetTargetProgress(0.9f);
        
        yield return new WaitUntil(() => currentProgress >= 0.89f);

        SetTargetProgress(0.92f);
        
        SetTargetProgress(1f);
        
        // Wait for interpolation to reach 100%
        yield return new WaitUntil(() => currentProgress >= 0.99f);
        yield return new WaitForSeconds(0.2f); // Brief pause to show 100%

        loadOperation.allowSceneActivation = true;

        yield return StartCoroutine(BuildNavMeshesWithProgress());
        
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadTime)
        {
            yield return new WaitForSeconds(minimumLoadTime - elapsedTime);
        }
        

        if (unloadLoadingSceneAfter)
        {
            Scene thisScene = gameObject.scene;
            if (thisScene.IsValid())
            {
                Destroy(gameObject);
                yield return SceneManager.UnloadSceneAsync(thisScene);
            }
        }
    }

    private IEnumerator InterpolateProgress()
    {
        while (gameObject != null)
        {
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, interpolationSpeed * Time.deltaTime);
            UpdateProgressUI(currentProgress);
            yield return null;
        }
    }

    private void SetTargetProgress(float target)
    {
        targetProgress = Mathf.Clamp01(target);
    }

    private void UpdateProgressUI(float value01)
    {
        value01 = Mathf.Clamp01(value01);
        
        if (progressBar != null) 
            progressBar.value = value01;
            
        if (percentText != null) 
            percentText.text = Mathf.RoundToInt(value01 * 100f) + "%";
    }

    private IEnumerator BuildNavMeshesWithProgress()
    {
        var surfaces = NavMeshSurface.activeSurfaces;
        if (surfaces == null || surfaces.Count == 0) 
        {
            Debug.LogWarning("No NavMeshSurfaces found in the scene. Skipping NavMesh building.");
            yield break;
        }
        
        foreach (NavMeshSurface s in surfaces)
        {
            s.BuildNavMesh();
            yield return null;
        }
    }
}