using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // NEW INPUT SYSTEM

namespace DigestQuest
{
    public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {

        private RectTransform rectTransform; //of the current card object 
        private Canvas canvas;
        private Vector2 originalLocalPointerPosition; //store og location of the card
        private Vector3 originalPanelLocalPosition;
        private Vector3 originalScale;
        private int currentState = 0;
        //private Quaternion originalRotation;
        private Vector3 originalPosition;

        [SerializeField] private float selectScale = 1.1f; //set scale when we hover over the card 
        [SerializeField] private Vector2 cardPlay;
        [SerializeField] private Vector3 playPosition;
        [SerializeField] private GameObject glowEffect;
        [SerializeField] private GameObject playArrow;

        void Awake()//when this object is activated
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>(); //from the parent canvas
            originalScale = rectTransform.localScale;
            originalPosition = rectTransform.localPosition;
            //originalRotation = rectTransform.localRotation;
        }

        void Update()
        {
            switch (currentState) //switches between different cases (e.g switch case for each type)
            {
                case 1: //if the currentState = 1
                    HandleHoverState();
                    break;
                case 2:
                    HandleDragState();
                    if (!Mouse.current.leftButton.isPressed) // NEW INPUT SYSTEM
                    {
                        TransitionToState0();
                    }
                    break;
                case 3:
                    HandlePlayState();
                    if (!Mouse.current.leftButton.isPressed) // NEW INPUT SYSTEM
                    {
                        TransitionToState0();
                    }
                    break;
            }
        }

        //write out the methods
        private void TransitionToState0()
        {
            currentState = 0;
            rectTransform.localScale = originalScale; //reset state
            rectTransform.localPosition = originalPosition; //reset position 
            glowEffect.SetActive(false); //disable the glow effect 
            playArrow.SetActive(false); //disable play arrow 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentState == 0)
            {
                originalPosition = rectTransform.localPosition;
                originalScale = rectTransform.localScale;

                currentState = 1;
            }
        }

        public void HandleHoverState()
        {
            glowEffect.SetActive(true);
            rectTransform.localScale = originalScale * selectScale;
        }

        private void HandleDragState()
        {
            // the tutorial talks about rotation but im not doing ts
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (currentState == 1)
            {
                TransitionToState0();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (currentState == 1)
            {
                currentState = 2;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
                //above makes sure we get the correct position in relation to the camera and the world 

                originalPanelLocalPosition = rectTransform.localPosition;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (currentState == 2)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition))
                {
                    localPointerPosition /= canvas.scaleFactor;
                    Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

                    rectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;

                    if (rectTransform.localPosition.y > cardPlay.y)
                    {
                        currentState = 3;
                        playArrow.SetActive(true);
                        rectTransform.localPosition = playPosition;
                    }
                }
            }
        }

        private void HandlePlayState()
        {
            rectTransform.localPosition = playPosition;

            // NEW INPUT SYSTEM: use Mouse.current.position.ReadValue()
            if (Mouse.current.position.ReadValue().y < cardPlay.y)
            {
                currentState = 2;
                playArrow.SetActive(false);
            }
        }
    }
}