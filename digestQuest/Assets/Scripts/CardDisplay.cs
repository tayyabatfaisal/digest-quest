using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace DigestQuest
{
    public class CardDisplay : MonoBehaviour, IPointerClickHandler
    {
        public Card card;

        //other potential variables we will need to display on the card, but for now i will just parse the name

        public Image cardImage;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text typeText;
        public TMP_Text stageRequirementText;
        public TMP_Text environmentRequirementText;
        public TMP_Text pointsText;

        void Start()
        {
            UpdateCardDisplay();
        }

        //the function for updating the card
        public void UpdateCardDisplay()
        {
            nameText.text = card.cardName;
            //set the text ON the card to the card object's name (e.g. amylase)
            //usethe class nameText with method to.text to specifically set it


            //add other card features below:
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"the card has been clicked");
            // Only allow play if in hand (you can add checks or state if needed)
            PlayZoneManager playZone = FindObjectOfType<PlayZoneManager>();
            HandManager handManager = FindObjectOfType<HandManager>();

            playZone.PlayCard(this.gameObject);
            handManager.RemoveCardFromHand(this.gameObject);
        }


    }
}