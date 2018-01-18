using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public enum State
    {
        EMPTY,
        TOUCHING,
        HOLDING
    };

    public OVRInput.Controller Controller = OVRInput.Controller.LTouch;
    public State mHandState = State.EMPTY;
    public Rigidbody AttachPoint = null;
    public bool IgnoreContactPoint = false;
    public Rigidbody mHeldObject;
    public FixedJoint mTempJoint;
    private Vector3 mOldVelocity;
    public float throwThreshold = 5f;
    public Vector3 scalingThrowSpeed = new Vector3(5, 5, 5);
    public GameObject playerController;
    private CharacterController characterController;


    //TODO Rewrite so that can grab something after letting it go and hand still colliding with the object

    //TODO bug where state stays at holding after awhile 

    // Use this for initialization
    /// <summary>
    // Upon the hands’ initialization we will get the rigid body component within it.  
    //This will be used as an attach point for our hands when we pick up objects,
    //if we have not specified an attach point in the Inspector view
    /// </summary>
    void Start()
    {
        if (AttachPoint == null)
        {
            AttachPoint = GetComponent<Rigidbody>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch (mHandState)
        {
            //If there is no temporary joint and the player is pressing down on the hand trigger with enough pressure (>= 0.5f in this case), 
            //we set the held object’s velocity to zero, and create a temporary joint attached to it. 
            //We then connect that joint to our AttachPoint (by default, the hand itself) 
            //and set the hand state to HOLDING
            case State.TOUCHING:
             //   Debug.LogWarning(mHandState);

                if (mTempJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) >= 0.5f)
                {
                    mHeldObject.velocity = Vector3.zero;
                    mTempJoint = mHeldObject.gameObject.AddComponent<FixedJoint>();
                    mTempJoint.connectedBody = AttachPoint;
                    mHandState = State.HOLDING;

                }
                break;

                // If the hand state is already in the HOLDING state, 
                //we check that we do have a temporary joint (i.e. that it is not null) 
                //and that the player is releasing enough of the trigger (in this case, < 0.5f).  
                //If so, we immediately destroy the temporary joint, and set it to null signifying that it is no longer in use.  
                //We then throw the object using a throw method (described further below) and set the hand state to EMPTY.
            
            case State.HOLDING:
            //    Debug.LogWarning(mHandState);

                if (mTempJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) < 0.5f)
                {
                    Object.DestroyImmediate(mTempJoint);
                    mTempJoint = null;

                    //To stop collisions with hands when throwing an object
                    gameObject.GetComponent<SphereCollider>().enabled = false;
                    throwObject();
                    gameObject.GetComponent<SphereCollider>().enabled = true;
                    mHandState = State.EMPTY;

                }
                else if (mHeldObject == null) {
                    mHandState = State.EMPTY;
                }
                    mOldVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);

                break;
        }
    }

    /// <summary>
    /// handles when a hand collides with an object.  
    /// It checks that we do not have something in this hand already, 
    /// and then ensures that the object is on the grabbable layer and has a rigid body component attached to it.  
    /// We then store it in our held object and change the hand state to touching
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter(Collider collider)
    {
        if (mHandState == State.EMPTY && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) < 0.5f)
        {

            GameObject temp = collider.gameObject;
            if (temp != null && temp.layer == LayerMask.NameToLayer("grabbable") && temp.GetComponent<Rigidbody>() != null)
            {
                mHeldObject = temp.GetComponent<Rigidbody>();
                mHandState = State.TOUCHING;
            }
        }
    }

    /// <summary>
    /// checking that we were not holding an object,
    /// and that the object we are no longer touching was on the grabbable layer. 
    /// We then set the held object to null and set the hand state to empty.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerExit(Collider collider)
    {
        if (mHandState != State.HOLDING)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("grabbable"))
            {
                mHeldObject = null;
                mHandState = State.EMPTY;
            }
        }
    }

    private void throwObject()
    {
        Debug.Log("Vector Distance: " + Vector3.Distance(OVRInput.GetLocalControllerAngularVelocity(Controller), new Vector3(1, 1, 1)));

        //Only throw object if controller velocity exceeds some threshold


        if (Vector3.Distance(OVRInput.GetLocalControllerAngularVelocity(Controller), new Vector3(1, 1, 1)) > throwThreshold)
        {
            mHeldObject.velocity = OVRInput.GetLocalControllerVelocity(Controller);
            if (mOldVelocity != null)
            {
                // mHeldObject.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);

                //Increase throw speed using scalingThrowSpeed
                mHeldObject.angularVelocity = Vector3.Scale(OVRInput.GetLocalControllerAngularVelocity(Controller), scalingThrowSpeed);
            }
            mHeldObject.maxAngularVelocity = mHeldObject.angularVelocity.magnitude;

            // lock the rotation so it looks smooth
            mHeldObject.freezeRotation = true;
        }
        else
        {
            //if you are not exceeding the threshold for throwing something, set its velocity to 0
            mHeldObject.velocity = new Vector3(0, 0, 0);
            mHeldObject.useGravity = false;
        }
    }
}

