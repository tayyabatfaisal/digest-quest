using UnityEngine;
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
                Button skipButton = GameObject.Find("SkipStageButton")?.GetComponent<Button>();
                if (skipButton != null && sceneController != null)
                {
                    skipButton.onClick.RemoveAllListeners();
                    if (sceneName == "Mouth")
                    {
                        DeckManager.Instance.ResetDeck();
                        DeckManager.Instance.TryPopulateInitialHand();
                    }
                    else if (sceneName == "Stomach")
                        skipButton.onClick.AddListener(() => sceneController.GoToIntestine());
                    else if (sceneName == "Intestine")
                        skipButton.onClick.AddListener(() => sceneController.GoToEndScreen());
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
                GameObject handManagerGO = Instantiate(handManagerPrefab, Vector3.zero, Quaternion.identity);
                HandManager = handManagerGO.GetComponent<HandManager>();
                DontDestroyOnLoad(handManagerGO);
            }
            if (HandManager != null)
                HandManager.RelinkSceneReferences(canvas);

            // --- DeckManager ---
            if (DeckManager == null)
            {
                GameObject deckManagerPrefab = Resources.Load<GameObject>("Prefabs/DeckManager");
                GameObject deckManagerGO = Instantiate(deckManagerPrefab, Vector3.zero, Quaternion.identity);
                // NO parent argument!
                DeckManager = deckManagerGO.GetComponent<DeckManager>();
                DontDestroyOnLoad(deckManagerGO);
                Debug.Log("DeckManager instantiated as root object: " + deckManagerGO);
            }
            if (DeckManager != null)
                Debug.Log("DECKMANAGER EXISTS SOMEWHER");
            DeckManager.RelinkSceneReferences(canvas);

            // --- AudioManager ---
            if (AudioManager == null)
            {
                GameObject audioManagerPrefab = Resources.Load<GameObject>("Prefabs/AudioManager");
                GameObject audioManagerGO = Instantiate(audioManagerPrefab, Vector3.zero, Quaternion.identity);
                AudioManager = audioManagerGO.GetComponent<AudioManager>();
                DontDestroyOnLoad(audioManagerGO);
            }

            // --- PlayZoneManager (scene-specific, child of canvas) ---
            if (PlayZoneManager == null)
            {
                GameObject playZoneManagerPrefab = Resources.Load<GameObject>("Prefabs/PlayZoneManager");
                GameObject playZoneManagerGO = Instantiate(playZoneManagerPrefab, canvas.transform);
                PlayZoneManager = playZoneManagerGO.GetComponent<PlayZoneManager>();
                PlayZoneManager.handManager = HandManager;
                PlayZoneManager.deckManager = DeckManager;
            }
            // Always re-link PlayZoneArea
            Transform playZoneAreaTransform = canvas.transform.Find("PlayZoneArea");
            if (playZoneAreaTransform == null)
            {
                GameObject playZoneAreaGO = new GameObject("PlayZoneArea", typeof(RectTransform));
                playZoneAreaGO.transform.SetParent(canvas.transform, false);
                playZoneAreaTransform = playZoneAreaGO.transform;
            }
            PlayZoneArea = playZoneAreaTransform;
            PlayZoneManager.playZoneArea = PlayZoneArea;
            PlayZoneManager.RelinkSceneReferences(canvas);
            


            // --- Re-link Player score UI ---
            if (Player != null)
                Player.RelinkSceneReferences(canvas);
                    }
                }
}