using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{
    public class PlayZoneManager : MonoBehaviour
    {
        public Button playButton;
        public Transform playZoneArea;
        public int maxCardsInPlay = 2;
        public List<GameObject> cardsInPlay = new List<GameObject>();
        public HandManager handManager;
        public DeckManager deckManager;

        void Start()
        {

            if (deckManager == null)
                deckManager = GameManager.Instance.DeckManager;
            if (handManager == null)
                handManager = GameManager.Instance.HandManager;
            Debug.Log($"[PlayZoneManager] playZoneArea in Start: {playZoneArea}");
                if (playZoneArea == null)
                    Debug.LogError("[PlayZoneManager] playZoneArea is NULL in Start! Check assignment.");
        }

        public void PlayCard(GameObject card)
        {
            if (cardsInPlay.Count >= maxCardsInPlay)
                return;

            Debug.Log("you have decided to move this card to the playzone:" + card.name);

            // Remove from hand first
            handManager.RemoveCardFromHand(card);

            // Add to play zone list
            if (!cardsInPlay.Contains(card))
                cardsInPlay.Add(card);

            //debug purpose
            Debug.Log($"playZoneArea ref: {playZoneArea}, parent: {playZoneArea?.parent}");

            // Move in UI
            card.transform.SetParent(playZoneArea, false);

            //debug for after setting parent
            Debug.Log($"Card '{card.name}' new parent: {card.transform.parent}, playZoneArea: {playZoneArea}");

            if (cardsInPlay.Count == maxCardsInPlay)
            {
                Debug.Log("2 cards in play! Resolve logic here.");
            }
        }

        public void ResetPlayZone()
        {
            foreach (var card in cardsInPlay)
                Destroy(card);
            cardsInPlay.Clear();
        }

        public void RemoveCardFromPlay(GameObject card)
        {
            if (cardsInPlay.Contains(card))
                cardsInPlay.Remove(card);

            // Move card back to hand (UI and logic)
            handManager.AddExistingCardToHand(card);
        }


        public void OnDigestButtonClicked()
        {
            int totalPoints = 0;

            // Copy cardsInPlay to avoid modification during iteration
            var digestingCards = new List<GameObject>(cardsInPlay);

            foreach (GameObject cardObj in digestingCards)
            {
                // Get card data
                Card cardData = cardObj.GetComponent<CardDisplay>().card;

                if (cardData != null)
                {
                    // Tally points
                    totalPoints += 10; // DUMMY VARIABLE SET TO 10 WHEN TALLYING AFTER YOU CLICK <---- CHANGE THIS 

                    // Remove from hand (if present)
                    handManager.RemoveCardFromHand(cardObj);

                    // Remove from deck (if present)
                    deckManager.RemoveFromDeck(cardData);
                }

                // Remove from play zone
                cardsInPlay.Remove(cardObj);

                // Destroy the card UI object
                Destroy(cardObj);
            }

            // Optionally, update your score manager
            // scoreManager.AddToScore(totalPoints);

            Debug.Log("Digest complete! Total points: " + totalPoints);
        }

    }
}