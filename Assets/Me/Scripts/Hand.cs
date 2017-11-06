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
        private Rigidbody mHeldObject;
        private FixedJoint mTempJoint;
        private Vector3 mOldVelocity;
        public float throwThreshold = 5f;
        public Vector3 scalingThrowSpeed = new Vector3(5, 5, 5);
        public GameObject playerController;
        private CharacterController characterController;
        

    //TODO Rewrite so that can grab something after letting it go and hand still colliding with the object

        // Use this for initialization
        void Start()
        {
            if (AttachPoint == null)
            {
                AttachPoint = GetComponent<Rigidbody>();
            }

     //   characterController = playerController.GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (mHandState)
            {
                case State.TOUCHING:
                    if (mTempJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) >= 0.5f)
                    {
                        mHeldObject.velocity = Vector3.zero;
                        mTempJoint = mHeldObject.gameObject.AddComponent<FixedJoint>();
                        mTempJoint.connectedBody = AttachPoint;
                        mHandState = State.HOLDING;
                    //disable character controller collisions while holding things
         //           characterController.detectCollisions = false;
                }
                    break;
                case State.HOLDING:
                
                if (mTempJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) < 0.5f)
                    {
                        Object.DestroyImmediate(mTempJoint);
                        mTempJoint = null;
                    //To stop collisions with hands when throwing an object
                    gameObject.GetComponent<SphereCollider>().enabled = false;
                        throwObject();
                    gameObject.GetComponent<SphereCollider>().enabled = true;
                    mHandState = State.EMPTY;
                    //re enable character controller collisions after letting go of things
              //      characterController.detectCollisions = true;
                }
                    mOldVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);
                
                break;
            }
        }

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
        else {
            //if you are not exceeding the threshold for throwing something, set its velocity to 0
            mHeldObject.velocity = new Vector3(0, 0, 0);
            mHeldObject.useGravity = false;
        }
    }
    }

 