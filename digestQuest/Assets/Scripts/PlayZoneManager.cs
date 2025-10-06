using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{
    public class PlayZoneManager : MonoBehaviour
    {
        public Button digestButton;
        public Transform playZoneArea;
        public int maxCardsInPlay = 2;
        public List<GameObject> cardsInPlay = new List<GameObject>();
        public HandManager handManager;
        public DeckManager deckManager;

        void Awake()
        {
            Debug.Log($"[Awake] PlayZoneManager on {gameObject.name}, InstanceID: {GetInstanceID()}");
        }

        void Start()
        {
            Debug.Log($"[Start] PlayZoneManager on {gameObject.name}, InstanceID: {GetInstanceID()}");
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
            Debug.Log($"[PlayCard] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()}) BEFORE: cardsInPlay.Count={cardsInPlay.Count}");
            if (cardsInPlay.Count >= maxCardsInPlay)
                return;

            Debug.Log("you have decided to move this card to the playzone:" + card.name);

            // Remove from hand first
            handManager.RemoveCardFromHand(card);

            // Add to play zone list
            if (!cardsInPlay.Contains(card))
            {
                Debug.Log("added the card to the pLAYZONE LIST ");
                cardsInPlay.Add(card);
            }

            //debug purpose
            Debug.Log($"playZoneArea ref: {playZoneArea}, parent: {playZoneArea?.parent}");

            // Move in UI
            card.transform.SetParent(playZoneArea, false);

            //debug for after setting parent
            Debug.Log($"Card '{card.name}' new parent: {card.transform.parent}, playZoneArea: {playZoneArea}");

            Debug.Log($"[PlayCard] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()}) AFTER: cardsInPlay.Count={cardsInPlay.Count}");

            if (cardsInPlay.Count == maxCardsInPlay)
            {
                Debug.Log("2 cards in play! Resolve logic here.");
            }
        }

        public void ResetPlayZone()
        {
            Debug.Log($"[ResetPlayZone] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()})");
            foreach (var card in cardsInPlay)
                Destroy(card);
            cardsInPlay.Clear();
        }

        public void RemoveCardFromPlay(GameObject card)
        {
            Debug.Log($"[RemoveCardFromPlay] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()})");
            if (cardsInPlay.Contains(card))
                cardsInPlay.Remove(card);

            // Move card back to hand (UI and logic)
            handManager.AddExistingCardToHand(card);
        }

        public void OnDigestButtonClicked()
        {
            Debug.Log($"[Digest] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()}), cardsInPlay.Count: {cardsInPlay.Count}");
            int totalPoints = 0;

            // Make a copy of the list to avoid modification during iteration
            List<GameObject> digestingCards = new List<GameObject>(cardsInPlay);

            Debug.Log("[Digest] Contents of cardsInPlay:");
            foreach (var c in cardsInPlay)
            {
                Debug.Log($"[Digest] - {c.name}");
            }

            foreach (GameObject cardObj in digestingCards)
            {
                // Get Card data (if needed for points)
                Card cardData = cardObj.GetComponent<CardDisplay>().card;
                Debug.Log("HERE ARE CARDS YOU ARE DIGESTING: " + (cardData != null ? cardData.name : "null"));
                if (cardData != null)
                {
                    totalPoints += 10; //SET 10 AS A DUMMY VALUE FOR NOW
                }

                // Remove from play zone list
                cardsInPlay.Remove(cardObj);

                // Destroy the card UI object
                Destroy(cardObj);
            }

            // Optionally update score
            Debug.Log("Digest complete! Total points: " + totalPoints);
        }
    }
}