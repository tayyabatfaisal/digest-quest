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
            if (allCards.Count == 0)
            {
                Card[] cards = Resources.LoadAll<Card>("Cards");
                allCards.AddRange(cards);
            }
        }

        public void RelinkSceneReferences(Canvas canvas)
        {
            drawCardButton = GameObject.Find("DrawCardButton")?.GetComponent<Button>();
            drawsLeftText = GameObject.Find("DrawsLeftText")?.GetComponent<TMP_Text>();
            if (drawCardButton != null)
            {
                drawCardButton.onClick.RemoveAllListeners();
                drawCardButton.onClick.AddListener(TryDrawCardButton);
            }
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
            if (allCards.Contains(card))
            {
                allCards.Remove(card);
                if (currentIndex >= allCards.Count)
                {
                    currentIndex = 0;
                }
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

        // Updated: Only populate if hand is empty
        public void TryPopulateInitialHand()
        {
            HandManager hand = GameManager.Instance.HandManager;
            int handCount = hand.cardsInHand.Count;
            int deckCount = allCards.Count;

            int toDraw = Mathf.Min(maxCardsInHand - handCount, deckCount);
            for (int i = 0; i < toDraw; i++)
            {
                DrawCard(hand);
            }
            Debug.Log($"Populated initial hand with {toDraw} cards.");
        }

     public void ResetDeck()
        {
            allCards.Clear();
            Card[] cards = Resources.LoadAll<Card>("Cards");
            allCards.AddRange(cards);
            currentIndex = 0; // Optionally reset draw index
            drawCardUses = 0; // Reset draw counter
            Debug.Log("Deck reset and reloaded with cards: " + allCards.Count);
        }
    }
}