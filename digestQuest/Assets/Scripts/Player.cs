using UnityEngine;
using System.Collections.Generic;
using TMPro; // If you want UI links

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public int score = 0;
    // public List<TokenType> tokens = new List<TokenType>(); // Or Token objects, if you prefer

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score updated! Current score: " + score);
        // Optionally update score UI here
    }

    // public void AddToken(TokenType type)
    // {
    //     tokens.Add(type);
    // }

    // public void RemoveToken(TokenType type)
    // {
    //     tokens.Remove(type);
    // }

    // Add digest/absorb logic here later
}