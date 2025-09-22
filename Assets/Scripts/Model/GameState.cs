using System.Collections.Generic;

public class GameState
{
    private List<Card> _deck;
    private List<Card> _playerA;
    private List<Card> _playerB;
    private List<Card> _discardPile;
    private Suit _trump;
    private PlayerType _turn;
    private (Card, Card)[] _slots;
}