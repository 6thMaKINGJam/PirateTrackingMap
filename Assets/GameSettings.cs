using UnityEngine;

public static class GameSettings
{
    public const int CARD_TYPE_COUNT = 4;
    private static int[] cardCounts = new int[CARD_TYPE_COUNT];

    public static void SetInitialCardCounts(int[] counts)
    {
        if (counts == null || counts.Length != CARD_TYPE_COUNT)
        {
            Debug.LogError("Invalid card count array");
            return;
        }

        for (int i = 0; i < CARD_TYPE_COUNT; i++)
            cardCounts[i] = counts[i];
    }

    public static int GetCardCount(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= CARD_TYPE_COUNT)
            return 0;

        return cardCounts[cardIndex];
    }

    public static int[] GetAllCardCounts()
    {
        int[] copy = new int[CARD_TYPE_COUNT];
        cardCounts.CopyTo(copy, 0);
        return copy;
    }
    public static int GetTotalCardCount()
    {
        int sum = 0;
        for (int i = 0; i < CARD_TYPE_COUNT; i++)
            sum += cardCounts[i];
        return sum;
    }

    public static bool UseCard(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= CARD_TYPE_COUNT)
            return false;

        if (cardCounts[cardIndex] <= 0)
            return false;

        cardCounts[cardIndex]--;
        return true;
    }

    public static bool HasAnyCard()
    {
        return GetTotalCardCount() > 0;
    }

    public static void Reset()
    {
        for (int i = 0; i < CARD_TYPE_COUNT; i++)
            cardCounts[i] = 0;
    }
}
