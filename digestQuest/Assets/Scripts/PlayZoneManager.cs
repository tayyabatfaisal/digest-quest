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

            // Move in UI
            card.transform.SetParent(playZoneArea, false);

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
    }
}