using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class TrainerManager : GameManager
{
    public System.Collections.Generic.Queue<Brain> brainQueue;

    private void Awake()
    {
        brainQueue = new System.Collections.Generic.Queue<Brain>();
        if (grabFromFile)
        {
            winningBrain = dataSaver.LoadFile();
        }
        else
        {
            winningBrain = null;
        }
        newGame = true;
        if (winningBrain != null)
        {
            brainQueue.Enqueue(winningBrain);
        }
    }

    void NewGame()
    {
        episode++;
        Debug.Log(brainQueue.Count);

        if (episode >= saveRate)
        {
            episode = 0;
            string name = dataSaver.SaveFile(winningBrain);
            texter.UpdateText("Saved to: " + name);
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

    private void Start()
    {
        if (newGame)
            NewGame();
    }

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

    Brain getNextBrain()
    {
        if (brainQueue.Count > 0)
        {
            return brainQueue.Dequeue();
        }
        else
        {
            return new Brain(teamBlue.getNumInputs(), teamBlue.getNumOutputs(), teamBlue.networkLayers, teamBlue.networkComplexity);
        }
    }

    void CheckAndFixWin()
    {
        if (teamRed.HasWon())
        {
            newGame = true;
            teamRed.SetWonOff();
            teamBlue.SetWonOff();
            teamBlue.teamBrain = getNextBrain(); // Replace the loser with a new Brain
        }
        else if (teamBlue.HasWon())
        {
            newGame = true;
            teamBlue.SetWonOff();
            teamRed.SetWonOff();
            brainQueue.Enqueue(teamBlue.teamBrain);
            winningBrain = teamBlue.teamBrain;
            Brain newBrain = teamBlue.teamBrain.cloneAndMutate();
            teamBlue.teamBrain = newBrain; // Mutate the winner a bit
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
                Debug.Log("Team red won since they were holding more flags! RED: " + redHoldScore + " BLUE: " + blueHoldScore);
                teamBlue.teamBrain = getNextBrain();
            }
            else if (blueHoldScore > redHoldScore)
            {
                Debug.Log("Team blue won since they were holding more flags! RED: " + redHoldScore + " BLUE: " + blueHoldScore);
                Brain newBrain = teamBlue.teamBrain.cloneAndMutate();
                brainQueue.Enqueue(teamBlue.teamBrain);
                winningBrain = teamBlue.teamBrain;
                teamBlue.teamBrain = newBrain;
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
                    teamBlue.teamBrain = getNextBrain();
                }
                else
                {
                    Debug.Log("Red Distance: " + redDist + " Blue Distance: " + blueDist);
                    Debug.Log("Team blue won by combined score! RED: " + redScore + " BLUE: " + blueScore);
                    Brain newBrain = teamBlue.teamBrain.cloneAndMutate();
                    brainQueue.Enqueue(teamBlue.teamBrain);
                    winningBrain = teamBlue.teamBrain;
                    teamBlue.teamBrain = newBrain;
                }
            }
        }
    }
}
