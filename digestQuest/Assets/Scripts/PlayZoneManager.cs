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
        }

        public void RelinkSceneReferences(Canvas canvas)
        {
            Transform playZoneAreaTransform = canvas.transform.Find("PlayZoneArea");
            playZoneArea = playZoneAreaTransform;
            // Optionally, rebuild play zone UI from cardDataInPlay here if needed
        }

        public void PlayCard(GameObject card)
        {
            if (cardsInPlay.Count >= maxCardsInPlay)
                return;

            handManager.RemoveCardFromHand(card);

            Card cardData = card.GetComponent<CardDisplay>().card;
            if (!cardDataInPlay.Contains(cardData))
                cardDataInPlay.Add(cardData);

            if (!cardsInPlay.Contains(card))
                cardsInPlay.Add(card);

            card.transform.SetParent(playZoneArea, false);
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
            foreach (var card in cardsInPlay)
                Destroy(card);
            cardsInPlay.Clear();
            cardDataInPlay.Clear();
        }

        public void RemoveCardFromPlay(GameObject card)
        {
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
            int totalPoints = 0;
            List<GameObject> digestingCards = new List<GameObject>(cardsInPlay);

            foreach (GameObject cardObj in digestingCards)
            {
                Card cardData = cardObj.GetComponent<CardDisplay>().card;
                if (cardData != null)
                {
                    totalPoints += 10; // dummy value
                }

                cardsInPlay.Remove(cardObj);
                cardDataInPlay.Remove(cardData);
                Destroy(cardObj);
            }

            Player.Instance.AddScore(totalPoints);
        }
    }
}