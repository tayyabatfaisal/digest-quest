using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{
    public class DeckManager : MonoBehaviour // <--- Must inherit MonoBehaviour!
    //only monobehaviour scripts can be attatched to game objects 
    {
        public List<Card> allCards = new List<Card>();
        private int currentIndex = 0;

        private int maxCardsInHand = 5;

        public int drawCardUses = 0;
        public int maxDrawCardUses = 2;

        // FOR THE BUTTON to darw the cards
        public Button drawCardButton;

        void Start()
        {
            //load all card assets from the cardData resrouces 
            Card[] cards = Resources.LoadAll<Card>("Cards");

            //add the loaded cards to the allCards list
            allCards.AddRange(cards);

            //find the hand manager 
            HandManager hand = FindObjectOfType<HandManager>();

            //populate the hand with random cards 
            for (int i = 0; i <= maxCardsInHand; i++)
            {
                DrawCard(hand);
            }

            //attatching the drawCardButton
            if (drawCardButton != null)
            {
                drawCardButton.onClick.RemoveAllListeners();
                drawCardButton.onClick.AddListener(TryDrawCardButton);
            }
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
                // Optionally adjust currentIndex if needed
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
            HandManager handManager = FindObjectOfType<HandManager>();
            if (handManager.cardsInHand.Count >= 5)
            {
                Debug.Log("Hand is full!");
                return;
            }
            if (drawCardUses >= maxDrawCardUses)
            {
                Debug.Log("No draws left!");
                return;
            }
            drawCardUses++;
            DrawCard(handManager);
        }


    }
}