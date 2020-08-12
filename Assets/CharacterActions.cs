using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterActions : MonoBehaviour
{
    [SerializeField] private float pickupDistance;
    [SerializeField] private Vector3 objectInHandOffset;
    [SerializeField] private LayerMask pickableMask;
    [SerializeField] private float canDepositCheckRadius;
    [SerializeField] private float depositBoxOffset;
    [SerializeField] private Vector3 depositPointCoords;
    [SerializeField] private float depositRayY = 2.3f;
    
    public Transform depositPoint;
    public Grid snapGrid;
    public GameObject previewPlace;
    public ScoreManager ScoreManager;    
    
    RaycastHit depositHit;
    private Boolean objectInFront = false;
    private Boolean hasObject = false;
    private GameObject objectInHand;
    private CharacterMovement characterMovement;

    private void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        if (hasObject)
        {
            Physics.Raycast(depositPoint.position, Vector3.down, out depositHit);
            
            objectInHand.transform.position = transform.position + objectInHandOffset; 
            
            Vector3 previewObjectPos = snapGrid.CellToWorld(snapGrid.WorldToCell(depositHit.point));

            previewPlace.transform.position =
                new Vector3(previewObjectPos.x, previewPlace.transform.position.y, previewObjectPos.z);

            previewPlace.transform.rotation = objectInHand.transform.rotation;
        }
    }

    public void PickupObject(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RaycastHit hit;
            objectInFront = Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance, pickableMask);

            if (objectInFront && !hasObject)
            {
                hasObject = true;
                objectInHand = hit.transform.gameObject;
                objectInHand.GetComponent<Rigidbody>().freezeRotation = true;
                objectInHand.GetComponent<Rigidbody>().useGravity = false;
                objectInHand.GetComponent<Rigidbody>().detectCollisions = false;
                objectInHand.transform.SetParent(transform);
                objectInHand.transform.SetPositionAndRotation(transform.position, new Quaternion(0, 0, 0, 0));

                depositPoint.transform.localPosition =
                    new Vector3(0, depositRayY, objectInHand.GetComponent<Collider>().bounds.size.z);

                previewPlace.transform.localScale = new Vector3(objectInHand.transform.localScale.x, 0,
                    objectInHand.transform.localScale.z);
                previewPlace.transform.localRotation = new Quaternion(0, 0, 0, 0);
                
                previewPlace.SetActive(true);

            }
            else if (hasObject && Physics.OverlapSphere(depositPoint.position, canDepositCheckRadius).Length == 0)
            {
                if (depositHit.point.y <= transform.position.y)
                {
                    depositHit.point = new Vector3(depositHit.point.x, transform.position.y, depositHit.point.z);
                }

                Vector3 roundedPos = snapGrid.CellToWorld(snapGrid.WorldToCell(depositHit.point + new Vector3(0, depositBoxOffset, 0)));
                
                objectInHand.GetComponent<Rigidbody>().freezeRotation = false;
                objectInHand.GetComponent<Rigidbody>().useGravity = true;
                objectInHand.GetComponent<Rigidbody>().detectCollisions = true;
                
                ScoreManager.BoxPlaced(depositHit.point.y);
                objectInHand.transform.SetParent(null);
                objectInHand.transform.position = new Vector3(roundedPos.x, depositHit.point.y + depositBoxOffset, roundedPos.z);
                previewPlace.SetActive(false);
                
                objectInHand = null;
                hasObject = false;
            }
        }
    }

    public void RotateBoxLeft(InputAction.CallbackContext context)
    {
        if (context.performed && objectInHand != null)
        {
            objectInHand.transform.Rotate(0, -90, 0);
            
            if(depositPoint.transform.localPosition.z == objectInHand.GetComponent<Collider>().bounds.size.z)
            {
                depositPoint.transform.localPosition =
                    new Vector3(0, depositRayY, objectInHand.GetComponent<Collider>().bounds.size.x);
            }
            else
            {
                depositPoint.transform.localPosition =
                    new Vector3(0, depositRayY, objectInHand.GetComponent<Collider>().bounds.size.z);
            }
        }
    }
    
    public void RotateBoxRight(InputAction.CallbackContext context)
    {
        if (context.performed && objectInHand != null)
        {
            objectInHand.transform.Rotate(0, 90, 0);
            
            if(depositPoint.transform.localPosition.z == objectInHand.GetComponent<Collider>().bounds.size.z)
            {
                depositPoint.transform.localPosition =
                    new Vector3(0, depositRayY, objectInHand.GetComponent<Collider>().bounds.size.x);
            }
            else
            {
                depositPoint.transform.localPosition =
                    new Vector3(0, depositRayY, objectInHand.GetComponent<Collider>().bounds.size.z);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.DrawWireSphere(depositPoint.position,canDepositCheckRadius);
    }
}
