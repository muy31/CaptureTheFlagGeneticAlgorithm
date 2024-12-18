using UnityEngine;

public class PlayManager : GameManager
{
    public int numGames = 0;
    public int numBlueWins = 0;

    private void Awake()
    {
        if (grabFromFile)
        {
            dataSaver.loadFileName = FileTextSender.wantedFile;
            Debug.Log(FileTextSender.wantedFile);
            
            winningBrain = dataSaver.LoadFile();
            if (winningBrain == null)
            {
                texter.UpdateText("File " + dataSaver.loadFileName + " was not found. Generating a new untrained agent to play.");
            }
            else
            {
                texter.UpdateText("Attempting to evaluate " + dataSaver.loadFileName + " against random agent team. Successful");
            }
        }
        else
        {
            winningBrain = null;
        }
        newGame = true;
    }

    private void Start()
    {
        if (newGame)
            NewGame();
    }

    // Update is called once per frame
    private void Update()
    {
        Time.timeScale = timeScaleSetter.timeScale;
        currentGameTime += Time.deltaTime;
        if (newGame)
        {
            NewGame();
            texter.UpdateText("Blue win rate: " + (float) numBlueWins / numGames + " (" + numBlueWins + "/" + numGames + ")");
        }
        else
        {
            CheckAndFixWin();
        }
    }

    void NewGame()
    {
        // Move base positions
        placeBases();
        // Reset flag positions and metadata
        placeFlags();
        // Move team agents around home base and set metadata
        teamRed.SetTeamAgents();
        teamBlue.SetTeamAgents();
        newGame = false;
        currentGameTime = 0f;
    }

    void CheckAndFixWin()
    {
        if (teamRed.HasWon())
        {
            newGame = true;
            teamRed.SetWonOff();
            teamBlue.SetWonOff();
            Debug.Log("Team Red victory!");
            numGames++;
        }
        else if (teamBlue.HasWon())
        {
            newGame = true;
            teamBlue.SetWonOff();
            teamRed.SetWonOff();
            Debug.Log("Team Blue Victory!");
            numGames++;
            numBlueWins++;
        }
        else if (currentGameTime >= maxGameTime)
        {
            teamRed.SetWonOff();
            teamBlue.SetWonOff();
            newGame = true;
            numGames++;

            int redHoldScore = 0;
            int blueHoldScore = 0;

            // Winner is the team who is in possession of more flags
            foreach (AgentBehavior a in teamRed.agentBehaviors)
            {
                if (a.holdingFlag != null)
                {
                    redHoldScore++;
                }
            }

            foreach (AgentBehavior a in teamBlue.agentBehaviors)
            {
                if (a.holdingFlag != null)
                {
                    blueHoldScore++;
                }
            }

            Debug.Log("RED: " + redHoldScore + " BLUE: " + blueHoldScore);

            if (redHoldScore > blueHoldScore)
            {
                Debug.Log("Team red won since they were holding more flags! RED: " + redHoldScore + " BLUE: " + blueHoldScore);
            }
            else if (blueHoldScore > redHoldScore)
            {
                Debug.Log("Team blue won since they were holding more flags! RED: " + redHoldScore + " BLUE: " + blueHoldScore);
                numBlueWins++;
            }
            else
            {
                // Winner is team with opponent flag closer to their base and factor in a score that de-incentivize hugging the edge of the map
                float redDist = Vector3.Distance(blueFlag.transform.position, redBase.position);
                float blueDist = Vector3.Distance(redFlag.transform.position, blueBase.position);
                float redScore = -redDist + 0.15f * teamRed.teamPlayScore;
                float blueScore = -blueDist + 0.15f * teamBlue.teamPlayScore;

                if (redScore > blueScore)
                {
                    // Red won, so blue is replaced
                    Debug.Log("Red Distance: " + redDist + " Blue Distance: " + blueDist);
                    Debug.Log("Team red won by combined score! RED: " + redScore + " BLUE: " + blueScore);
                }
                else
                {
                    Debug.Log("Red Distance: " + redDist + " Blue Distance: " + blueDist);
                    Debug.Log("Team blue won by combined score! RED: " + redScore + " BLUE: " + blueScore);
                    numBlueWins++;
                }
            }
        }
    }
}
