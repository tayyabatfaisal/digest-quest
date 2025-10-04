using UnityEngine;
using System.Collections.Generic; // You need this for List<T>

namespace DigestQuest
{
    public class HandManager : MonoBehaviour
    {
        public GameObject cardPrefab;
        public Transform handTransform;
        public float fanSpread = 2f;

        public List<GameObject> cardsInHand = new List<GameObject>();

        void Start()
        {
            AddCardToHand();
            AddCardToHand();
            AddCardToHand();
        }

        public void AddCardToHand()
        {
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);
            UpdateHandVisuals();
        }

        public void UpdateHandVisuals()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0) return;

            float totalWidth = (cardCount - 1) * fanSpread;
            Vector3 handCenter = handTransform.position;

            for (int i = 0; i < cardCount; i++)
            {
                float xOffset = (i * fanSpread) - (totalWidth / 2f);
                Vector3 cardPos = handCenter + new Vector3(xOffset, 0, 0);
                cardsInHand[i].transform.position = cardPos;
                cardsInHand[i].transform.rotation = Quaternion.identity;
            }
        }
    }
}