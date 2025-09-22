using System;

public readonly struct Card : IEquatable<Card>
{
    public static readonly Card None = new(Suit.None, Rank.None);

    public readonly Suit Suit;
    public readonly Rank Rank;    

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    bool IEquatable<Card>.Equals(Card other)
    {
        return Suit == other.Suit && Rank == other.Rank;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Suit, Rank);
    }
}