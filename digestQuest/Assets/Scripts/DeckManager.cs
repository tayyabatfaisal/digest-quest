using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace DigestQuest
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance { get; private set; }

        public List<Card> allCards = new List<Card>();
        private int currentIndex = 0;
        private int maxCardsInHand = 5;

        public int drawCardUses = 0;
        public int maxDrawCardUses = 2;

        public Button drawCardButton;
        public TMP_Text drawsLeftText;

        private void Awake()
        {
            // Make persistent
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // Only load cards once
            if (allCards.Count == 0)
            {
                Card[] cards = Resources.LoadAll<Card>("Cards");
                allCards.AddRange(cards);
            }

            // Do NOT auto-populate the hand here!
            // Only draw cards when starting the game, not on every scene load

            // Set up draw card button
            if (drawCardButton != null)
            {
                drawCardButton.onClick.RemoveAllListeners();
                drawCardButton.onClick.AddListener(TryDrawCardButton);
            }

            // Set up draws left text
            drawsLeftText = GameObject.Find("DrawsLeftText")?.GetComponent<TMP_Text>();
            if (drawsLeftText == null)
                Debug.LogWarning("DrawsLeftText TMP_Text not found in scene!");
            UpdateDrawsLeftText();
        }

        public void DrawCard(HandManager handManager)
        {
            if (allCards.Count == 0)
            {
                Debug.Log($"NO MORE CARDS IN THE DECK");
                return;
            }
            Card nextCard = allCards[currentIndex];
            handManager.AddCardToHand(nextCard);
            currentIndex = (currentIndex + 1) % allCards.Count;
        }

        public void RemoveFromDeck(Card card)
        {
            Debug.Log("REMOVEING THIS CARD FROM DECK: " + card.name);
            if (allCards.Contains(card))
            {
                allCards.Remove(card);
                if (currentIndex >= allCards.Count)
                {
                    currentIndex = 0;
                }
                Debug.Log("Removed card from deck: " + card.cardName);
            }
            else
            {
                Debug.LogWarning("Tried to remove card not in deck: " + card.cardName);
            }
        }

        public void TryDrawCardButton()
        {
            HandManager handManager = GameManager.Instance.HandManager;
            PlayZoneManager playZoneManager = GameManager.Instance.PlayZoneManager;

            int handCount = handManager.cardsInHand.Count;
            int playZoneCount = playZoneManager.cardsInPlay.Count;

            if ((handCount + playZoneCount) >= maxCardsInHand)
            {
                Debug.Log("You still have 5 cards to play with! Can't draw more.");
                return;
            }
            if (drawCardUses >= maxDrawCardUses)
            {
                Debug.Log("No draws left!");
                return;
            }
            if (allCards.Count == 0)
            {
                Debug.Log("NO MORE CARDS IN THE DECK");
                return;
            }

            drawCardUses++;
            DrawCard(handManager);
            UpdateDrawsLeftText();
        }

        public void UpdateDrawsLeftText()
        {
            if (drawsLeftText != null)
            {
                int drawsLeft = maxDrawCardUses - drawCardUses;
                drawsLeftText.text = $"Draws Left: {drawsLeft}";
            }
        }

        // Call this ONCE at game start, not on every scene load!
        public void PopulateInitialHand()
        {
            HandManager hand = GameManager.Instance.HandManager;
            for (int i = 0; i < maxCardsInHand; i++)
            {
                DrawCard(hand);
            }
        }
    }
}