using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UnityEngine.UI;
using VirtualKeyboardUI;
 
public enum GameStates
{
    SignIn,
    InGame,
    End,
    Reseting,
    Ranking,
    Settings,
    Expired
}

public class GameController : MonoBehaviour 
{
    #region variables

    #region Global Variables
    public GameObject [] cards;
    public GameObject background;
    public VirtualKeyboard virtualKeyboard;
   
    private GameStates currentState;

    private CardScript[] cardScript;

    public GameObject easyCards;
    public GameObject mediumCards;
    public GameObject hardCards;
    public GameObject cardsParent;

    public Texture2D cardBackTexture;
    public Texture [] cardTextures = new Texture[6];   

    public Font myFont;
    public GUIStyle myStyle;
    public Rect scoreLabelCoordinates;
    public int fontSize = 50;

    public Rect timeCoordinates;
    public float timeToEndMatch;
    private float timeToDecrementOneSecond;
    private float timeToCallEndGame;

    public int numCardsFlipped;
    public int totalNumCardsFlipped;

    int[] numbersChosen;

    public CardScript[] cardsFlipped = new CardScript[2];
    public GameObject[] totalCardsFlipped;

    public float timeToUnflip;

    public int score;
    public int numOfPairs;
    private int partialScore;
    public int totalScore;
    public int numberOfMisses;
    public int numberOfMatches;

    public string playerName;
    public string playerEmail;
    public string playerPhone; 

    public bool isShowingCards;

    private bool endGameCalled;

    private bool isCountDown;
    private int countDown = 5;

    public int numberOftries;

    private FileStream file;
    private string fileDataPath;
    private string customFolderDataPath;

    public int firstUseDay;
    public int currentDay;
    public int expireDay;
    private bool showExpiredMessage;

    public GameObject directionalLight;
    public GameObject prizeStar;

    private bool callRankingMenu;
    public Texture backTexture;
   
    private bool resetButtonClicked;
    private bool leaveButtonClicked;

    public bool isTrial;

    private bool inputName;
    private bool inputEmail;
    private bool inputPhone;

    public int maxRankingPositions = 20;

    private Vector2 rankingScrollView = Vector2.zero;

    private int difficulty;
    private string [] difficulties = new string[] {"Fácil", "Médio", "Difícil"};

    private int numOfResets;

    public RankingController rankingController;

    #endregion

    #region  UI VARIABLES

    public InputField nameInputField;
    public InputField emailInputField;
    public InputField phoneInputField;
    public Button RankingButton;
    public Button StartGameButton;
    public Button SettingsButton;
    public Button BackButton;
    public Text Title;
    public GameObject ResetConfirmation;
    public GameObject QuitConfirmation;
    public Button ResetButton;
    public Button QuitButton;
    public GameObject MainMenuUI;
    public GameObject SettingsUI;
    public GameObject InGameUI;
    public Text PointsText;
    public Text NumOfTriesText;
    public Text TimeLeftText;
    public Button ResetGameButton;
    public GameObject EndGameUI;
    public Text PairsPointText;
    public Text TimePointText;
    public Text MissesPointText;
    public Text TotalScoreText;
    public GameObject RankingUI;
    public Text ErrorMessage;
    public Button UpdateGraphicsButton;
    public GameObject NewBackgroundImg;
    public GameObject smileFace;
    public GameObject defaultEndingTitle;
    public GameObject bestEndingTitle;

    #endregion

    #endregion   

    // Use this for initialization
    void Start () 
    {
        customFolderDataPath = Application.dataPath + "/Custom";        

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            fileDataPath = Application.dataPath + "//Cadastros.txt";
            
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            fileDataPath = "/storage/emulated/0/Unity";
            fileDataPath += "/Cadastros.txt";
        }

        if (!string.IsNullOrEmpty(fileDataPath) && !File.Exists(fileDataPath))
        {            
            file = File.Create(fileDataPath);
            file.Close();
        }

        if (!string.IsNullOrEmpty(customFolderDataPath) && !Directory.Exists(customFolderDataPath))
        {
            Directory.CreateDirectory(customFolderDataPath);
        }

