using System.Collections.Generic;
using UnityEngine;

public class PanamaxCraneMovement : MonoBehaviour
{
    public enum State { Placing, Taking, Resetting, Idle }
    public State CurState = State.Idle;
    public State PrevState;

    // Crane parts.
    public GameObject Trolley;
    public GameObject Spreader;
    public GameObject Ropes;

    private GameObject Ship;
    private ContainerYardScript StorageYard;

    // Movement variables.
    private Vector3 Target;
    private Vector3 TruckTarget;
    // Use plane/target distance to determine correct positions (so rotation of the crane wont matter).
    private Plane UpForward;
    private Plane UpRight;
    private Plane ForwardRight;
    // How fast each part of the crane should move.
    public float speed = 3f;
    private float RopeSpeed = 0.052f;
    // Specifies how close the crane needs to get to the target (for each axis) to consider it reached.
    public float CraneAccuracy = 0.1f;
    public float TrolleyAccuracy = 0.1f;
    public float SpreaderAccuracy = 0.1f;
    // Distances from the planes to the target position.
    float CraneDistance;
    float TrolleyDistance;
    float SpreaderDistance;
    private float InitialSpreaderPos;

    // Pickup variables.
    private GameObject HeldObj;

    // Test variables.
    private Queue<(bool, Vector3)> TruckQueue;
    bool IsPickup = false;
    float SpreaderUpOffset = 3.7f;


    // Set a target for the crane.
    public void SetTarget(Vector3 position)
    {
       Target = position + transform.up * 3.5f + transform.forward * -2.2f;
       //Target = position + transform.forward * -2.2f;
    }

    public void TargetReached()
    {
        switch (CurState)
        {
            case State.Placing:
                PrevState = State.Placing;
                CurState = State.Resetting;
                Drop();               
                break;
            case State.Taking:
                PrevState = State.Taking;
                CurState = State.Resetting;
                Pickup();
                break;         
            case State.Resetting:
                if(PrevState == State.Taking)
                {                  
                    CurState = State.Placing;
                    AskPlace();                    
                }
                else
                {
                    CurState = State.Idle;
                }
                break;
            default:
                CurState = State.Idle;
                break;
        }
    }

    public void MoveTowardsTarget()
    {
        bool crane = Mathf.Abs(CraneDistance = UpRight.GetDistanceToPoint(Target)) > CraneAccuracy;
        bool trolley = Mathf.Abs(TrolleyDistance = UpForward.GetDistanceToPoint(Target)) > TrolleyAccuracy;

        // Crane movement
        if (crane || trolley)
        {
            if (crane)
            {
                transform.position += CraneDistance > 0f ? speed * Time.deltaTime * transform.right : -speed * Time.deltaTime * transform.right;
                UpRight.Set3Points(transform.up + transform.position, transform.forward + transform.position, transform.position);
            }

            if (trolley)
            {
                Trolley.transform.position += TrolleyDistance < 0f ? speed * Time.deltaTime * Trolley.transform.forward : -speed * Time.deltaTime * Trolley.transform.forward;           
                UpForward.Set3Points(Trolley.transform.up + Trolley.transform.position, Trolley.transform.right + Trolley.transform.position, Trolley.transform.position);
            }
        }
        // Spreader movement
        else if (Mathf.Abs(SpreaderDistance = ForwardRight.GetDistanceToPoint(Target)) > SpreaderAccuracy)
        {
            Spreader.transform.position += SpreaderDistance > 0f ? speed * Time.deltaTime * Spreader.transform.up : -speed * Time.deltaTime * Spreader.transform.up;
            Ropes.transform.localScale += SpreaderDistance > 0f ? RopeSpeed * speed * Time.deltaTime * Spreader.transform.up : -RopeSpeed * speed * Time.deltaTime * Spreader.transform.up;

            ForwardRight.Set3Points(Spreader.transform.forward + Spreader.transform.position, Spreader.transform.right + Spreader.transform.position, Spreader.transform.position);
        }
        else
        {
            TargetReached();
        }
    }

