using UnityEngine;
using System.Collections.Generic;

namespace DigestQuest
{
    public class HandManager : MonoBehaviour
    {

        public int maxHandSize = 5; //5 cards in a hand at a time

        public DeckManager deckManager;

        public GameObject cardPrefab;
        public Transform handTransform;

        public float cardSpacing = 375; // from trial and error on the game

        public List<GameObject> cardsInHand = new List<GameObject>();

        void Start()
        {
            handTransform.position = handTransform.position + new Vector3(-100f, -250f, 0f); // moves hand down by 50 units
        }

        void Update()
        {
            
            //UpdateHandVisuals();
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

            //set the cardData of the instantiated card
            newCard.GetComponent<CardDisplay>().card = cardData;

            UpdateHandVisuals();
        }

        public void UpdateHandVisuals()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0) return;

            float totalWidth = (cardCount - 1) * cardSpacing;
            Vector3 handCenter = handTransform.position;

            for (int i = 0; i < cardCount; i++)
            {
                float xOffset = (i * cardSpacing) - (totalWidth / 2f);
                Vector3 cardPos = handCenter + new Vector3(xOffset, 0, 0);
                cardsInHand[i].transform.position = cardPos;
                cardsInHand[i].transform.rotation = Quaternion.identity;
            }
        }
    }
}