        ResetPlayerData();
        CallSignInMenu();
    }

	// Update is called once per frame
	void Update ()
    {       
        switch (currentState)
        {
            case GameStates.SignIn:
                    SetCardsParent();
                    break;

            #region InGame State

            case GameStates.InGame:
               
                if (timeToEndMatch <= 0 || numOfPairs == cards.Length/2)
                {
                    if (!endGameCalled)
                    {
                        totalScore = GetTotalScore();
                        StartCoroutine(CallEndGame());
                        endGameCalled = true;
                    }                                       
                }
                else if (timeToEndMatch > 0)
                {
                    GetSeconds();
                    TimeLeftText.text = TimeToString(timeToEndMatch);
                }
                
                if (numCardsFlipped == 2)
                {                    
                    if (IsEqual(cardsFlipped[0], cardsFlipped[1]))
                    {
                        AddScore();
                        PointsText.text = score.ToString();
                        numberOftries++;
                        NumOfTriesText.text = numberOftries.ToString();
                        numCardsFlipped = 0;
                        totalNumCardsFlipped += 2;
                    }
                    else
                    {
                        if (timeToUnflip > 1f)
                        {
                            UnflipCards(cardsFlipped[0], cardsFlipped[1]);                            
                            numberOfMisses = 1 + numberOfMisses;
                            numberOftries++;
                            NumOfTriesText.text = numberOftries.ToString();
                            timeToUnflip = 0;
                        }
                        else
                        {
                            timeToUnflip += Time.deltaTime;
                        }                                        
                    }
                }
                break;

            #endregion

            case GameStates.End:                
                break;
        }
	}

    public void CallUpdateGameGraphics()
    { 
        StartCoroutine("UpdateGameGraphics");
    }

    private IEnumerator UpdateGameGraphics()
    {
        yield return null;
        // WWW backgroundLink = null;
        // WWW[] cardsLink = new WWW[NumberOfCards()];
        // WWW backTextureLink = null;
        // Texture2D backgroundTexture = null;        
                   
        // backgroundLink = new WWW("file:///" + customFolderDataPath + "/background.png");
        // yield return backgroundLink;

        // backTextureLink = new WWW("file:///" + customFolderDataPath + "/back.png");
        // yield return backTextureLink;

        // for (int i = 1; i <= NumberOfCards(); i++)
        // {
        //     string url = string.Format("file:///{0}/carta {1}.png", customFolderDataPath, i);
        //     cardsLink[i - 1] = new WWW(url);
        //     yield return cardsLink[i - 1];
        // }            
           
        // if(backgroundLink.error == null)
        // {
        //     backgroundTexture = backgroundLink.texture;
        //     background.GetComponent<SpriteRenderer>().sprite = Sprite.Create(backgroundTexture,
        //     new Rect(0, 0, backgroundTexture.width, backgroundTexture.height),
        //     new Vector2(0.5f, 0.5f));
        //     ErrorMessage.text = "";
        // }      
        // else
        // {
        //     ErrorMessage.text = "Não foi possível carregar o plano de fundo.";
        // }              
        
        // foreach(WWW loadedCard in cardsLink)
        // {
        //     if(loadedCard.error != null)
        //     {
        //         if (ErrorMessage.text != "") ErrorMessage.text += "\n\n";
                    
        //         ErrorMessage.text += "Algumas cartas não foram carregadas corretamente.";                                     
        //         break;
        //     }
        // }

        // if(backTextureLink.error == null)
        // {            
        //     cardBackTexture = backTextureLink.texture;
        //     cardsParent.SetActive(true);
        //     cardScript = cardsParent.GetComponentsInChildren<CardScript>();
        //     cardsParent.SetActive(false);

        //     foreach(CardScript card in cardScript)
        //     {                
        //         card.backTexture = cardBackTexture; 
        //     }            
        // }
        // else
        // {
        //     if (ErrorMessage.text != "") ErrorMessage.text += "\n\n";
                
        //     ErrorMessage.text += "Não foi possível carregar o verso da carta.";
        // }

        // if (!string.IsNullOrEmpty(ErrorMessage.text))
        //     ErrorMessage.text += "\n\n Verifique se os arquivos estão na pasta correta e nomeados corretamente.";

        // for (int i = 0; i < cardsLink.Length; i++)
		// {
		// 	if(cardsLink[i].error == null)
        //     {
        //         cardTextures[i] = cardsLink[i].texture;
        //     }
		// }       
    }

    private void UpdateCardErrorMessage()
    {
        if (ErrorMessage.text != "")
            ErrorMessage.text += "\n\n";

        ErrorMessage.text += "Não foi possível carregar todas as cartas. \n" +
                             "O jogo pode não funcionar corretamente.";
    }

    private int NumberOfCards()
    {
        switch (difficulty)
        {
            default:
                return 8; 

            case 1:
                return 9;

            case 2:
                return 12;
        }
    }      

    private void WritePlayerData()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (playerName != "")
            {
                File.AppendAllText(fileDataPath, playerName + ", ", Encoding.UTF8);
            }

            if (playerEmail != "")
            {
                File.AppendAllText(fileDataPath, playerEmail + ", ", Encoding.UTF8);
            }

            if (playerPhone != "")
            {
                File.AppendAllText(fileDataPath, playerPhone + ", ", Encoding.UTF8);
            }

            File.AppendAllText(fileDataPath, 60 - timeToEndMatch + " segundos, ", Encoding.UTF8);

            File.AppendAllText(fileDataPath, numberOfMisses + " erros, ", Encoding.UTF8);

            File.AppendAllText(fileDataPath, GetTotalScore().ToString() + " pontos" + Environment.NewLine + Environment.NewLine, Encoding.UTF8);        
        }        
    }

    private void ResetPlayerData()
    {
        playerName = "";
        playerEmail = "";
        playerPhone = "";
    }

    private void ToggleCardsRenderer()
    {        
        foreach (GameObject card in cards)
        {
            card.GetComponent<Renderer>().enabled = !card.GetComponent<Renderer>().enabled;
            if (card.gameObject.tag != "card")
            {
                card.gameObject.tag = "card";
                card.transform.rotation = Quaternion.identity;
                card.GetComponent<CardScript>().SetCardState(CardStates.Back);
                card.GetComponent<CardScript>().gameObject.GetComponent<Renderer>().material.mainTexture = cardScript[0].backTexture;
            }
        }
    }

    private IEnumerator CallEndGame()
    {       
        if (!Application.isEditor)
        {
            WritePlayerData();
        }

        ResetGameButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        FlipAllCards();

        if (rankingController.IsTop20(GetTotalScore()) && playerName != "")
        {
            rankingController.RefreshRanking(playerName, GetTotalScore(), rankingController.GetTop20Position(GetTotalScore()));
        }

        yield return new WaitForSeconds(1.5f);
        ToggleCardsRenderer();               
        ResetGameButton.gameObject.SetActive(true);        
        DeactivateInGameUI();
        ActivateEndGameUI();            
        currentState = GameStates.End;        
        endGameCalled = false;
    }
   
    private void GetSeconds()
    {
        if (timeToDecrementOneSecond >= 1f)
        {
            timeToEndMatch--;
            timeToDecrementOneSecond = 0;
        }
        else
        {
            timeToDecrementOneSecond += Time.deltaTime;
        }
    }   

    private void AddScore()
    {
        score += 100;
        numOfPairs = 1 + numOfPairs;
    }

    private bool IsEqual(CardScript cardOne, CardScript cardTwo)
    {
        if (cardOne != null && cardTwo != null)
        {
            return cardOne.cardNumber == cardTwo.cardNumber;
        }
        return false;
    }

    private void GetCards()
    {           
        if(cardScript == null)
            cardScript = cardsParent.GetComponentsInChildren<CardScript>();

        cards = new GameObject[cardScript.Length];
       
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = cardScript[i].gameObject;
            cardScript[i].SetCardName(i);
            cardScript[i].cardNumber = 13; // padrão            
        }
    }

    private bool numberAlreadyChoosen(int numberToCheck)
    {
        if(numberToCheck == cards.Length) numberToCheck--;
        return numberToCheck == numbersChosen[numberToCheck];
    }

    private void ShuffleCards()
    {
        numbersChosen = new int[cards.Length];
        Transform cardToChangeTransform = transform;
        int randomNumber;              
              
        for (int i = 0; i < cards.Length; i++)
        {
            numbersChosen[i] = cards.Length;
        }

        for (int i = 0; i < cards.Length/2; i++)
        {
            numbersChosen[i] = i;

            randomNumber = UnityEngine.Random.Range(0, cards.Length);    

            while (numberAlreadyChoosen(randomNumber))
            {
                randomNumber = UnityEngine.Random.Range(0, cards.Length);    
            }

            numbersChosen[randomNumber] = randomNumber;

            // Trocando as posições
            cardToChangeTransform.transform.position = new Vector3(cards[i].transform.position.x, cards[i].transform.position.y, cards[i].transform.position.z);

            cards[i].transform.position = new Vector3(cards[randomNumber].transform.position.x, cards[randomNumber].transform.position.y, cards[randomNumber].transform.position.z);

            cards[randomNumber].transform.position = new Vector3(cardToChangeTransform.transform.position.x, cardToChangeTransform.transform.position.y, cardToChangeTransform.transform.position.z);
        }
    }

    private void SetCardTextures()
    {
        int numCards = NumberOfCards();
        int randomNumber = UnityEngine.Random.Range(0, numCards);        
        
        for (int i = 0; i < cards.Length; i++)
        {
            while (ExistPair(randomNumber))
            {
                randomNumber = UnityEngine.Random.Range(0, NumberOfCards());                
            }

            cardScript[i].cardNumber = randomNumber;
            cardScript[i].flippedTexture = cardTextures[randomNumber];           
        }
    }

    private bool ExistPair(int numberToCheck)
    { 
        int cardNumberFound = 0;

        for (int i = 0; i < cardScript.Length; i++)
        {
            if (cardScript[i].cardNumber == numberToCheck)
            {
                cardNumberFound++;
                if (cardNumberFound == 2) return true;
            }
        }

        return false;
    }

    public GameStates GetCurrentState()
    {
        return currentState;
    }

    public void SetGameState(GameStates stateToChange)
    {
        currentState = stateToChange;
    }

    private void CallSignInMenu()
    {
        isShowingCards = false; 
        currentState = GameStates.SignIn; 
    }

    private string TimeToString(float time)
    {       
        string minutesString;
        string secondsString;

        int minutes;
        int seconds;

        minutes = (int)time / 60;
        seconds = (int)time % 60;

        minutesString = minutes.ToString();
        secondsString = seconds.ToString();

        if (minutes < 10)
        {
            minutesString = "0" + minutesString;
        }

        if (seconds < 10)
        {
            secondsString = "0" + secondsString;
        }

        return minutesString + ":" + secondsString;
    }

    private int GetTotalScore()
    {
        int total;        

        total = (int)timeToEndMatch;       
        total *= 10;        
        total += score;
        partialScore = total;
        total -= numberOfMisses * 10;

        if (total < 0)
        {
            total = 0;
        }

        return total;
    }    

    private void UnflipCards(CardScript cardOne, CardScript cardTwo)
    {
        numCardsFlipped = 0;

        if (cardOne != null && cardTwo != null)
        {
            cardOne.isFlipping = true;
            cardTwo.isFlipping = true; 
        }
        
        cardsFlipped[0] = null;
        cardsFlipped[1] = null;
       
    }    

    private void UnflipAllCards()
    {
        CardScript[] flippedCardScript = FindObjectsOfType(typeof(CardScript)) as CardScript[];
        
        for (int i = 0; i < flippedCardScript.Length; i++)
        {            
            flippedCardScript[i].isFlipping = true;
        }

        numCardsFlipped = 0;
        isShowingCards = false;
    }

    private void FlipAllCards()
    {
        GameObject[] flippedCards = GameObject.FindGameObjectsWithTag("card");
        CardScript[] flippedCardScript = new CardScript [flippedCards.Length]; //= FindObjectsOfType(typeof(CardScript)) as CardScript[];  

        for (int i = 0; i < flippedCards.Length; i++)
        {
            flippedCardScript[i] = flippedCards[i].GetComponent<CardScript>();
        }

        for (int i = 0; i < flippedCardScript.Length; i++)
        {
            flippedCardScript[i].isFlipping = true;
        }

        numCardsFlipped = NumberOfCards() * 2;
        isShowingCards = true;
    }

    public IEnumerator StartGame()
    {        
        if (!cards[0].GetComponent<Renderer>().enabled)
        {            
            ToggleCardsRenderer(); 
        }

        if (totalCardsFlipped.Length > 0)
        {            
            SetCardTextures();
            ShuffleCards();           
        }        

        FlipAllCards();

        yield return new WaitForSeconds(5f);

        ActivateInGameUI();

        UnflipAllCards();

        currentState = GameStates.InGame;                   
    }

    private void ResetGameData()
    {      
        cardsParent.SetActive(true);                 
        numberOfMisses = 0;
        numberOftries = 0;
        numOfPairs = 0;      
        score = 0;
        totalNumCardsFlipped = 0;
        timeToCallEndGame = 0;
        timeToDecrementOneSecond = 0;
        SetMatchTime();
        timeToUnflip = 0;
        totalScore = 0;

        PointsText.text = "0";
        NumOfTriesText.text = "0";

        if (cardsFlipped != null)
        {
            cardsFlipped[0] = null;
            cardsFlipped[1] = null;
        }

        if (cardScript != null)
        {
            for (int i = 0; i < cardScript.Length; i++)
            {
                cardScript[i].cardNumber = 13;
                cardScript[i].isFlipping = false;
            }
        }                                   
    }

    private void ReorderCards()
    {
        GetCards();
        SetCardTextures();
        ShuffleCards();
    }      

    public void ToggleVirtualKeyboard(bool value)
    {
        print(Application.platform);
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            virtualKeyboard.ToggleVirtualKeyboard(value);
    }

    private void SetCardsParent()
    {
        switch (difficulty)
        {
            default:
                cardsParent = easyCards;
                break;

            case 1:
                cardsParent = mediumCards;
                break;

            case 2:
                cardsParent = hardCards;
                break;
        }        
    }

    private void SetMatchTime()
    {
        switch (difficulty)
        {
            default:
                timeToEndMatch = 60f;
                //timeToEndMatch = 3f;
                break;
            case 1:
                timeToEndMatch = 75f;
                break;
            case 2:
                timeToEndMatch = 90f;
                break;
        }
    }

    private void SetAllCardsUnflipped()
    {
        CardScript[] scripts = FindObjectsOfType(typeof(CardScript)) as CardScript[];

        foreach (CardScript script in scripts)
        {
            script.isFlipping = false;
            script.SetCardState(CardStates.Back);
            script.gameObject.tag = "card";
            script.gameObject.GetComponent<Renderer>().material.mainTexture = script.backTexture;
            script.gameObject.transform.rotation = Quaternion.identity;
        }
    }   
    
    public void SetPlayerName()
    {
        playerName = nameInputField.text;
    }
    
    public void SetPlayerEmail()
    {
        playerEmail = emailInputField.text;
    }

    public void SetPlayerPhone()
    {
        playerPhone = phoneInputField.text;
    }

    public void FormatPhoneInput()
    {
        if (!string.IsNullOrEmpty(phoneInputField.text))        
            phoneInputField.text = string.Format("{0:#### - ####}", int.Parse(phoneInputField.text));                                              
    }

    private void ActivateMainMenuUI()
    {               
        MainMenuUI.SetActive(true);
    }

    private void ActivateInGameUI()
    {
        InGameUI.SetActive(true);
    }

    public bool forceWin;

    private void ActivateEndGameUI()
    {
        PairsPointText.text = score.ToString();
        TimePointText.text = (timeToEndMatch * 10).ToString();
        MissesPointText.text = string.Format("-{0}", numberOfMisses * 10);
        TotalScoreText.text = string.Format("{0} pontos", totalScore.ToString());

        if (totalScore >= 970)
        {
            prizeStar.SetActive(true);
        }

        EndGameUI.SetActive(true);
        if (totalNumCardsFlipped == cardTextures.Length * 2 || forceWin)
        {
            smileFace.SetActive(true);
            defaultEndingTitle.SetActive(false);
            bestEndingTitle.SetActive(true);
        }
        else
        {
            smileFace.SetActive(false);
            defaultEndingTitle.SetActive(true);
            bestEndingTitle.SetActive(false);
        }
    }

    private void ActivateRankingUI()
    {
        RankingUI.SetActive(true);
    }

    private void DeactivateRankingUI()
    {
        RankingUI.SetActive(false);
    }

    private void DeactivateEndGameUI()
    {
        if (prizeStar.activeInHierarchy)
        {
            prizeStar.SetActive(false);
        }
        EndGameUI.SetActive(false);
    }

    private void DeactivateInGameUI()
    {
        InGameUI.SetActive(false);
    }

    private void DeactivateMainMenuUI()
    {                
        MainMenuUI.SetActive(false);
    }

    private void DeactivateSettingsUI()
    {
        SettingsUI.SetActive(false);
    }    

    public void Set2ndBackgroundActivation(bool value)
    {
        NewBackgroundImg.SetActive(value);
    }

    public void CallStartGame()
    {
        DeactivateMainMenuUI();
        //VirtualKeyboard.HideTouchKeyboard();
        numberOfMatches++;        
        ClearInputFields();

        if (numberOfMatches > 1)  ResetGameData();
        else cardsParent.SetActive(true);

        if ((totalCardsFlipped.Length > 0) || numberOfMatches != 0)
            ReorderCards();

        SetMatchTime();
        StartCoroutine(StartGame());        
    }

    public void CallRankingMenu()
    {
        DeactivateMainMenuUI();
        //VirtualKeyboard.HideTouchKeyboard();
        BackButton.gameObject.SetActive(true);
        ActivateRankingUI();
        currentState = GameStates.Ranking;
    }

    public void CallSettingsMenu()
    {
        DeactivateMainMenuUI();
        //VirtualKeyboard.HideTouchKeyboard();
        BackButton.gameObject.SetActive(true);
        SettingsUI.SetActive(true);
        ErrorMessage.text = "";
        currentState = GameStates.Settings;
    }

    public void CallMainMenu()
    {
        if (currentState == GameStates.Settings)
            DeactivateSettingsUI();

        else if (currentState == GameStates.Ranking)
            DeactivateRankingUI();

        currentState = GameStates.SignIn;
        BackButton.gameObject.SetActive(false);
        ActivateMainMenuUI();
    }

    public void CallResetConfirmation()
    {
        BackButton.gameObject.SetActive(false);
        ResetButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        UpdateGraphicsButton.gameObject.SetActive(false);
        ErrorMessage.gameObject.SetActive(false);
        ResetConfirmation.gameObject.SetActive(true);
    }

    public void CallQuitConfirmation()
    {
        BackButton.gameObject.SetActive(false);
        ResetButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        UpdateGraphicsButton.gameObject.SetActive(false);
        ErrorMessage.gameObject.SetActive(false);
        QuitConfirmation.gameObject.SetActive(true);
    }

    public void BackFromResetToSettings()
    {
        ResetConfirmation.SetActive(false);
        ResetButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
        ErrorMessage.gameObject.SetActive(true);
        ErrorMessage.text = "";
        UpdateGraphicsButton.gameObject.SetActive(true);
    }

    public void BackFromQuitToSettings()
    {
        QuitConfirmation.SetActive(false);
        ResetButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
        ErrorMessage.gameObject.SetActive(true);
        ErrorMessage.text = "";
        UpdateGraphicsButton.gameObject.SetActive(true);
    }

    private void ClearInputFields()
    {
        nameInputField.text = "";
        emailInputField.text = "";
        phoneInputField.text = "";
    }

    public void CallResetGame()
    {
        StartCoroutine("ResetGame");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator ActivateAllGameObjects()
    {
        GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        foreach (GameObject gameObject in gameObjects)
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.5f);        
    }

    public void BackToMainMenuUI()
    {
        DeactivateEndGameUI();
        if (prizeStar.activeInHierarchy)
        {
            prizeStar.SetActive(false);
        }

        cardScript = null;
        numOfResets = 0;

        ResetPlayerData();
        currentState = GameStates.SignIn;
        ActivateMainMenuUI();               
    }

    private IEnumerator ResetGame()
    {
        DeactivateInGameUI();
        ResetGameButton.gameObject.SetActive(false);
        numOfResets++;
        currentState = GameStates.Reseting;

        SetAllCardsUnflipped();

        ResetGameData();        

        SetCardTextures();
        ShuffleCards();        

        FlipAllCards();

        yield return new WaitForSeconds(5f);

        UnflipAllCards();        

        currentState = GameStates.InGame;
        ActivateInGameUI();
    }

    void OnGUI()
    {
        //GUI.Label(new Rect(Screen.width / 4, Screen.height / 2, 1000, 1000), Application.dataPath);
    }
}