    private void AskTake()
    {
        var take = StorageYard.AskTake();
        //works for unload, not for load
        /*if (take.HasValue && PlayerPrefs.GetInt("isLoadMode") == 1)
        {
            StorageYard.transform.parent.gameObject.SendMessage("OperationFinished");
            CurState = State.Idle;
            Ship = null;
            StorageYard = null;
            Debug.Log("Ship is null");
        }*/
        if (take.HasValue)
        {
            SetTarget(take.Value - transform.up * 3.5f);
        }      
        else
        {
            StorageYard.transform.parent.gameObject.SendMessage("OperationFinished");
            CurState = State.Idle;
            Ship = null;
            StorageYard = null;
            Debug.Log("Ship is null");
        }
    }

    private void AskPlace()
    {
        if (IsPickup)
        {
            var place = StorageYard.AskPlace();
            if (place.HasValue)
            {
                SetTarget(place.Value);
            }
            else
            {
                Ship.SendMessage("OperationFinished");
                CurState = State.Idle;
                Ship = null;
                StorageYard = null;
                Debug.Log("Ship is null");
            }
        }
        else
        {
            SetTarget(TruckTarget);
        }
    }

    private void ResetSpreader()
    {
        Target = new Vector3(Target.x, InitialSpreaderPos, Target.z);
    }

    private void Pickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Spreader.transform.position, -Spreader.transform.up, out hit, 5.0f))
        {
            var obj = hit.collider.gameObject;
            if(obj.CompareTag("Container"))
            {
                HeldObj = obj;
                HeldObj.transform.parent = Spreader.transform;
                HeldObj.AddComponent<Rigidbody>();
                HeldObj.GetComponent<Rigidbody>().isKinematic = true;
                HeldObj.transform.rotation = Spreader.transform.rotation * Quaternion.Euler(0, 90, 0);
                HeldObj.transform.position = Spreader.transform.position - Spreader.transform.up * SpreaderUpOffset;
            }
        }

        ResetSpreader();
    }

    private void Drop()
    {
        if(HeldObj != null)
        {
            HeldObj.transform.parent = null;
            HeldObj.GetComponent<Rigidbody>().isKinematic = false;
            HeldObj = null;
        }
        ResetSpreader();
    }

    public void SetStorageYard(GameObject ship)
    {
        Debug.Log("Ship is set");
        Ship = ship;
        if (Ship == null)
            Debug.Log("Ship is null");
        StorageYard = ship.transform.GetChild(0).GetComponent<ContainerYardScript>();
    }

    public void OnTruckArrival(bool pickup, Vector3 pos)
    {
        Debug.Log("Truck arrived at panamax crane");
        TruckQueue.Enqueue((pickup, pos));
        Debug.Log($"pickup: {pickup}");
    }

    void Start()
    {
        // Initialize the navigation planes.
        UpRight = new Plane(transform.up + transform.position, transform.right + transform.position, transform.position);
        UpForward = new Plane(Trolley.transform.up + Trolley.transform.position, Trolley.transform.forward + Trolley.transform.position, Trolley.transform.position);
        ForwardRight = new Plane(Spreader.transform.forward + Spreader.transform.position, Spreader.transform.right + Spreader.transform.position, Spreader.transform.position);

        TruckQueue = new Queue<(bool, Vector3)>();

        InitialSpreaderPos = Spreader.transform.position.y;
    }

    void Update()
    {
        if(CurState != State.Idle)
        {
            MoveTowardsTarget();
        }
        else if(Ship != null)
        {
            if (TruckQueue.Count > 0)
            {
                var queueItem = TruckQueue.Dequeue();

                if (IsPickup = queueItem.Item1)
                {
                    if (!StorageYard.PeekPlace)
                    {
                        TruckQueue.Enqueue(queueItem);
                        Ship.SendMessage("OperationFinished");
                        CurState = State.Idle;
                        Ship = null;
                        StorageYard = null;
                        Debug.Log("Ship is null");
                        return;
                    }

                    CurState = State.Taking;
                    SetTarget(queueItem.Item2);
                }
                else
                {
                    if (!StorageYard.PeekTake)
                    {
                        TruckQueue.Enqueue(queueItem);
                        Ship.SendMessage("OperationFinished");
                        CurState = State.Idle;
                        Ship = null;
                        StorageYard = null;
                        Debug.Log("Ship is null");
                        return;
                    }
                    TruckTarget = queueItem.Item2;
                    CurState = State.Taking;
                    AskTake();
                }
            }
        }
    }
}
