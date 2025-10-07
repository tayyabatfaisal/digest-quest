using UnityEngine;
using System.Collections.Generic;

namespace DigestQuest
{
    public class HandManager : MonoBehaviour
    {
        public int maxHandSize = 5;
        public DeckManager deckManager;
        public GameObject cardPrefab;
        public Transform handTransform;
        public float cardSpacing = 3.5f;
        public List<GameObject> cardsInHand = new List<GameObject>();

        void Start()
        {
            handTransform.position = handTransform.position + new Vector3(-100f, -250f, 0f);
        }

        public void AddCardToHand(Card cardData)
        {
            if (cardsInHand.Count >= maxHandSize)
            {
                Debug.Log("Hand is full!");
                return;
            }

            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);
            newCard.GetComponent<CardDisplay>().card = cardData;
            newCard.name = cardData.cardName;

            UpdateHandVisuals();
        }

        public void RemoveCardFromHand(GameObject card)
        {
            // Only remove if it's actually in the hand
            if (cardsInHand.Contains(card))
            {
                cardsInHand.Remove(card);
                UpdateHandVisuals();
            }
        }

        public void UpdateHandVisuals()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0) return;

            RectTransform handRect = handTransform.GetComponent<RectTransform>();
            float usableWidth = handRect.rect.width;

            // Calculate spacing so cards fill the entire available width (with padding)
            float cardWidth = cardsInHand[0].GetComponent<RectTransform>().rect.width;
            float padding = 20f; // space between cards, adjust as needed
            float totalCardWidth = cardCount * cardWidth + (cardCount - 1) * padding;
            float startX = -totalCardWidth / 2f + cardWidth / 2f;

            for (int i = 0; i < cardCount; i++)
            {
                GameObject cardGO = cardsInHand[i];
                RectTransform cardRect = cardGO.GetComponent<RectTransform>();
                cardRect.SetParent(handTransform, false);

                float x = startX + i * (cardWidth + padding);
                cardRect.anchoredPosition = new Vector2(x, 0);
                cardRect.localRotation = Quaternion.identity;
                cardRect.SetSiblingIndex(i);
            }
        }

        public void AddExistingCardToHand(GameObject card)
        {
            // Only add if not already in hand
            if (!cardsInHand.Contains(card))
            {
                card.transform.SetParent(handTransform, false);
                cardsInHand.Add(card);
                card.transform.SetAsLastSibling();
                card.transform.localScale = Vector3.one;
                card.transform.localRotation = Quaternion.identity;
                Debug.Log($"this is the card that is coming back into the hand:   " + card.name);
                UpdateHandVisuals();
            }
        }
    }
}