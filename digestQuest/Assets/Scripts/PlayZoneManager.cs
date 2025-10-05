using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DigestQuest
{
    public class PlayZoneManager : MonoBehaviour
    {
        public Transform playZoneArea; //marks the area of the play zone 
        public int maxCardsInPlay = 2;
        public List<GameObject> cardsInPlay = new List<GameObject>();
        public List<Card> cardsDataInPlay = new List<Card>();

        public void PlayCard(GameObject card)
        {
            if (cardsInPlay.Count >= maxCardsInPlay) return;

            // Remove from hand (handled by HandManager, see below)
            cardsInPlay.Add(card);
            card.transform.SetParent(playZoneArea, false);
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
        }

    }
}
