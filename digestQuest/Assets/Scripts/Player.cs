using UnityEngine;
using TMPro;

namespace DigestQuest
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        public TMP_Text scoreText; // Will be found automatically at runtime
        public int score = 0;

        private void Awake()
        {
            // Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Find the TMP_Text in the scene by name
            scoreText = GameObject.Find("PlayerScoreText")?.GetComponent<TMP_Text>();

            // Make sure it's found
            if (scoreText == null)
                Debug.LogWarning("PlayerScoreText TMP_Text not found in scene!");

            // Initial update
            UpdateScoreUI();
        }

        public void AddScore(int amount)
        {
            score += amount;
            Debug.Log("Score updated! Current score: " + score);
            UpdateScoreUI();
        }

        public void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
            else
            {
                Debug.LogWarning("ScoreText UI reference is not assigned!");
            }
        }
    }
}