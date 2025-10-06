using UnityEngine;
using System.Collections.Generic;

namespace DigestQuest
{
    public class HandManager : MonoBehaviour
    {

        public int maxHandSize = 5; //5 cards in a hand at a time

        public DeckManager deckManager;

        public GameObject cardPrefab; //the prefab to instantiate when adding a new card to the hand
        public Transform handTransform;

        public float cardSpacing = 3.5f; // Try 3.5, 4, or 5 for a little extra space

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

            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform); //make a game object to reference for UI
            //you need a gameobject for visuals 
            cardsInHand.Add(newCard);
            //set the cardData of the instantiated card
            newCard.GetComponent<CardDisplay>().card = cardData;
            newCard.name = cardData.cardName;

            UpdateHandVisuals();
        }

        public void RemoveCardFromHand(GameObject card) //it gets just the reference though?
        {
            if (cardsInHand.Contains(card))
                cardsInHand.Remove(card);
 
            UpdateHandVisuals();
        }

        public void UpdateHandVisuals()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0) return;

            // Start at the left edge of your hand area
            Vector3 startPosition = handTransform.position; // This is your pre-set hand position
            float xSpacing = cardSpacing;                   // Space between cards

            for (int i = 0; i < cardCount; i++)
            {
                Vector3 cardPos = startPosition + new Vector3(i * xSpacing, 0, 0);
                cardsInHand[i].transform.position = cardPos;
                cardsInHand[i].transform.rotation = Quaternion.identity;
                cardsInHand[i].transform.SetSiblingIndex(i); // Ensures draw order in UI
            }

            for (int i = 0; i < cardsInHand.Count; i++)
            {
                Debug.Log($"Hand [{i}]: {cardsInHand[i].name} at {cardsInHand[i].transform.position}");
            }
        }

        public void AddExistingCardToHand(GameObject card)
        {

            card.transform.SetParent(handTransform, false);
            cardsInHand.Add(card); // add to end
            card.transform.SetAsLastSibling(); // ensure it's drawn above others
            Debug.Log($"this is the card that is coming back :   " + card.name);

            // Reset local scale/rotation, if needed
            card.transform.localScale = Vector3.one;
            card.transform.localRotation = Quaternion.identity;

            UpdateHandVisuals();
        }

    }
}