using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnPhase
{
    idle,
    pre,
    waiting,
    post,
    gameOver
}

public class LostCities : MonoBehaviour
{
    static public LostCities S;
    static public Player CURRENT_PLAYER;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegrees = 10f;
    public int numStartingCards = 8;
    public float drawTimeStagger = 0.1f;

    [Header("Set Dynamically")]
    public Deck deck;
    public List<CardLostCities> drawPile;
    //public List<CardLostCities> discardPile;

    public List<CardLostCities> redDiscardPile;
    public List<CardLostCities> greenDiscardPile;
    public List<CardLostCities> whiteDiscardPile;
    public List<CardLostCities> blueDiscardPile;
    public List<CardLostCities> yellowDiscardPile;

    public List<CardLostCities> redPlayer1;
    public List<CardLostCities> redPlayer2;
    public List<CardLostCities> greenPlayer1;
    public List<CardLostCities> greenPlayer2;
    public List<CardLostCities> whitePlayer1;
    public List<CardLostCities> whitePlayer2;
    public List<CardLostCities> bluePlayer1;
    public List<CardLostCities> bluePlayer2;
    public List<CardLostCities> yellowPlayer1;
    public List<CardLostCities> yellowPlayer2;

    public List<Player> players;
    public CardLostCities targetCard;
    public CardLostCities targetKeepCard;
    public CardLostCities lastPlayedCard;


    public TurnPhase phase = TurnPhase.idle;
    public int firstCard = 0;

    public bool popUp = false;
    public bool popUpGameOver = false;
    public bool playerTurn = true;
    public bool played = false;
    public bool glow = false;
    public bool winner;
    public bool draw = false;
    public int p1Score;
    public int p2Score;
    public int finished = 0;

    private BartokLayout layout;
    private Transform layoutAnchor;


    private void Awake()
    {
        S = this;
    }

    private void Start()
    {


        deck = GetComponent<Deck>(); // Get the Deck
        deck.InitDeck(deckXML.text); // Pass DeckXML to it
        Deck.Shuffle(ref deck.cards); // This shuffles the deck

        layout = GetComponent<BartokLayout>(); // Get the Layout
        layout.ReadLayout(layoutXML.text); // Pass LayoutXML to it

        drawPile = UpgradeCardsList(deck.cards);
        LayoutGame();
        FixFace();
    }

    public void FixFace()
    {
        for (int i = 0; i < players[1].hand.Count; i++)
        {
            players[1].hand[i].faceUp = false;
        }
        for (int i = 0; i < players[0].hand.Count; i++)
        {
            players[0].hand[i].state = CBState.toHand;
        }
    }

    private void Update()
    {
        if (!playerTurn && finished == 1)
        {
            FixFace();
            AITurn();
            
        }

        FixFace();


    }

    public void AITurn()
    {
        finished = 0;
        Invoke("playAI", 2);
        //playAI();
        Invoke("aIDoDraw", 3);


    }

    List<CardLostCities> UpgradeCardsList(List<Card> lCD)
    {
        List<CardLostCities> lCB = new List<CardLostCities>();
        foreach (Card tCD in lCD)
        {
            lCB.Add(tCD as CardLostCities);
        }
        return (lCB);
    }

