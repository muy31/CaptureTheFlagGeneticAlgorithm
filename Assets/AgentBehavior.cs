using UnityEngine;

public class AgentBehavior : MonoBehaviour
{
    public float speed = 2;
    public Vector3 homeBase;
    public bool tagged = false;
    public Flag holdingFlag = null;

    private MeshRenderer Renderer;
    private bool blueAgent;
    public Vector3 moveDelta = Vector3.zero;
    private Rigidbody myRb;

    public void SetBlueAgent(bool isBlue)
    {
        blueAgent = isBlue;
        if (blueAgent)
        {
            gameObject.tag = "Blue Agent";
        }
        else
        {
            gameObject.tag = "Red Agent";
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = GetComponent<MeshRenderer>();
        myRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!tagged)
        {
            Move();
        }
        else
        {
            MoveToBase();
        }
        
        UpdateColor();
    }

    public void ReceiveBrainOutput(double deltaX, double deltaY)
    {
        Vector3 moveVec = new Vector3((float) deltaX, (float) deltaY).normalized;
        moveDelta = moveVec;
    }

    void Move()
    {
        // gameObject.transform.position += moveDelta * speed * Time.deltaTime;
        myRb.maxLinearVelocity = speed;
        myRb.AddForce(moveDelta * Time.deltaTime * speed, ForceMode.VelocityChange);
    }

    void MoveToBase()
    {
        // myRb.isKinematic = true;
        Vector3 moveVec = (homeBase - transform.position);
        gameObject.transform.localPosition += moveVec.normalized * speed * Time.deltaTime; //  Forcefully move towards base
        // Push towards base
        // myRb.AddForce(moveVec.normalized * Time.deltaTime * speed, ForceMode.VelocityChange);

        if (moveVec.magnitude <= 0.5f)
        {
            tagged = false;
            if (blueAgent)
                gameObject.tag = "Blue Agent";
            else gameObject.tag = "Red Agent";
            // myRb.isKinematic = false;
        }
    }

    void UpdateColor()
    {
        if (holdingFlag != null)
        {
            if (holdingFlag.tag == "Red Flag")
            {
                if (blueAgent)
                    Renderer.material.color = Color.yellow;
                else Renderer.material.color = Color.magenta;
            }
            else
            {
                if (blueAgent)
                    Renderer.material.color = Color.green;
                else Renderer.material.color = Color.grey;
            }       
        }
        else
        {
            if (blueAgent)
                Renderer.material.color = Color.blue;
            else Renderer.material.color = Color.red;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // If tagged by the opposing team
        if ((blueAgent && collision.collider.tag == "Red Agent") || (!blueAgent && collision.collider.tag == "Blue Agent"))
        {
            if (holdingFlag != null)
            {
                holdingFlag.controllingAgent = null; // Essentially drop the flag
                holdingFlag = null;
            }
            tagged = true;
            // Debug.Log(gameObject + " Blue? : " + blueAgent + " has been tagged by " + collision.collider);
            gameObject.tag = "Untagged"; // Prevent interactions with other agents / flags until safe


            // Do the same to opposing collider
            AgentBehavior other = collision.collider.GetComponent<AgentBehavior>();
            if (other != null)
            {
                if (other.holdingFlag != null)
                {
                    other.holdingFlag.controllingAgent = null;
                    other.holdingFlag = null;
                }
                other.tagged = true;
                other.gameObject.tag = "Untagged";
                // Debug.Log(gameObject + " Blue? : " + blueAgent + " has been tagged by " + collision.collider);
            }
        }
    }
}
