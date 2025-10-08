using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DigestQuest
{
    public class GameManager : MonoBehaviour
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
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

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
                var playerObj = Instantiate(playerPrefab);
                DontDestroyOnLoad(playerObj);
            }
            Player = Player.Instance;
            Debug.Log("GameManager.Player assigned: " + (Player != null ? Player.gameObject.name : "NULL"));
        }

        private void InitialiseManagers()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in the scene!");
                return;
            }

            // --- HandManager ---
            if (HandManager == null)
            {
                GameObject handManagerPrefab = Resources.Load<GameObject>("Prefabs/HandManager");
                if (handManagerPrefab != null)
                {
                    // ROOT OBJECT: No parent!
                    GameObject handManagerGO = Instantiate(handManagerPrefab, Vector3.zero, Quaternion.identity);
                    HandManager = handManagerGO.GetComponent<HandManager>();
                    DontDestroyOnLoad(handManagerGO);
                }
                else
                {
                    Debug.LogError("Cannot find the HandManager prefab");
                }
            }
            // Always re-link handTransform to new HandPosition in the scene
            Transform handPositionTransform = canvas.transform.Find("HandPosition");
            RectTransform handRect = null;
            if (handPositionTransform == null)
            {
                GameObject handPositionPrefab = Resources.Load<GameObject>("Prefabs/HandPosition");
                if (handPositionPrefab != null)
                {
                    GameObject handPositionGO = Instantiate(handPositionPrefab, canvas.transform);
                    handPositionGO.name = "HandPosition";
                    handRect = handPositionGO.GetComponent<RectTransform>();
                }
                else
                {
                    Debug.LogError("Cannot find the HandPosition prefab in Resources/Prefabs!");
                }
            }
            else
            {
                handRect = handPositionTransform.GetComponent<RectTransform>();
            }
            if (HandManager != null && handRect != null)
            {
                HandManager.handTransform = handRect;
            }

            // --- DeckManager ---
            if (DeckManager == null)
            {
                GameObject deckManagerPrefab = Resources.Load<GameObject>("Prefabs/DeckManager");
                if (deckManagerPrefab != null)
                {
                    // ROOT OBJECT: No parent!
                    GameObject deckManagerGO = Instantiate(deckManagerPrefab, Vector3.zero, Quaternion.identity);
                    DeckManager = deckManagerGO.GetComponent<DeckManager>();
                    DontDestroyOnLoad(deckManagerGO);
                }
                else
                {
                    Debug.LogError("Cannot find the DECK manager prefab");
                }
            }

            // --- AudioManager ---
            if (AudioManager == null)
            {
                GameObject audioManagerPrefab = Resources.Load<GameObject>("Prefabs/AudioManager");
                if (audioManagerPrefab != null)
                {
                    // ROOT OBJECT: No parent!
                    GameObject audioManagerGO = Instantiate(audioManagerPrefab, Vector3.zero, Quaternion.identity);
                    AudioManager = audioManagerGO.GetComponent<AudioManager>();
                    DontDestroyOnLoad(audioManagerGO);
                }
                else
                {
                    Debug.LogError("Cannot find the Audio manager prefab");
                }
            }

            // --- PlayZoneManager ---
            // PlayZoneManager is SCENE-SPECIFIC, keep as child of Canvas!
            if (PlayZoneManager == null)
            {
                GameObject playZoneManagerPrefab = Resources.Load<GameObject>("Prefabs/PlayZoneManager");
                if (playZoneManagerPrefab != null)
                {
                    // Find or create PlayZoneArea under Canvas
                    Transform playZoneAreaTransform = canvas.transform.Find("PlayZoneArea");
                    if (playZoneAreaTransform == null)
                    {
                        GameObject playZoneAreaGO = new GameObject("PlayZoneArea", typeof(RectTransform));
                        playZoneAreaGO.transform.SetParent(canvas.transform, false);
                        playZoneAreaTransform = playZoneAreaGO.transform;
                    }
                    PlayZoneArea = playZoneAreaTransform;

                    GameObject playZoneManagerGO = Instantiate(playZoneManagerPrefab, canvas.transform);
                    PlayZoneManager = playZoneManagerGO.GetComponent<PlayZoneManager>();
                    PlayZoneManager.playZoneArea = PlayZoneArea;
                    PlayZoneManager.handManager = HandManager;
                    PlayZoneManager.deckManager = DeckManager;

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
                else
                {
                    Debug.LogError("Cannot find the PlayZoneManager prefab");
                }
            }
            else
            {
                // Always re-link playZoneArea in the new scene
                Transform playZoneAreaTransform = canvas.transform.Find("PlayZoneArea");
                if (playZoneAreaTransform == null)
                {
                    GameObject playZoneAreaGO = new GameObject("PlayZoneArea", typeof(RectTransform));
                    playZoneAreaGO.transform.SetParent(canvas.transform, false);
                    playZoneAreaTransform = playZoneAreaGO.transform;
                }
                PlayZoneArea = playZoneAreaTransform;
                PlayZoneManager.playZoneArea = PlayZoneArea;
                PlayZoneManager.handManager = HandManager;
                PlayZoneManager.deckManager = DeckManager;

                var digestButtonGO = canvas.transform.Find("DigestButton");
                if (digestButtonGO != null)
                {
                    Button digestButton = digestButtonGO.GetComponent<Button>();
                    PlayZoneManager.digestButton = digestButton;
                    digestButton.onClick.RemoveAllListeners();
                    digestButton.onClick.AddListener(PlayZoneManager.OnDigestButtonClicked);
                }
                else
                {
                    Debug.LogError("DigestButton not found under Canvas!");
                }
            }
        }
    }
}