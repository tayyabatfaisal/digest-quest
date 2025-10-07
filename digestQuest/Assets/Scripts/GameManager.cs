using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{
    public class GameManager : MonoBehaviour //a singleton, only one instance of this class should exist in a game session lifetime
    {
        //init variables
        private int playerScore = 0;

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
            InitialiseManagers();
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

            // --- HandManager instantiation and HandPosition area setup ---
            if (HandManager == null)
            {
                GameObject handManagerPrefab = Resources.Load<GameObject>("Prefabs/HandManager");
                if (handManagerPrefab == null)
                {
                    Debug.Log("cannot find the HandManager prefab");
                }
                else
                {
                    Canvas canvas = FindObjectOfType<Canvas>();
                    if (canvas == null)
                    {
                        Debug.LogError("No Canvas found in the scene! Cannot create HandPosition.");
                        return;
                    }

                    Transform handPositionTransform = canvas.transform.Find("HandPosition");
                    RectTransform handRect;
                    if (handPositionTransform == null)
                    {
                        GameObject handPositionGO = new GameObject("HandPosition", typeof(RectTransform));
                        handRect = handPositionGO.GetComponent<RectTransform>();
                        handRect.SetParent(canvas.transform, false);

                        handRect.anchorMin = new Vector2(0.5f, 0f);
                        handRect.anchorMax = new Vector2(0.5f, 0f);
                        handRect.pivot = new Vector2(0.5f, 0.5f);
                        handRect.anchoredPosition = new Vector2(300f, 600f); // Centered horizontally, 246px above bottom
                        handRect.sizeDelta = new Vector2(1679.708f, 400f);     // 1000px wide, 200px tall

                        // Add HorizontalLayoutGroup for automatic card arrangement
                        var layout = handPositionGO.AddComponent<HorizontalLayoutGroup>();
                        layout.childAlignment = TextAnchor.MiddleCenter;
                        layout.spacing = 275f; // Adjust for card separation
                        layout.padding = new RectOffset(20, 20, 20, 20); // Edge spacing
                        layout.childForceExpandWidth = false;
                        layout.childForceExpandHeight = false;
                        layout.childControlWidth = true;
                        layout.childControlHeight = true;

                        // Optional debug background
                        var img = handPositionGO.AddComponent<UnityEngine.UI.Image>();
                        img.color = new Color(0f, 0f, 0f, 0.1f);
                    }
                    else
                    {
                        handRect = handPositionTransform.GetComponent<RectTransform>();
                        var layout = handRect.GetComponent<HorizontalLayoutGroup>();
                        if (layout == null)
                        {
                            layout = handRect.gameObject.AddComponent<HorizontalLayoutGroup>();
                            layout.childAlignment = TextAnchor.MiddleCenter;
                            layout.spacing = 20f;
                            layout.padding = new RectOffset(20, 20, 20, 20);
                            layout.childForceExpandWidth = false;
                            layout.childForceExpandHeight = false;
                            layout.childControlWidth = true;
                            layout.childControlHeight = true;
                        }
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

        public int PlayerScore
        {
            get { return playerScore; }
            set { playerScore = value; }
        }
    }
}