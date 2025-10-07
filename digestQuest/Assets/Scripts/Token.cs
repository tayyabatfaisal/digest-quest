using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DigestQuest
{

    public enum TokenType
    {
        Carb,
        Fat,
        Protein
        // Add more types as needed (e.g., Fibre, Water)
    }
    public class Token
    {
        public TokenType tokenType;
        public bool digested = false;
        public bool absorbed = false;

        public Token(TokenType type)
        {
            tokenType = type;
        }

        public int GetPoints()
        {
            if (absorbed) return 5;   // Example scoring
            if (digested) return 3;
            return 0;
        }
    }
}