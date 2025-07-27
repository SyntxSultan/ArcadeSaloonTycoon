using UnityEngine;

namespace DebugsUtilities
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI fpsText;
        [SerializeField] private float updateInterval = 0.1f;

        private float fps;
        private float timer;
        private int frameCount;

        private void Update()
        {
            frameCount++;
            timer += Time.deltaTime;

            if (timer >= updateInterval)
            {
                fps = frameCount / timer;
                frameCount = 0;
                timer = 0f;
            }

            fpsText.text = fps.ToString("F1");
        }

    }
}