using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DigestQuest
{
    public class GameManager : MonoBehaviour //a singleton, only one instance of this class should exist in a game session lifetime
    {
        public static GameManager Instance { get; private set; }

        public AudioManager AudioManager { get; private set; }
        public DeckManager DeckManager { get; private set; }
        public HandManager HandManager { get; private set; }

        public PlayZoneManager PlayZoneManager { get; private set; }
        public Transform PlayZoneArea { get; private set; }

        public Player Player { get; private set; }

        public string sceneName;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Only one GameManager allowed!
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Listen for scene changes!
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks!
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // Called by Unity automatically after any scene loads
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneName = scene.name;
            Debug.Log($">>>>>==========GameManager detected scene loaded: {sceneName}");

            if (sceneName != "Title")
            {
                InitialisePlayer();
                InitialiseManagers();

                var sceneController = GetComponent<SceneController>();

                // --- Assign SkipStageButton for all stages ---
                Button skipButton = GameObject.Find("SkipStageButton")?.GetComponent<Button>();
                if (skipButton != null && sceneController != null)
                {
                    skipButton.onClick.RemoveAllListeners();

                    if (sceneName == "Mouth")
                    {
                        skipButton.onClick.AddListener(() => sceneController.GoToStomach());
                        Debug.Log("SkipStageButton in Mouth wired to GoToStomach!");
                    }
                    else if (sceneName == "Stomach")
                    {
                        skipButton.onClick.AddListener(() => sceneController.GoToIntestine());
                        Debug.Log("SkipStageButton in Stomach wired to GoToIntestine!");
                    }
                    else if (sceneName == "Intestine")
                    {
                        skipButton.onClick.AddListener(() => sceneController.GoToEndScreen());
                        Debug.Log("SkipStageButton in Intestine wired to GoToEndScreen!");
                    }
                }
                else
                {
                    Debug.LogWarning("SkipStageButton or SceneController not found in stage scene!");
                }
            }
        }

        private void InitialisePlayer()
        {
            if (Player.Instance == null)
            {
                GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
                if (playerPrefab == null)
                {
                    Debug.LogError("Cannot find Player prefab in Resources/Prefabs!");
                    return;
                }
                Instantiate(playerPrefab); // Don't parent it!
            }
            // Assign the Player property (even if it already existed)
            Player = Player.Instance;
            Debug.Log("GameManager.Player assigned: " + (Player != null ? Player.gameObject.name : "NULL"));
        }

        private void InitialiseManagers()
        {
            // --- PlayZoneManager instantiation ---
            if (PlayZoneManager != null)
            {
                Destroy(PlayZoneManager.gameObject);
                PlayZoneManager = null;
            }
            if (HandManager != null)
            {
                Destroy(HandManager.gameObject);
                HandManager = null;
            }
            if (AudioManager != null)
            {
                Destroy(AudioManager.gameObject);
                AudioManager = null;
            }
            if (DeckManager != null)
            {
                Destroy(DeckManager.gameObject);
                DeckManager = null;
            }

            // --- PlayZoneManager instantiation ---
            GameObject playZoneManagerPrefab = Resources.Load<GameObject>("Prefabs/PlayZoneManager");
            if (playZoneManagerPrefab == null)
            {
                Debug.LogError("Cannot find the PlayZoneManager prefab");
            }
            else
            {
                // Find the Canvas in the scene
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("No Canvas found in the scene! Cannot create PlayZoneManager.");
                    return;
                }

                // Find or create PlayZoneArea under Canvas
                Transform playZoneAreaTransform = canvas.transform.Find("PlayZoneArea");
                if (playZoneAreaTransform == null)
                {
                    GameObject playZoneAreaGO = new GameObject("PlayZoneArea", typeof(RectTransform));
                    playZoneAreaGO.transform.SetParent(canvas.transform, false);
                    playZoneAreaTransform = playZoneAreaGO.transform;
                    // You can add a HorizontalLayoutGroup here too if needed!
                }
                PlayZoneArea = playZoneAreaTransform;

                // Instantiate PlayZoneManager under Canvas (for UI)
                GameObject playZoneManagerGO = Instantiate(playZoneManagerPrefab, canvas.transform);
                PlayZoneManager = playZoneManagerGO.GetComponent<PlayZoneManager>();

                // Assign dependencies
                PlayZoneManager.playZoneArea = PlayZoneArea;
                PlayZoneManager.handManager = HandManager;
                PlayZoneManager.deckManager = DeckManager;

                // Find the DigestButton under Canvas
                var digestButtonGO = canvas.transform.Find("DigestButton");
                if (digestButtonGO != null)
                {
                    Button digestButton = digestButtonGO.GetComponent<Button>();
                    PlayZoneManager.digestButton = digestButton;
                    digestButton.onClick.AddListener(PlayZoneManager.OnDigestButtonClicked);
                }
                else
                {
                    Debug.LogError("DigestButton not found under Canvas!");
                }
            }

            // --- HandManager instantiation ---
            GameObject handManagerPrefab = Resources.Load<GameObject>("Prefabs/HandManager");
            GameObject handPositionPrefab = Resources.Load<GameObject>("Prefabs/HandPosition");
            if (handManagerPrefab == null)
            {
                Debug.Log("cannot find the HandManager prefab");
            }
            else if (handPositionPrefab == null)
            {
                Debug.LogError("Cannot find the HandPosition prefab in Resources/Prefabs!");
            }
            else
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("No Canvas found in the scene! Cannot create HandPosition.");
                    return;
                }

                // Find or create HandPosition under Canvas
                Transform handPositionTransform = canvas.transform.Find("HandPosition");
                RectTransform handRect;
                if (handPositionTransform == null)
                {
                    // Instantiate HandPosition prefab under Canvas
                    GameObject handPositionGO = Instantiate(handPositionPrefab, canvas.transform);
                    handPositionGO.name = "HandPosition"; // Ensure consistent naming
                    handRect = handPositionGO.GetComponent<RectTransform>();
                }
                else
                {
                    handRect = handPositionTransform.GetComponent<RectTransform>();
                }

                // Instantiate HandManager and assign handTransform
                GameObject handManagerGO = Instantiate(handManagerPrefab, transform.position, Quaternion.identity, transform);
                HandManager = handManagerGO.GetComponent<HandManager>();
                HandManager.handTransform = handRect;
            }

            // --- AudioManager instantiation ---
            GameObject audioManagerPrefab = Resources.Load<GameObject>("Prefabs/AudioManager");
            if (audioManagerPrefab == null)
            {
                Debug.Log("cannot find the Audio manager prefab");
            }
            else
            {
                GameObject audioManagerGO = Instantiate(audioManagerPrefab, transform.position, Quaternion.identity, transform);
                AudioManager = audioManagerGO.GetComponent<AudioManager>();
            }

            // --- DeckManager instantiation ---
            GameObject deckManagerPrefab = Resources.Load<GameObject>("Prefabs/DeckManager");
            if (deckManagerPrefab == null)
            {
                Debug.Log("cannot find the DECK manager prefab");
            }
            else
            {
                GameObject deckManagerGO = Instantiate(deckManagerPrefab, transform.position, Quaternion.identity, transform);
                DeckManager = deckManagerGO.GetComponent<DeckManager>();
            }
        }
    }
}