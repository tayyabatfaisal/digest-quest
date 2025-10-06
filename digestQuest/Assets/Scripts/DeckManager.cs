using UnityEngine;
using System.Collections.Generic;

namespace DigestQuest
{
    public class DeckManager : MonoBehaviour // <--- Must inherit MonoBehaviour!
    //only monobehaviour scripts can be attatched to game objects 
    {
        public List<Card> allCards = new List<Card>();
        private int currentIndex = 0;


        void Start()
        {
            //load all card assets from the cardData resrouces 
            Card[] cards = Resources.LoadAll<Card>("Cards");

            //add the loaded cards to the allCards list
            allCards.AddRange(cards);

            HandManager hand = FindObjectOfType<HandManager>();

            for (int i = 0; i < 6; i++)
            {
                DrawCard(hand);
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


    }
}