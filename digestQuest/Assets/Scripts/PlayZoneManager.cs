using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI; // For Button

namespace DigestQuest
{
    public class PlayZoneManager : MonoBehaviour
    {
        public Button playButton;
        public Transform playZoneArea; //marks the area of the play zone 
        public int maxCardsInPlay = 2; //set max cards to play into the play zone
        public List<GameObject> cardsInPlay = new List<GameObject>();
        public List<Card> cardsDataInPlay = new List<Card>();
        public HandManager handManager;
        public DeckManager deckManager;

        void Start()
        {
            playButton.gameObject.SetActive(false); // Hide initially

            if (deckManager == null) { //because the Game Manager instantiates the Deck manager, so I will just set it to the global one
                deckManager = GameManager.Instance.DeckManager;
            }
        }

        public void PlayCard(GameObject card)
        {
            if (cardsInPlay.Count >= maxCardsInPlay) return;

            Debug.Log("you have played the following card:" + card.name);

            // Remove from hand (handled by HandManager, see below)
            cardsInPlay.Add(card);
            card.transform.SetParent(playZoneArea, false);


            // Optionally move/animate card to play zone position here

            if (cardsInPlay.Count == maxCardsInPlay)
            {
                // Trigger game logic for "2 cards selected"
                Debug.Log("2 cards in play! Resolve logic here.");
            }

            // Update Play button
            playButton.gameObject.SetActive(cardsInPlay.Count > 0);
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


            // Update Play button
            playButton.gameObject.SetActive(cardsInPlay.Count > 0);
        }


        //link the digest button to this 
        public void OnPlayButtonClicked()
        {
            int points = 0;

            foreach (GameObject cardObj in cardsInPlay)
            {
                // Get Card data
                Card cardData = cardObj.GetComponent<CardDisplay>().card;
                Debug.Log("CARD DATA OBTAINED FOR : "+ cardData.name);
                if (cardData != null)
                    //POINTS TALLY WHEN YOU PLAY THE CARD 
                    // points += cardData.points;  // Assuming Card has a points field

                    // Remove from hand and deck
                    handManager.RemoveCardFromHand(cardObj);
                deckManager.RemoveFromDeck(cardData);

                // Destroy card GameObject
                Destroy(cardObj);
            }

            // scoreManager.AddToScore(points); // Update running score
            cardsInPlay.Clear();

            // Hide Play button
            playButton.gameObject.SetActive(false);

        }

    }
}
