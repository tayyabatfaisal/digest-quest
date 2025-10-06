using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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