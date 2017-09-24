using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    public float              moveTime = 100.0f;           //Time it will take object to move, in seconds.
    public LayerMask          blockingLayer;             //Layer on which collision will be checked.


    private BoxCollider2D     boxCollider;      //The BoxCollider2D component attached to this object.
    private Rigidbody2D       rb2D;               //The Rigidbody2D component attached to this object.
    private float             inverseMoveTime;          //Used to make movement more efficient.
    private bool              selected = false;
	
	// Update is called once per frame
	void Update () {
        if (this.selected == true && Input.GetMouseButtonUp(0))
        {
            Debug.Log("left clicked");
            this.selected = false;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Move((int)pos.x, (int)pos.y);
        }
		
	}

    private void OnMouseDown()
    {
        this.selected = true;
    }

    //Protected, virtual functions can be overridden by inheriting classes.
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        //Get a component reference to this object's Rigidbody2D
        rb2D = GetComponent<Rigidbody2D>();

        //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
        inverseMoveTime = 1f / moveTime;
    }


    //Move returns true if it is able to move and false if not. 
    //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
    protected bool Move(int xDir, int yDir)
    {
        Vector2 start = transform.position;
        Vector2 end   = new Vector2(xDir, yDir);
        StartCoroutine(SmoothMovement(end));

        return true;
    }


    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }
}