    // Position all the cards in the drawPile properly
    public void ArrangeDrawPile()
    {
        CardLostCities tCB;

        for (int i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.drawPile.pos;
            // Rotation should start at 0
            Vector2 dpStagger = layout.drawPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
                -layout.drawPile.layerID + 0.1f * i);
            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i * 4); // Order them front-to-back
            tCB.state = CBState.drawpile;
        }
    }

    public void ArrangePlayerPile()
    {
        Vector3 pos;
        CardLostCities tCB;
        for (int i = 0; i < redPlayer1.Count; i++)
        {
            tCB = redPlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.redPlayer1.pos;

            Vector2 dpStagger = layout.redPlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.redPlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.redPlayer1.y + i * dpStagger.y),
                -layout.redPlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.redPlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toRedPlayer1;
        }
        for (int i = 0; i < greenPlayer1.Count; i++)
        {
            tCB = greenPlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.greenPlayer1.pos;

            Vector2 dpStagger = layout.greenPlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.greenPlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.greenPlayer1.y + i * dpStagger.y),
                -layout.greenPlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.greenPlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toGreenPlayer1;
        }
        for (int i = 0; i < whitePlayer1.Count; i++)
        {
            tCB = whitePlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.whitePlayer1.pos;

            Vector2 dpStagger = layout.whitePlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.whitePlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.whitePlayer1.y + i * dpStagger.y),
                -layout.whitePlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.whitePlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toWhitePlayer1;
        }
        for (int i = 0; i < bluePlayer1.Count; i++)
        {
            tCB = bluePlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.bluePlayer1.pos;

            Vector2 dpStagger = layout.bluePlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.bluePlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.bluePlayer1.y + i * dpStagger.y),
                -layout.bluePlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.bluePlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toBluePlayer1;
        }
        for (int i = 0; i < yellowPlayer1.Count; i++)
        {
            tCB = yellowPlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.yellowPlayer1.pos;

            Vector2 dpStagger = layout.yellowPlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.yellowPlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.yellowPlayer1.y + i * dpStagger.y),
                -layout.yellowPlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.yellowPlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toYellowPlayer1;
        }
    }

    public void ArrangePlayer2Pile()
    {
        CardLostCities tCB;
        for (int i = 0; i < redPlayer2.Count; i++)
        {

            tCB = redPlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.redPlayer2.pos;

            Vector2 dpStagger = layout.redPlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.redPlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.redPlayer2.y + i * dpStagger.y),
                -layout.redPlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.redPlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.toRedPlayer2;
        }
        for (int i = 0; i < greenPlayer2.Count; i++)
        {
            tCB = greenPlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.greenPlayer2.pos;

            Vector2 dpStagger = layout.greenPlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.greenPlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.greenPlayer2.y + i * dpStagger.y),
                -layout.greenPlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.greenPlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.toGreenPlayer2;
        }
        for (int i = 0; i < whitePlayer2.Count; i++)
        {
            tCB = whitePlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.whitePlayer2.pos;

            Vector2 dpStagger = layout.whitePlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.whitePlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.whitePlayer2.y + i * dpStagger.y),
                -layout.whitePlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.whitePlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.toWhitePlayer2;
        }
        for (int i = 0; i < bluePlayer2.Count; i++)
        {
            tCB = bluePlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.bluePlayer2.pos;

            Vector2 dpStagger = layout.bluePlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.bluePlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.bluePlayer2.y + i * dpStagger.y),
                -layout.bluePlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.bluePlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.toBluePlayer2;
        }
        for (int i = 0; i < yellowPlayer2.Count; i++)
        {
            tCB = yellowPlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.yellowPlayer2.pos;

            Vector2 dpStagger = layout.yellowPlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.yellowPlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.yellowPlayer2.y + i * dpStagger.y),
                -layout.yellowPlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.yellowPlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.toYellowPlayer2;
        }
    }

    public void ArrangeDiscardPiles()
    {
        CardLostCities tCB;
        for (int i = 0; i < redDiscardPile.Count; i++)
        {

            tCB = redDiscardPile[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.redPlayer2.pos;

            Vector2 dpStagger = layout.redDiscardPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.redDiscardPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.redDiscardPile.y + i * dpStagger.y),
                -layout.redDiscardPile.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.redDiscardPile.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toRedDiscard;
        }
        for (int i = 0; i < greenDiscardPile.Count; i++)
        {
            tCB = greenDiscardPile[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.greenPlayer2.pos;

            Vector2 dpStagger = layout.greenDiscardPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.greenDiscardPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.greenDiscardPile.y + i * dpStagger.y),
                -layout.greenDiscardPile.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.greenDiscardPile.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toGreenDiscard;
        }
        for (int i = 0; i < whiteDiscardPile.Count; i++)
        {
            tCB = whiteDiscardPile[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.whitePlayer2.pos;

            Vector2 dpStagger = layout.whiteDiscardPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.whiteDiscardPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.whiteDiscardPile.y + i * dpStagger.y),
                -layout.whiteDiscardPile.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.whiteDiscardPile.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toWhiteDiscard;
        }
        for (int i = 0; i < blueDiscardPile.Count; i++)
        {
            tCB = blueDiscardPile[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.bluePlayer2.pos;

            Vector2 dpStagger = layout.blueDiscardPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.blueDiscardPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.blueDiscardPile.y + i * dpStagger.y),
                -layout.blueDiscardPile.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.blueDiscardPile.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toBlueDiscard;
        }
        for (int i = 0; i < yellowDiscardPile.Count; i++)
        {
            tCB = yellowDiscardPile[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.yellowPlayer2.pos;

            Vector2 dpStagger = layout.yellowDiscardPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.yellowDiscardPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.yellowDiscardPile.y + i * dpStagger.y),
                -layout.yellowDiscardPile.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.yellowDiscardPile.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.toYellowDiscard;
        }
    }

    //Perform the initial game layout
    void LayoutGame()
    {
        // Create an empty GameObject to serve as the tableau's anchor
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        // Position the drawPile cards
        ArrangeDrawPile();

        // Set up the players
        Player pl;
        players = new List<Player>();
        foreach (SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;

        }
        players[0].type = PlayerType.human; // Make only the 0th player human
        players[1].type = PlayerType.ai; // Make only the 0th player human
        
        CardLostCities tCB;
        // Deal # cards to each player
        for (int i = 0; i < numStartingCards; i++)
        {
            for (int j = 0; j < 2; j++)
            {

                tCB = Draw(); // Draw a card

                // Stagger the draw time a bit.
                tCB.timeStart = Time.time + drawTimeStagger * (i * 4 + j);

                players[j].AddCard(tCB);


            }


        }

        //Invoke("DrawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
    }



    // This callback is used by the last card to be dealt at the beginning
    public void CBCallback(CardLostCities cb)
    {
        // You sometimes want to have reporting of method calls like this
        Utils.tr("LostCities:CBCallback()", cb.name);
        StartGame(); // Start the Game
    }

    public void StartGame()
    {
        // Pick the player to the left of the human to go first.
        //PassTurn(1);
    }

    public void PassTurn(int num = -1)
    {
        Utils.tr("LostCities:PassTurn()", "Current Player: " + players.IndexOf(CURRENT_PLAYER));
        int lastPlayerNum = 0;
        int ndx = players.IndexOf(CURRENT_PLAYER);
        lastPlayerNum = players.IndexOf(CURRENT_PLAYER);
        if (ndx == 0)
            num = 1;
        else if (ndx == 1)
            num = 0;
        if (CURRENT_PLAYER != null)
        {
            lastPlayerNum = CURRENT_PLAYER.playerNum;
            // Check for Game Over and need to reshuffle discards
            /*
            if (CheckGameOver())
            {
                return;
            }*/
        }
        CURRENT_PLAYER = players[num];
        phase = TurnPhase.pre;

        CURRENT_PLAYER.TakeTurn();

        // Report the turn passing
        Utils.tr("LostCities:PassTurn()", "Old: " + lastPlayerNum, "New: " + CURRENT_PLAYER.playerNum);
    }


    public bool CheckGameOver()
    {
        // See if we need to reshuffle the discard pile into the draw pile
        if (drawPile.Count == 0)
        {
            // The player that just played has won!
            phase = TurnPhase.gameOver;
            Invoke("RestartGame", 1);
            return (true);
        }


        return (false);
    }


    public void RestartGame()
    {
        CURRENT_PLAYER = null;
        SceneManager.LoadScene("__LostCities_Scene_0");
    }


    // ValidPlay verifies that the card chosen can be played on the discard pile
    public bool ValidPlay(CardLostCities cb)
    {

        if (playerTurn == true)
        {
            if (WhichPlayerDiscard(cb).Count == 0)
                return true;
            else if (cb.rank > WhichPlayerDiscard(cb)[WhichPlayerDiscard(cb).Count - 1].rank) return true;
            else if (cb.rank == 1 && WhichPlayerDiscard(cb)[WhichPlayerDiscard(cb).Count - 1].rank == 1) return true;
        }

        if (playerTurn == false)
        {
            if (WhichAIDiscard(cb).Count == 0)
                return true;
            else if (cb.rank > WhichAIDiscard(cb)[WhichAIDiscard(cb).Count - 1].rank) return true;
            else if (cb.rank == 1 && WhichAIDiscard(cb)[WhichAIDiscard(cb).Count - 1].rank == 1) return true;
        }

        // Otherwise, return false
        return (false);
    }

    public int DifferencePlay(CardLostCities cb)
    {
        if (WhichAIDiscard(cb).Count == 0)
        {
            return cb.rank;
        }
        else if (cb.rank > WhichAIDiscard(cb)[WhichAIDiscard(cb).Count - 1].rank)
        {
            return cb.rank - WhichAIDiscard(cb)[WhichAIDiscard(cb).Count - 1].rank;
        }
        else
        {
            return 0;
        }
    }

    // This makes a new card the target
    public CardLostCities MoveToTarget(CardLostCities tCB)
    {
        tCB.timeStart = 0;


        tCB.state = CBState.toTarget;
        tCB.faceUp = true;

        tCB.SetSortingLayerName("10");
        tCB.eventualSortLayer = layout.target.layerName;
        if (targetCard != null)
        {
            MoveToDiscard(targetCard);
        }

        targetCard = tCB;

        return tCB;
    }

    public void ChangeTarget(CardLostCities tCB)
    {
        targetKeepCard = tCB;
        Debug.Log(tCB.state);
    }
    public CardLostCities CurrentCard()
    {
        return targetKeepCard;
    }
    public void ArrangeDiscard(CardLostCities tCB)
    {

        if (tCB.suit.Equals("R"))
        {
            tCB.SetSortingLayerName(layout.redDiscardPile.layerName);
            tCB.SetSortOrder(redDiscardPile.Count * 4);
            tCB.transform.localPosition = layout.redDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenDiscardPile.layerName);
            tCB.SetSortOrder(greenDiscardPile.Count * 4);
            tCB.transform.localPosition = layout.greenDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whiteDiscardPile.layerName);
            tCB.SetSortOrder(whiteDiscardPile.Count * 4);
            tCB.transform.localPosition = layout.whiteDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.blueDiscardPile.layerName);
            tCB.SetSortOrder(blueDiscardPile.Count * 4);
            tCB.transform.localPosition = layout.blueDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowDiscardPile.layerName);
            tCB.SetSortOrder(yellowDiscardPile.Count * 4);
            tCB.transform.localPosition = layout.yellowDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

    }

    public CardLostCities MoveToDiscard(CardLostCities tCB)
    {
        WhichDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        if (tCB.suit.Equals("R"))
        {

            tCB.SetSortingLayerName(layout.redDiscardPile.layerName);
            tCB.SetSortOrder(redDiscardPile.Count * 4);
            //tCB.transform.localPosition = Vector3.back / 2;
            //tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.redDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);
            tCB.MoveTo(pos);

            Invoke("ArrangeDiscardPiles", 2);

            tCB.state = CBState.toRedDiscard;


        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenDiscardPile.layerName);
            tCB.SetSortOrder(greenDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.greenDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toGreenDiscard;
        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whiteDiscardPile.layerName);
            tCB.SetSortOrder(whiteDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.whiteDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toWhiteDiscard;
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.blueDiscardPile.layerName);
            tCB.SetSortOrder(blueDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.blueDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toBlueDiscard;
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowDiscardPile.layerName);
            tCB.SetSortOrder(yellowDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.yellowDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toYellowDiscard;
        }

        players[0].RemoveCard(tCB);
        lastPlayedCard = tCB;
        return tCB;
    }

    public CardLostCities MoveToDiscardForAI(CardLostCities tCB)
    {
        WhichDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        if (tCB.suit.Equals("R"))
        {

            tCB.SetSortingLayerName(layout.redDiscardPile.layerName);
            tCB.SetSortOrder(redDiscardPile.Count * 4);
            //tCB.transform.localPosition = layout.greenDiscardPile.pos + Vector3.back / 2;
            //tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.redDiscardPile.pos + Vector3.back / 2;
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);
            tCB.MoveTo(pos);
            tCB.faceUp = true;

            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toRedDiscard;
        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenDiscardPile.layerName);
            tCB.SetSortOrder(greenDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.greenDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            tCB.faceUp = true;
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toGreenDiscard;
        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whiteDiscardPile.layerName);
            tCB.SetSortOrder(whiteDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.whiteDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            tCB.faceUp = true;
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toWhiteDiscard;
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.blueDiscardPile.layerName);
            tCB.SetSortOrder(blueDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.blueDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            tCB.faceUp = true;
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toBlueDiscard;
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowDiscardPile.layerName);
            tCB.SetSortOrder(yellowDiscardPile.Count * 4);
            tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.yellowDiscardPile.pos + Vector3.back / 2;
            tCB.MoveTo(pos);
            tCB.faceUp = true;
            Invoke("ArrangeDiscardPiles", 2);
            tCB.state = CBState.toYellowDiscard;
        }

        players[1].RemoveCard(tCB);
        return tCB;
    }


    public CardLostCities MoveToPlayerDiscard(CardLostCities tCB)
    {
        WhichPlayerDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        Vector2 dpStagger = layout.redPlayer1.stagger;
        if (tCB.suit.Equals("R"))
        {
            tCB.SetSortingLayerName(layout.redPlayer1.layerName);
            tCB.SetSortOrder(redPlayer1.Count * 4);

            pos = layout.redPlayer1.pos;
            if (redPlayer1.Count == 1)
            {
                pos = layout.redPlayer1.pos;
                pos = new Vector3(pos.x, pos.y, -layout.redPlayer1.layerID + 0.1f * (redPlayer1.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.redPlayer1.y + (redPlayer1.Count - 1) * dpStagger.y), -layout.redPlayer1.layerID + 0.1f * (redPlayer1.Count));
            }
            tCB.state = CBState.toRedPlayer1;
            tCB.MoveTo(pos);




        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenPlayer1.layerName);
            tCB.SetSortOrder(greenPlayer1.Count * 4);

            pos = layout.greenPlayer1.pos;
            if (greenPlayer1.Count == 1)
            {
                pos = layout.greenPlayer1.pos;
                pos = new Vector3(pos.x, pos.y, -layout.greenPlayer1.layerID + 0.1f * (greenPlayer1.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.greenPlayer1.y + (greenPlayer1.Count - 1) * dpStagger.y), -layout.greenPlayer1.layerID + 0.1f * (greenPlayer1.Count));
            }
            tCB.state = CBState.toGreenPlayer1;
            tCB.MoveTo(pos);



        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whitePlayer1.layerName);
            tCB.SetSortOrder(whitePlayer1.Count * 4);

            pos = layout.whitePlayer1.pos;
            if (whitePlayer1.Count == 1)
            {
                pos = layout.whitePlayer1.pos;
                pos = new Vector3(pos.x, pos.y, -layout.whitePlayer1.layerID + 0.1f * (whitePlayer1.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.whitePlayer1.y + (whitePlayer1.Count - 1) * dpStagger.y), -layout.whitePlayer1.layerID + 0.1f * (whitePlayer1.Count));
            }
            tCB.state = CBState.toWhitePlayer1;
            tCB.MoveTo(pos);


        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.bluePlayer1.layerName);
            tCB.SetSortOrder(bluePlayer1.Count * 4);

            pos = layout.bluePlayer1.pos;
            if (bluePlayer1.Count == 1)
            {
                pos = layout.bluePlayer1.pos;
                pos = new Vector3(pos.x, pos.y, -layout.bluePlayer1.layerID + 0.1f * (bluePlayer1.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.bluePlayer1.y + (bluePlayer1.Count - 1) * dpStagger.y), -layout.bluePlayer1.layerID + 0.1f * (bluePlayer1.Count));
            }

            tCB.MoveTo(pos);

            tCB.state = CBState.toBluePlayer1;
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowPlayer1.layerName);
            tCB.SetSortOrder(yellowPlayer1.Count * 4);

            pos = layout.yellowPlayer1.pos;
            if (yellowPlayer1.Count == 1)
            {
                pos = layout.yellowPlayer1.pos;
                pos = new Vector3(pos.x, pos.y, -layout.yellowPlayer1.layerID + 0.1f * (yellowPlayer1.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.yellowPlayer1.y + (yellowPlayer1.Count - 1) * dpStagger.y), -layout.yellowPlayer1.layerID + 0.1f * (yellowPlayer1.Count));
            }
            tCB.state = CBState.toYellowPlayer1;
            tCB.MoveTo(pos);

        }



        players[0].RemoveCard(tCB);
        Invoke("ArrangePlayerPile", 2);
        return tCB;
    }

    public CardLostCities MoveToAIDiscard(CardLostCities tCB)
    {
        WhichAIDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        Vector2 dpStagger = layout.redPlayer2.stagger;

        Vector3 v = new Vector3(180, 0, 0);
        Quaternion rotQT = Quaternion.Euler(v);



        if (tCB.suit.Equals("R"))
        {
            tCB.SetSortingLayerName(layout.redPlayer2.layerName);
            tCB.SetSortOrder(redPlayer2.Count * 4);

            pos = layout.redPlayer2.pos;
            if (redPlayer2.Count == 1)
            {
                pos = layout.redPlayer2.pos;
                pos = new Vector3(pos.x, pos.y, -layout.redPlayer2.layerID + 0.1f * (redPlayer2.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.redPlayer2.y + (redPlayer2.Count - 1) * dpStagger.y), -layout.redPlayer2.layerID + 0.1f * (redPlayer2.Count));
            }

            tCB.MoveTo(pos, rotQT);

            tCB.state = CBState.toRedPlayer2;


        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenPlayer2.layerName);
            tCB.SetSortOrder(greenPlayer2.Count * 4);

            pos = layout.greenPlayer2.pos;
            if (greenPlayer2.Count == 1)
            {
                pos = layout.greenPlayer2.pos;
                pos = new Vector3(pos.x, pos.y, -layout.greenPlayer2.layerID + 0.1f * (greenPlayer2.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.greenPlayer2.y + (greenPlayer2.Count - 1) * dpStagger.y), -layout.greenPlayer2.layerID + 0.1f * (greenPlayer2.Count));
            }

            tCB.MoveTo(pos, rotQT);

            tCB.state = CBState.toGreenPlayer2;


        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whitePlayer2.layerName);
            tCB.SetSortOrder(whitePlayer2.Count * 4);

            pos = layout.whitePlayer2.pos;
            if (whitePlayer2.Count == 1)
            {
                pos = layout.whitePlayer2.pos;
                pos = new Vector3(pos.x, pos.y, -layout.whitePlayer2.layerID + 0.1f * (whitePlayer2.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.whitePlayer2.y + (whitePlayer2.Count - 1) * dpStagger.y), -layout.whitePlayer2.layerID + 0.1f * (whitePlayer2.Count));
            }

            tCB.MoveTo(pos, rotQT);

            tCB.state = CBState.toWhitePlayer2;
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.bluePlayer2.layerName);
            tCB.SetSortOrder(bluePlayer2.Count * 4);

            pos = layout.bluePlayer2.pos;
            if (bluePlayer2.Count == 1)
            {
                pos = layout.bluePlayer2.pos;
                pos = new Vector3(pos.x, pos.y, -layout.bluePlayer2.layerID + 0.1f * (bluePlayer2.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.bluePlayer2.y + (bluePlayer2.Count - 1) * dpStagger.y), -layout.bluePlayer2.layerID + 0.1f * (bluePlayer2.Count));
            }

            tCB.MoveTo(pos, rotQT);

            tCB.state = CBState.toBluePlayer2;
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowPlayer2.layerName);
            tCB.SetSortOrder(yellowPlayer2.Count * 4);

            pos = layout.yellowPlayer2.pos;
            if (bluePlayer2.Count == 1)
            {
                pos = layout.yellowPlayer2.pos;
                pos = new Vector3(pos.x, pos.y, -layout.yellowPlayer2.layerID + 0.1f * (yellowPlayer2.Count));
            }
            else
            {
                pos = new Vector3(pos.x, layout.multiplier.y * (layout.yellowPlayer2.y + (yellowPlayer2.Count - 1) * dpStagger.y), -layout.yellowPlayer2.layerID + 0.1f * (yellowPlayer2.Count));
            }

            tCB.MoveTo(pos, rotQT);

            tCB.state = CBState.toYellowPlayer2;
        }
        tCB.faceUp = true;
        players[1].RemoveCard(tCB);
        Invoke("ArrangePlayer2Pile", 2);

        return tCB;
    }

    public List<CardLostCities> WhichDiscard(CardLostCities tCB)
    {
        Debug.Log(tCB.suit);
        if (tCB.suit.Equals("R"))
        {
            return redDiscardPile;
        }
        else if (tCB.suit.Equals("G"))
        {
            return greenDiscardPile;
        }
        else if (tCB.suit.Equals("W"))
        {
            return whiteDiscardPile;
        }
        else if (tCB.suit.Equals("B"))
        {
            return blueDiscardPile;
        }
        else
        {
            return yellowDiscardPile;
        }
    }

    public List<CardLostCities> WhichPlayerDiscard(CardLostCities tCB)
    {
        Debug.Log(tCB.suit);


        {
            if (tCB.suit.Equals("R"))
            {
                return redPlayer1;
            }
            else if (tCB.suit.Equals("G"))
            {
                return greenPlayer1;
            }
            else if (tCB.suit.Equals("W"))
            {
                return whitePlayer1;
            }
            else if (tCB.suit.Equals("B"))
            {
                return bluePlayer1;
            }
            else
            {
                return yellowPlayer1;
            }
        }


    }
    public List<CardLostCities> WhichAIDiscard(CardLostCities tCB)
    {
        if (tCB.suit.Equals("R"))
        {
            return redPlayer2;
        }
        else if (tCB.suit.Equals("G"))
        {
            return greenPlayer2;
        }
        else if (tCB.suit.Equals("W"))
        {
            return whitePlayer2;
        }
        else if (tCB.suit.Equals("B"))
        {
            return bluePlayer2;
        }
        else
        {
            return yellowPlayer2;
        }
    }
    void OnGUI()
    {

        if (popUp && !played && playerTurn && !popUpGameOver)
        {
            if (GUI.Button(new Rect(10, 200, 100, 28), "Discard"))
            {

                if (playerTurn && !played && targetKeepCard.state == CBState.hand)
                {
                    MoveToDiscard(targetKeepCard);
                    played = true;
                    popUp = false;



                }
            }

            if (GUI.Button(new Rect(10, 300, 100, 28), "Play"))
            {

                if (ValidPlay(targetKeepCard) && playerTurn && !played && targetKeepCard.state == CBState.hand)
                {

                    MoveToPlayerDiscard(targetKeepCard);
                    popUp = false;
                    played = true;


                }


            }


        }

        if (popUpGameOver)
        {
            if (GUI.Button(new Rect(475, 750, 200, 48), "Play Again?"))
            {

                RestartGame();


            }
        }

    }

    public void playAI()
    {
        Utils.tr("AI Turn");
        players[1].TakeTurn();
        playerTurn = true;

    }

    // The Draw function will pull a single card from the drawPile and return it
    public CardLostCities Draw()
    {

        CardLostCities cd = drawPile[0]; // Pull the 0th CardLostCities





        drawPile.RemoveAt(0); // Then remove it from List<> drawPile
        ArrangeDrawPile();
        CheckGameOver();
        if (drawPile.Count == 0)
        {
            Invoke("GameOver", 3);
        }
        cd.state = CBState.toHand;
        return (cd); // And return it

    }


    public void aIDoDraw()
    {
        players[1].AddCard(Draw());
    }

    public CardLostCities RedDraw()
    {
        Debug.Log(redDiscardPile.Count - 1);
        int i = redDiscardPile.Count - 1;
        CardLostCities cd = redDiscardPile[i];
        redDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities GreenDraw()
    {
        int i = greenDiscardPile.Count - 1;
        CardLostCities cd = greenDiscardPile[i];
        greenDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities WhiteDraw()
    {
        int i = whiteDiscardPile.Count - 1;
        CardLostCities cd = whiteDiscardPile[i];
        whiteDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities BlueDraw()
    {
        int i = blueDiscardPile.Count - 1;
        CardLostCities cd = blueDiscardPile[i];
        blueDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities YellowDraw()
    {
        int i = yellowDiscardPile.Count - 1;
        CardLostCities cd = yellowDiscardPile[i];
        yellowDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }

    public void CardClicked(CardLostCities tCB)
    {

        ChangeTarget(tCB);


        switch (tCB.state)
        {
            case CBState.drawpile:


                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(Draw());
                    ArrangeDrawPile();
                    playerTurn = !playerTurn;
                    played = false;
                    finished = 1;
                }
                break;

            case CBState.hand:
                // Check to see whether the card is valid
                if (!played)
                {
                    popUp = true;
                }

                break;

            case CBState.redDiscard:
                if (players[0].hand.Count < 8 && tCB != lastPlayedCard)
                {
                    players[0].AddCard(RedDraw());
                    playerTurn = !playerTurn;
                    played = false;
                    finished = 1;
                }
                break;
            case CBState.greenDiscard:
                if (players[0].hand.Count < 8 && tCB != lastPlayedCard)
                {
                    players[0].AddCard(GreenDraw());
                    playerTurn = !playerTurn;
                    played = false;
                    finished = 1;
                }
                break;
            case CBState.whiteDiscard:
                if (players[0].hand.Count < 8 && tCB != lastPlayedCard)
                {
                    players[0].AddCard(WhiteDraw());
                    playerTurn = !playerTurn;
                    played = false;
                    finished = 1;
                }
                break;
            case CBState.blueDiscard:
                if (players[0].hand.Count < 8 && tCB != lastPlayedCard)
                {
                    players[0].AddCard(BlueDraw());
                    playerTurn = !playerTurn;
                    played = false;
                    finished = 1;
                }
                break;
            case CBState.yellowDiscard:
                if (players[0].hand.Count < 8 && tCB != lastPlayedCard)
                {
                    players[0].AddCard(YellowDraw());
                    playerTurn = !playerTurn;
                    played = false;
                    finished = 1;
                }
                break;
        }

    }

    public void GameOver()
    {
        int player1Score = 0;
        int player2Score = 0;
        int player1RMultiplier = 1;
        int player1GMultiplier = 1;
        int player1WMultiplier = 1;
        int player1BMultiplier = 1;
        int player1YMultiplier = 1;
        int player2RMultiplier = 1;
        int player2GMultiplier = 1;
        int player2WMultiplier = 1;
        int player2BMultiplier = 1;
        int player2YMultiplier = 1;
        int p1R = 20;
        int p1G = 20;
        int p1W = 20;
        int p1B = 20;
        int p1Y = 20;
        int p2R = 20;
        int p2G = 20;
        int p2W = 20;
        int p2B = 20; 
        int p2Y = 20;


        if (redPlayer1.Count != 0)
        {
            player1RMultiplier = Score.S.Multiplier(redPlayer1);
            if (player1RMultiplier == 1) player1Score = player1Score + Score.S.ComputeScore(redPlayer1);
            else player1Score = player1Score + (player1RMultiplier * Score.S.ComputeScore(redPlayer1));
            p1R = p1R * player1RMultiplier;
            player1Score = player1Score - p1R;
        }
        if (greenPlayer1.Count != 0)
        {
            player1GMultiplier = Score.S.Multiplier(greenPlayer1);
            if (player1GMultiplier == 1) player1Score = player1Score + Score.S.ComputeScore(greenPlayer1);
            else player1Score = player1Score + (player1GMultiplier * Score.S.ComputeScore(greenPlayer1));
            p1G = p1G * player1GMultiplier;
            player1Score = player1Score - p1G;
        }
        if (whitePlayer1.Count != 0)
        {
            player1WMultiplier = Score.S.Multiplier(whitePlayer1);
            if (player1WMultiplier == 1) player1Score = player1Score + Score.S.ComputeScore(whitePlayer1);
            else player1Score = player1Score + (player1WMultiplier * Score.S.ComputeScore(whitePlayer1));
            p1W = p1W * player1WMultiplier;
            player1Score = player1Score - p1W;
        }
        if (bluePlayer1.Count != 0)
        {
            player1BMultiplier = Score.S.Multiplier(bluePlayer1);
            if (player1BMultiplier == 1) player1Score = player1Score + Score.S.ComputeScore(bluePlayer1);
            else player1Score = player1Score + (player1BMultiplier * Score.S.ComputeScore(bluePlayer1));
            p1B = p1B * player1BMultiplier;
            player1Score = player1Score - p1B;
        }
        if (yellowPlayer1.Count != 0)
        {
            player1YMultiplier = Score.S.Multiplier(yellowPlayer1);
            if (player1YMultiplier == 1) player1Score = player1Score + Score.S.ComputeScore(yellowPlayer1);
            else player1Score = player1Score + (player1YMultiplier * Score.S.ComputeScore(yellowPlayer1));
            p1Y = p1Y * player1YMultiplier;
            player1Score = player1Score - p1Y;
        }

        if (redPlayer2.Count != 0)
        {
            player2RMultiplier = Score.S.Multiplier(redPlayer2);
            if (player2RMultiplier == 1) player2Score = player2Score + Score.S.ComputeScore(redPlayer2);
            else player2Score = player2Score + (player2RMultiplier * Score.S.ComputeScore(redPlayer2));
            p2R = p2R * player2RMultiplier;
            player2Score = player2Score - p2R;
        }
        if (greenPlayer2.Count != 0)
        {
            player2GMultiplier = Score.S.Multiplier(greenPlayer2);
            if (player2GMultiplier == 1) player2Score = player2Score + Score.S.ComputeScore(greenPlayer2);
            else player2Score = player2Score + (player2GMultiplier * Score.S.ComputeScore(greenPlayer2));
            p2G = p2G * player2GMultiplier;
            player2Score = player2Score - p2G;
        }
        if (whitePlayer2.Count != 0)
        {
            player2WMultiplier = Score.S.Multiplier(whitePlayer2);
            if (player2WMultiplier == 1) player2Score = player2Score + Score.S.ComputeScore(whitePlayer2);
            else player2Score = player2Score + (player2WMultiplier * Score.S.ComputeScore(whitePlayer2));
            p2W = p2W * player2WMultiplier;
            player2Score = player2Score - p2W;
        }
        if (bluePlayer2.Count != 0)
        {
            player2BMultiplier = Score.S.Multiplier(bluePlayer2);
            if (player2BMultiplier == 1) player2Score = player2Score + Score.S.ComputeScore(bluePlayer2);
            else player2Score = player2Score + (player2BMultiplier * Score.S.ComputeScore(bluePlayer2));
            p2B = p2B * player2BMultiplier;
            player2Score = player2Score - p2B;
        }
        if (yellowPlayer2.Count != 0)
        {
            player2YMultiplier = Score.S.Multiplier(yellowPlayer2);
            if (player2YMultiplier == 1) player2Score = player2Score + Score.S.ComputeScore(yellowPlayer2);
            else player2Score = player2Score + (player2YMultiplier * Score.S.ComputeScore(yellowPlayer2));
            p2Y = p2Y * player2YMultiplier;
            player2Score = player2Score - p2Y;
        }

        p1Score = player1Score;
        p2Score = player2Score;

        if (drawPile.Count == 40)
        {
            if (player1Score > player2Score)
            {
                winner = true;
                //ScoreManager.S.state = eScoreEvent.gameWin;
            }
            else if (player2Score > player1Score)
            {
                winner = false;
                //ScoreManager.S.state = eScoreEvent.gameLoss;
            }
            else if (player1Score == player2Score)
            {
                draw = true;
                //ScoreManager.S.state = eScoreEvent.draw;
            }
            //ScoreManager.S.score = player1Score;
            //ScoreManager.S.Event(ScoreManager.S.state);
            //GetComponent<Text>().text = player1Score;
            phase = TurnPhase.gameOver;
            //Invoke("RestartGame", 10);
            popUpGameOver = true;

        }
    }

}
