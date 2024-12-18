using UnityEngine;

public class RandomTeamController : Team
{
    // Update is called once per frame
    void Update()
    {
        foreach (AgentBehavior a in agentBehaviors)
        {
            float dir = Random.Range(0f, 2*Mathf.PI);
            a.moveDelta = new Vector3(Mathf.Sin(dir), Mathf.Cos(dir)).normalized;
        }
    }
}
