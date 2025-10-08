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

        public List<Card> cardDataList = new List<Card>(); // cards DATA of cards in hand
        public List<GameObject> cardsInHand = new List<GameObject>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (handTransform != null)
                handTransform.position += new Vector3(-100f, -250f, 0f);
            UpdateHandVisuals();
        }

        public void AddCardToHand(Card cardData)
        {
            if (cardDataList.Count >= maxHandSize)
            {
                Debug.Log("Hand is full!");
                return;
            }
            cardDataList.Add(cardData);
            CreateCardVisual(cardData);
            UpdateHandVisuals();
        }

        // Only remove from lists, do NOT Destroy card!
        public void RemoveCardFromHand(GameObject card)
        {
            int idx = cardsInHand.IndexOf(card);
            if (idx >= 0)
            {
                cardsInHand.RemoveAt(idx);
                cardDataList.Remove(card.GetComponent<CardDisplay>().card);
                UpdateHandVisuals();
            }
        }

        private void CreateCardVisual(Card cardData)
        {
            if (handTransform == null)
            {
                Debug.LogWarning("HandTransform is not set!");
                return;
            }
            Debug.LogWarning("recreating ");
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);
            newCard.GetComponent<CardDisplay>().card = cardData;
            newCard.name = cardData.cardName;
        }

        // Re-link handTransform and rebuild visuals
        public void RelinkSceneReferences(Canvas canvas)
        {
            Transform handPositionTransform = canvas.transform.Find("HandPosition");
            if (handPositionTransform != null)
                handTransform = handPositionTransform.GetComponent<RectTransform>();
            else
                Debug.LogWarning("HandPosition not found in new scene!");

            // Destroy all current card UI objects
            foreach (var cardGO in cardsInHand)
            {
                if (cardGO != null) Destroy(cardGO);
            }
            cardsInHand.Clear();

            // Recreate card visuals from persistent cardDataList
            foreach (var cardData in cardDataList)
            {
                CreateCardVisual(cardData);
            }
            UpdateHandVisuals();
        }

        public void UpdateHandVisuals()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0 || handTransform == null) return;

            RectTransform handRect = handTransform.GetComponent<RectTransform>();
            float cardWidth = cardsInHand[0].GetComponent<RectTransform>().rect.width;
            float padding = 20f;
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
            Card cardData = card.GetComponent<CardDisplay>().card;

            // Add card data if not already present
            if (!cardDataList.Contains(cardData))
            {
                cardDataList.Add(cardData);
            }

            // Add UI object if not already present
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