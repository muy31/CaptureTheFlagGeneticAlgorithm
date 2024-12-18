using UnityEngine;

public class Team : MonoBehaviour
{
    public GameManager gameManager;
    public Transform homeBase;  // Same as goal
    public Transform enemyBase; // Same as enemy's goal
    public Flag myFlag;
    public Flag oppFlag; // Key is to get opponent's flag to your homeBase

    public Brain teamBrain;
    public AgentBehavior[] agentBehaviors;
    public int networkComplexity;
    public int networkLayers;

    private int requiredOutputsPerAgent = 2; // x delta and y delta?
    private int numInformationPerAgent = 5; // my x and y, tagged boolean (0, 1), holdingFlag (-1, 0, 1), speed,
    private int numInputs;
    private bool hasWon = false;
    public bool blueTeam;

    public float teamPlayScore = 0;

    public int getNumInputs()
    {
        return numInputs;
    }

    public int getNumOutputs()
    {
        return agentBehaviors.Length * requiredOutputsPerAgent;
    }

    private void Start()
    {
        numInputs = 8 + agentBehaviors.Length * numInformationPerAgent;
        int numOutputs = agentBehaviors.Length * requiredOutputsPerAgent;
        if (gameManager.winningBrain != null && gameManager.grabFromFile)
        {
            teamBrain = gameManager.winningBrain;
        }
        else
        {
            teamBrain = new Brain(numInputs, numOutputs, networkLayers, networkComplexity);
        }
        
        Debug.Log(teamBrain.toString());
    }

    float[] collectInput()
    {
        float[] inputs = new float[numInputs];
        inputs[0] = homeBase.localPosition.x;
        inputs[1] = homeBase.localPosition.y;
        inputs[2] = enemyBase.localPosition.x;
        inputs[3] = enemyBase.localPosition.y;
        inputs[4] = myFlag.transform.localPosition.x;
        inputs[5] = myFlag.transform.localPosition.y;
        inputs[6] = oppFlag.transform.localPosition.x;
        inputs[7] = oppFlag.transform.localPosition.y;

        for (int i = 0; i < agentBehaviors.Length; i++)
        {
            inputs[numInformationPerAgent * i + 8] = agentBehaviors[i].transform.localPosition.x;
            inputs[numInformationPerAgent * i + 9] = agentBehaviors[i].transform.localPosition.y;
            inputs[numInformationPerAgent * i + 10] = agentBehaviors[i].tagged ? 1 : 0;
            inputs[numInformationPerAgent * i + 11] = agentBehaviors[i].holdingFlag == null? 0: agentBehaviors[i].holdingFlag == oppFlag ? 1: -1;
            inputs[numInformationPerAgent * i + 12] = agentBehaviors[i].speed;

            if (Mathf.Abs(agentBehaviors[i].transform.localPosition.y) > 4.5f)
            {
                teamPlayScore -=  Time.deltaTime;
            }

            if (Mathf.Abs(agentBehaviors[i].transform.localPosition.x) > 10.5f)
            {
                teamPlayScore -= Time.deltaTime;
            }
        }

        return inputs;
    }

    void SendOutputs(float[] outputFrame)
    {
        // Debug.Log(string.Join(", ", outputFrame));
        for (int i = 0; i < agentBehaviors.Length; i++)
        {
            agentBehaviors[i].ReceiveBrainOutput(outputFrame[2*i], outputFrame[2*i+1]);
        }
    }

    private void Update()
    {
        float[] inputFrame = collectInput();
        float[] outputs = teamBrain.PredictOutput(inputFrame);
        SendOutputs(outputs);
    }

    public void claimVictory()
    {
        hasWon = true;
        Debug.Log(this.ToString() + " " + "claims victory!");
    }
    public bool HasWon()
    {
        return hasWon;
    }

    public void SetWonOff()
    {
        hasWon = false;
    }

    public void SetTeamAgents()
    {
        foreach (AgentBehavior agent in agentBehaviors)
        {
            Vector3 delta = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            agent.transform.position = homeBase.position + delta;

            // Debug.Log("Moved " + agent.gameObject + " to " + agent.transform.position);

            agent.speed = Random.Range(1.5f, 4f);
            agent.homeBase = homeBase.position;
            agent.tagged = false;
            agent.holdingFlag = null;
            agent.SetBlueAgent(blueTeam);
            agent.moveDelta = Vector3.zero;
        }
        teamPlayScore = 0;
        hasWon = false;
    }
}
