using UnityEngine;
using TMPro;

namespace DigestQuest
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        public TMP_Text scoreText; // The UI text displaying the score

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
            // Try to find the score text in the scene by common names
            scoreText = GameObject.Find("PlayerScoreDisplay")?.GetComponent<TMP_Text>();
            if (scoreText == null)
                scoreText = GameObject.Find("PlayerScoreText")?.GetComponent<TMP_Text>();

            if (scoreText == null)
                Debug.LogWarning("ScoreText TMP_Text not found in scene!");

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

        // Call this after every scene load!
        public void RelinkSceneReferences(Canvas canvas)
        {
            // First, try to find PlayerScoreText under PlayerScoreDisplay
            Transform displayTransform = canvas.transform.Find("PlayerScoreDisplay");
            if (displayTransform != null)
            {
                Transform textTransform = displayTransform.Find("PlayerScoreText");
                if (textTransform != null)
                {
                    scoreText = textTransform.GetComponent<TMP_Text>();
                }
            }

            // If not found, try to find PlayerScoreText directly under Canvas
            if (scoreText == null)
            {
                Transform directTextTransform = canvas.transform.Find("PlayerScoreText");
                if (directTextTransform != null)
                    scoreText = directTextTransform.GetComponent<TMP_Text>();
            }

            // If still not found, try fallback: first TMP_Text in canvas
            if (scoreText == null)
                scoreText = canvas.GetComponentInChildren<TMP_Text>(true);

            if (scoreText == null)
                Debug.LogWarning("ScoreText UI reference is not assigned! Check Canvas for PlayerScoreText or Canvas > PlayerScoreDisplay > PlayerScoreText.");
            else
                UpdateScoreUI();
        }
    }
}