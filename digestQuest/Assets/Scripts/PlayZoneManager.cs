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
        public List<Card> cardDataInPlay = new List<Card>();
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

        // *** FIXED: Move card UI, add card data to play list ***
        public void PlayCard(GameObject card)
        {
            Debug.Log($"[PlayCard] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()}) BEFORE: cardsInPlay.Count={cardsInPlay.Count}");
            if (cardsInPlay.Count >= maxCardsInPlay)
                return;

            Debug.Log("you have decided to move this card to the playzone:" + card.name);

            // Remove from hand references only
            handManager.RemoveCardFromHand(card);

            // Add card data to list
            Card cardData = card.GetComponent<CardDisplay>().card;
            if (!cardDataInPlay.Contains(cardData))
                cardDataInPlay.Add(cardData);

            // Add to play zone list
            if (!cardsInPlay.Contains(card))
            {
                Debug.Log("added the card to the pLAYZONE LIST ");
                cardsInPlay.Add(card);
            }

            // Move in UI
            card.transform.SetParent(playZoneArea, false);

            Debug.Log($"Card '{card.name}' new parent: {card.transform.parent}, playZoneArea: {playZoneArea}");

            UpdatePlayZoneVisuals();

            Debug.Log($"[PlayCard] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()}) AFTER: cardsInPlay.Count={cardsInPlay.Count}");

            if (cardsInPlay.Count == maxCardsInPlay)
            {
                Debug.Log("2 cards in play! Resolve logic here.");
            }
        }

        // Call this after scene change to rebuild play zone UI
        public void RelinkSceneReferences(Canvas canvas)
        {
            Transform playZoneAreaTransform = canvas.transform.Find("PlayZoneArea");
            if (playZoneAreaTransform != null)
                playZoneArea = playZoneAreaTransform;
            else
                Debug.LogError("PlayZoneArea not found in new scene!");

            // Destroy orphaned card visuals
            foreach (var cardGO in cardsInPlay)
            {
                if (cardGO != null) Destroy(cardGO);
            }
            cardsInPlay.Clear();

            // Recreate visuals from cardDataInPlay
            foreach (var cardData in cardDataInPlay)
            {
                GameObject cardGO = Instantiate(handManager.cardPrefab, playZoneArea.position, Quaternion.identity, playZoneArea);
                cardGO.GetComponent<CardDisplay>().card = cardData;
                cardGO.name = cardData.cardName;
                cardsInPlay.Add(cardGO);
            }
            UpdatePlayZoneVisuals();
        }

        public void UpdatePlayZoneVisuals()
        {
            int cardCount = cardsInPlay.Count;
            if (cardCount == 0 || playZoneArea == null) return;

            float cardWidth = cardsInPlay[0].GetComponent<RectTransform>().rect.width;
            float padding = 30f;
            float totalCardWidth = cardCount * cardWidth + (cardCount - 1) * padding;
            float startX = -totalCardWidth / 2f + cardWidth / 2f;

            for (int i = 0; i < cardCount; i++)
            {
                GameObject cardGO = cardsInPlay[i];
                RectTransform cardRect = cardGO.GetComponent<RectTransform>();
                cardRect.SetParent(playZoneArea, false);

                float x = startX + i * (cardWidth + padding);
                cardRect.anchoredPosition = new Vector2(x, 0);
                cardRect.localRotation = Quaternion.identity;
                cardRect.SetSiblingIndex(i);
            }
        }

        public void ResetPlayZone()
        {
            Debug.Log($"[ResetPlayZone] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()})");
            foreach (var card in cardsInPlay)
                Destroy(card);
            cardsInPlay.Clear();
            cardDataInPlay.Clear();
        }

        public void RemoveCardFromPlay(GameObject card)
        {
            Debug.Log($"[RemoveCardFromPlay] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()})");
            int idx = cardsInPlay.IndexOf(card);
            if (idx >= 0)
            {
                cardsInPlay.RemoveAt(idx);
                cardDataInPlay.Remove(card.GetComponent<CardDisplay>().card);
            }
            handManager.AddExistingCardToHand(card);
            UpdatePlayZoneVisuals();
        }

        public void OnDigestButtonClicked()
        {
            Debug.Log($"[Digest] PlayZoneManager on {gameObject.name} (InstanceID: {GetInstanceID()}), cardsInPlay.Count: {cardsInPlay.Count}");
            int totalPoints = 0;

            List<GameObject> digestingCards = new List<GameObject>(cardsInPlay);

            foreach (GameObject cardObj in digestingCards)
            {
                Card cardData = cardObj.GetComponent<CardDisplay>().card;
                Debug.Log("HERE ARE CARDS YOU ARE DIGESTING: " + (cardData != null ? cardData.name : "null"));
                if (cardData != null)
                {
                    totalPoints += 10;
                }

                cardsInPlay.Remove(cardObj);
                cardDataInPlay.Remove(cardData);
                Destroy(cardObj);
            }

            Player.Instance.AddScore(totalPoints);
            Debug.Log("Digest complete! Total points: " + totalPoints);
        }
    }
}