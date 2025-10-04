using UnityEngine;

namespace DigestQuest //namespace for organisation
{
    public enum CardType { Enzyme, Environment, Obstacle, Special }

    [CreateAssetMenu(fileName = "NewCard", menuName = "digestQuest/Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public CardType cardType;
        [TextArea] public string description;
        public string stageRequirement;
        public string environmentRequirement;
        public int points;
    }
}