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
            // DO NOT hide the button on start
            // playButton.gameObject.SetActive(false);

            if (deckManager == null)
            { //because the Game Manager instantiates the Deck manager, so I will just set it to the global one
                deckManager = GameManager.Instance.DeckManager;
            }
            if (handManager == null) {
                handManager = GameManager.Instance.HandManager;
            }
        }

        public void PlayCard(GameObject card)
        {
            if (cardsInPlay.Count >= maxCardsInPlay) return;

            Debug.Log("you have decided to move this card to the playzone:" + card.name);

            // Remove from hand (handled by HandManager, see below)
            cardsInPlay.Add(card);
            card.transform.SetParent(playZoneArea, false);

            //updating the hand manager 
            handManager.RemoveCardFromHand(card);

            // Optionally move/animate card to play zone position here

            if (cardsInPlay.Count == maxCardsInPlay)
            {
                // Trigger game logic for "2 cards selected"
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

            //AND WE MUST also add it back to the hand
            handManager.AddExistingCardToHand(card);
        }

    }
}