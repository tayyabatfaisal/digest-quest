using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{

    public class GameManager : MonoBehaviour //a singletone, only one instance of this class should exist in a game session lifetime 
    {
        // STATIC METHOD BECAUSE EVERYONE SHOULD ACCESS THIS AND SHOULD BE ONLY ONE



        //init variables
        private int playerScore = 0;


        public static GameManager Instance { get; private set; } //can be gotten by other classes, but only set privately (self)

        public OptionsManager OptionsManager { get; private set; }
        public AudioManager AudioManager { get; private set; } //this must also persist between scenes
        public DeckManager DeckManager { get; private set; }
        public HandManager HandManager { get; private set; } //same hand manager throughout all scenes


        //now also moving playzonearea here

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


            //creating a playzone manager 

            // --- PlayZoneManager instantiation ---
            if (PlayZoneManager == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayZoneManager");
                if (prefab == null)
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
                        // If not found, create it
                        GameObject playZoneAreaGO = new GameObject("PlayZoneArea", typeof(RectTransform));
                        playZoneAreaGO.transform.SetParent(canvas.transform, false);
                        playZoneAreaTransform = playZoneAreaGO.transform;
                        // Set up RectTransform properties as needed...
                    }
                    PlayZoneArea = playZoneAreaTransform;

                    // Instantiate PlayZoneManager under Canvas (for UI)
                    GameObject playZoneManagerGO = Instantiate(prefab, canvas.transform);
                    PlayZoneManager = playZoneManagerGO.GetComponent<PlayZoneManager>();

                    // Assign dependencies
                    PlayZoneManager.playZoneArea = PlayZoneArea;
                    PlayZoneManager.handManager = HandManager;
                    PlayZoneManager.deckManager = DeckManager;
                    // Assign playButton if needed:
                    // PlayZoneManager.playButton = ... (find or create your button under Canvas)
                }
            }


            if (HandManager == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/HandManager"); //create a hand manager object
                if (prefab == null)
                {
                    Debug.Log("cannot find the HandManager prefab");
                }
                else
                {
                    // Find the Canvas in the scene
                    Canvas canvas = FindObjectOfType<Canvas>();
                    if (canvas == null)
                    {
                        Debug.LogError("No Canvas found in the scene! Cannot create HandPosition.");
                        return;
                    }

                    // Create HandPosition GameObject
                    GameObject handPositionGO = new GameObject("HandPosition", typeof(RectTransform));
                    RectTransform handRect = handPositionGO.GetComponent<RectTransform>();
                    handRect.SetParent(canvas.transform, false);

                    // Set anchors and pivot to center (or adjust as needed)
                    handRect.anchorMin = new Vector2(0.5f, 0.5f);
                    handRect.anchorMax = new Vector2(0.5f, 0.5f);
                    handRect.pivot = new Vector2(0.5f, 0.5f);

                    // Set your desired anchored position
                    handRect.anchoredPosition = new Vector2(-375f, -170f);

                    // Set size
                    handRect.sizeDelta = new Vector2(400, 100);

                    // Instantiate HandManager and assign handTransform
                    GameObject handManagerGO = Instantiate(prefab, transform.position, Quaternion.identity, transform);
                    HandManager = handManagerGO.GetComponent<HandManager>();
                    HandManager.handTransform = handRect;
                }
            }
                if (OptionsManager == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsManager"); //from the resources folder 
                if (prefab == null)
                {
                    Debug.Log($"cannot find the optionsmanager prefab");
                }
                else
                {
                    Instantiate(prefab, transform.position, Quaternion.identity, transform);
                    OptionsManager = GetComponentInChildren<OptionsManager>();
                }
            }

            if (AudioManager == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/AudioManager"); //from the resources folder 
                if (prefab == null)
                {
                    Debug.Log($"cannot find the Audio manager prefab");
                }
                else
                {
                    Instantiate(prefab, transform.position, Quaternion.identity, transform);
                    AudioManager = GetComponentInChildren<AudioManager>();
                }
            }

            if (DeckManager == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/DeckManager"); //from the resources folder 
                if (prefab == null)
                {
                    Debug.Log($"cannot find the DECK manager prefab");
                }
                else
                {
                    Instantiate(prefab, transform.position,Quaternion.identity, transform);
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