using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Team teamRed;
    public Flag redFlag;
    public Team teamBlue;
    public Flag blueFlag;

    public Transform redBase;
    public Transform blueBase;
    public bool newGame = true;

    Vector3 maxFlagPosition = new Vector3(10.5f, 4.5f, 0f);
    Vector3 maxBasePosition = new Vector3(10f, 4f, 0f);

    public float maxGameTime = 10f;  // 15 seconds until a winner is forcefully chosen
    public int saveRate = 5; // Number of episodes before we save the winning brain
    public int episode = 0;
    public float currentGameTime = 0f;
    public Brain winningBrain = null;
    public bool grabFromFile = false;
    public DataSaver dataSaver;
    public TimeScaleSetter timeScaleSetter;
    public TextDebug texter;

    private void Awake()
    {
        if (grabFromFile)
        {
            winningBrain = dataSaver.LoadFile();
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
    void Update()
    {
        Time.timeScale = timeScaleSetter.timeScale;
        currentGameTime += Time.deltaTime;
        if (newGame)
        {
            NewGame();
        }
        else
        {
            CheckAndFixWin();
        }
    }

    void NewGame()
    {
        episode++;

        if (episode >= saveRate)
        {
            episode = 0;
            string fName = dataSaver.SaveFile(winningBrain);
            texter.UpdateText("Saved a trained brain to: " + fName);
        }

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

    public void placeBases()
    {
        blueBase.localPosition = new Vector3(Random.Range(-maxBasePosition.x, maxBasePosition.x), Random.Range(-maxBasePosition.y, maxBasePosition.y));
        redBase.localPosition = new Vector3(Random.Range(-maxBasePosition.x, maxBasePosition.x), Random.Range(-maxBasePosition.y, maxBasePosition.y));
    }

    public void placeFlags()
    {
        Vector3 candidateBlue = new Vector3(Random.Range(-maxFlagPosition.x, maxFlagPosition.x), Random.Range(-maxFlagPosition.y, maxFlagPosition.y));
        Vector3 candidateRed = new Vector3(Random.Range(-maxFlagPosition.x, maxFlagPosition.x), Random.Range(-maxFlagPosition.y, maxFlagPosition.y));
        while (Vector3.Distance(candidateBlue, redBase.localPosition) <= 4f)
        {
            candidateBlue = new Vector3(Random.Range(-maxFlagPosition.x, maxFlagPosition.x), Random.Range(-maxFlagPosition.y, maxFlagPosition.y));
        }
        while (Vector3.Distance(candidateRed, blueBase.localPosition) <= 4f)
        {
            candidateRed = new Vector3(Random.Range(-maxFlagPosition.x, maxFlagPosition.x), Random.Range(-maxFlagPosition.y, maxFlagPosition.y));
        }
        blueFlag.transform.localPosition = candidateBlue;
        redFlag.transform.localPosition = candidateRed;
        redFlag.baseTagTrigger = "Blue Base";
        blueFlag.baseTagTrigger = "Red Base";
        redFlag.myTeam = teamRed;
        blueFlag.myTeam = teamBlue;
        redFlag.controllingAgent = null;
        blueFlag.controllingAgent = null;
    }

    void CheckAndFixWin()
    {
        if (teamRed.HasWon())
        {
            newGame = true;
            teamRed.SetWonOff();
            teamBlue.SetWonOff();
            winningBrain = teamRed.teamBrain;
            teamBlue.teamBrain = teamRed.teamBrain.cloneAndMutate(); // Replace the loser with a mutation of the winner
        }
        else if (teamBlue.HasWon())
        {
            newGame = true;
            teamBlue.SetWonOff();
            teamRed.SetWonOff();
            winningBrain = teamBlue.teamBrain;
            Brain newBrain = teamBlue.teamBrain.cloneAndMutate();
            teamRed.teamBrain = newBrain; // Replace the loser with a mutationof the winner
        }
        else if (currentGameTime >= maxGameTime)
        {
            teamRed.SetWonOff();
            teamBlue.SetWonOff();
            newGame = true;

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
                texter.UpdateText("Team red won since they were holding more flags! RED: " + redHoldScore + " BLUE: " + blueHoldScore);
                Brain newBrain = teamRed.teamBrain.cloneAndMutate();
                teamBlue.teamBrain = newBrain;
                winningBrain = teamRed.teamBrain;
            }
            else if (blueHoldScore > redHoldScore)
            {
                texter.UpdateText("Team blue won since they were holding more flags! RED: " + redHoldScore + " BLUE: " + blueHoldScore);
                Brain newBrain = teamBlue.teamBrain.cloneAndMutate();
                teamRed.teamBrain = newBrain;
                winningBrain = teamBlue.teamBrain;
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
                    texter.UpdateText("Red Distance: " + redDist + " Blue Distance: " + blueDist);
                    Debug.Log("Team red won by combined score! RED: " + redScore + " BLUE: " + blueScore);
                    Brain newBrain = teamRed.teamBrain.cloneAndMutate();
                    teamBlue.teamBrain = newBrain;
                    winningBrain = teamRed.teamBrain;
                }
                else
                {
                    texter.UpdateText("Red Distance: " + redDist + " Blue Distance: " + blueDist);
                    Debug.Log("Team blue won by combined score! RED: " + redScore + " BLUE: " + blueScore);
                    Brain newBrain = teamBlue.teamBrain.cloneAndMutate();
                    teamRed.teamBrain = newBrain;
                    winningBrain = teamBlue.teamBrain;
                }
            }
        }
    }
}
