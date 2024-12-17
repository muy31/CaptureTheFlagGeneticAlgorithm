using UnityEngine;

public class Flag : MonoBehaviour
{
    public AgentBehavior controllingAgent = null;
    public Team myTeam;
    public string baseTagTrigger;

    private void Update()
    {
        if (controllingAgent != null)
        {
            gameObject.transform.position = controllingAgent.transform.position; // Ultra specific following
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Red Agent" || other.tag == "Blue Agent")
        {
            if (controllingAgent != null)
            {
                controllingAgent.holdingFlag = null;
                controllingAgent = null;
            }
            controllingAgent = other.gameObject.GetComponent<AgentBehavior>();

            if (controllingAgent.holdingFlag != null)
            {
                controllingAgent.holdingFlag.controllingAgent = null; // Picking up one flag requires dropping the other
            }
            controllingAgent.holdingFlag = this;
        } else if (other.tag == baseTagTrigger)
        {
            myTeam.claimVictory();
        } 
    }
}
