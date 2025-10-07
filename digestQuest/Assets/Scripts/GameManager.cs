using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{
    public class GameManager : MonoBehaviour //a singleton, only one instance of this class should exist in a game session lifetime
    {
        //init variables

        public static GameManager Instance { get; private set; }

        public AudioManager AudioManager { get; private set; }
        public DeckManager DeckManager { get; private set; }
        public HandManager HandManager { get; private set; }

        public PlayZoneManager PlayZoneManager { get; private set; }
        public Transform PlayZoneArea { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Only one GameManager allowed!
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitialisePlayer();
            InitialiseManagers();
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
                Instantiate(playerPrefab, transform.position, Quaternion.identity);
            }
        }

        private void InitialiseManagers()
        {
            // --- PlayZoneManager instantiation ---
            if (PlayZoneManager == null)
            {
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
            }
            if (HandManager == null)
            {
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
            }

            // --- AudioManager instantiation ---
            if (AudioManager == null)
            {
                GameObject audioManagerPrefab = Resources.Load<GameObject>("Prefabs/AudioManager");
                if (audioManagerPrefab == null)
                {
                    Debug.Log("cannot find the Audio manager prefab");
                }
                else
                {
                    Instantiate(audioManagerPrefab, transform.position, Quaternion.identity, transform);
                    AudioManager = GetComponentInChildren<AudioManager>();
                }
            }

            // --- DeckManager instantiation ---
            if (DeckManager == null)
            {
                GameObject deckManagerPrefab = Resources.Load<GameObject>("Prefabs/DeckManager");
                if (deckManagerPrefab == null)
                {
                    Debug.Log("cannot find the DECK manager prefab");
                }
                else
                {
                    Instantiate(deckManagerPrefab, transform.position, Quaternion.identity, transform);
                    DeckManager = GetComponentInChildren<DeckManager>();
                }
            }
        }


    }
}