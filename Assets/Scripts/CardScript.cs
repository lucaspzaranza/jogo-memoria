using UnityEngine;
using System.Collections;

public enum CardStates
{
    Back,    
    Flipped
}

public class CardScript : MonoBehaviour 
{
    public GameController gameController;

    private CardStates currentCardState;

    public Texture backTexture;
    public Texture flippedTexture;

    private float timeFlipping;

    public bool isFlipping = false;

    public int cardNumber;

    private int cardFlippedNumber;

    private int numCall = 0;

    void Awake()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController; 
    } 

	void Start ()
    {
        currentCardState = CardStates.Back;

        gameObject.GetComponent<Renderer>().material.mainTexture = backTexture;
	}

    void Update()
    {
        if (isFlipping)
        {
            FlipCard();
            timeFlipping += Time.deltaTime;
        }

        //if (timeFlipping >= 0.5f)
        if (numCall > 0 && (GetCurrentCardState() == CardStates.Back && transform.eulerAngles.y < 90f ||
            GetCurrentCardState() == CardStates.Flipped && transform.eulerAngles.y > 180f))
        {
            print($"STOP! {transform.eulerAngles.y}");
            isFlipping = false;
            timeFlipping = 0;
            numCall = 0;           
        }
        //else if (timeFlipping >= 0.2875f && timeFlipping < 0.35f && isFlipping && numCall == 0)
        else if (isFlipping && numCall == 0)
        {
            if (GetCurrentCardState() == CardStates.Back && transform.eulerAngles.y > 100f)
            {
                numCall++;
                //print($"flippedTexture: {transform.eulerAngles.y}");
                GetComponent<Renderer>().material.mainTexture = flippedTexture;                

                currentCardState = CardStates.Flipped;
                
                tag = "flipped card";
            }
            else 
            if(transform.eulerAngles.y > 270f && transform.eulerAngles.y < 290f)
            {
                //print($"backTexture: {transform.eulerAngles.y}");
                GetComponent<Renderer>().material.mainTexture = backTexture;

                currentCardState = CardStates.Back;
                tag = "card";
                numCall++;
            }
        }
    }

    public void SetCardName(int number)
    {
        name += " " + number.ToString();
    }

    void OnMouseDown()
    {
        bool logic = !isFlipping && gameController.numCardsFlipped < 2;
        logic &= GetCurrentCardState() == CardStates.Back && gameController.GetCurrentState() == GameStates.InGame;
        logic &= gameController.timeToEndMatch != 0f;

        if (logic)
        {
            gameController.cardsFlipped[gameController.numCardsFlipped] = GetComponent<CardScript>();
            transform.rotation = Quaternion.identity;
            gameController.numCardsFlipped++;
            isFlipping = true;
        }
    }

    private void FlipCard()
    {        
        transform.Rotate(Vector3.up, 350f * Time.deltaTime);        
    }   

    public CardStates GetCurrentCardState()
    {
        return currentCardState;
    }

    public void SetCardState(CardStates cardState)
    {
        currentCardState = cardState;
    }   
}