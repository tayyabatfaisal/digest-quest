using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DigestQuest
{
    public class CardDisplay : MonoBehaviour
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



    }
}