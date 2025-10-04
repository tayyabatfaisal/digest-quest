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
            if (allCards.Count == 0) return;
            Card nextCard = allCards[currentIndex];
            handManager.AddCardToHand(nextCard);
            currentIndex = (currentIndex + 1) % allCards.Count;
        }
    }
}