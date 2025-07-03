using System.Collections.Generic;
using UnityEngine;

public class GantryCraneMovement : MonoBehaviour
{
    public enum State { Placing, Taking, Resetting, Idle }
    public State CurState = State.Idle;
    public State PrevState;

    // Crane parts.
    public GameObject Trolley;
    public GameObject Spreader;
    public GameObject Ropes;

    public ContainerYardScript StorageYard;

    // Movement variables.
    private Vector3 Target;
    private Vector3 TruckTarget;

    // Use plane/target distance to determine correct positions (so rotation of the crane wont matter).
    private Plane UpForward;
    private Plane UpRight;
    private Plane ForwardRight;
    // How fast each part of the crane should move.
    public float speed = 3f;
    public float RopeSpeed =  0.052f;
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

    public ExportStatistics ExportStatistics;

    float SpreaderUpOffset = 3.7f;

    public bool AutoControlActive = true;

	// Set a target for the crane.
    public void SetTarget(Vector3 position)
    {
        Target = position + transform.up * SpreaderUpOffset;
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
        bool crane = Mathf.Abs(CraneDistance = UpForward.GetDistanceToPoint(Target)) > CraneAccuracy;
        bool trolley = Mathf.Abs(TrolleyDistance = UpRight.GetDistanceToPoint(Target)) > TrolleyAccuracy;

        // Crane/trolley movement
        if (crane || trolley)
        {
            if(crane)
            {
                transform.position += CraneDistance > 0f ? speed * Time.deltaTime * transform.right : -speed * Time.deltaTime * transform.right;
                UpForward.Set3Points(transform.up + transform.position, transform.forward + transform.position, transform.position);
            }
            
            if(trolley)
            {
                Trolley.transform.position += TrolleyDistance < 0f ? speed * Time.deltaTime * Trolley.transform.forward : -speed * Time.deltaTime * Trolley.transform.forward;
                UpRight.Set3Points(Trolley.transform.up + Trolley.transform.position, Trolley.transform.right + Trolley.transform.position, Trolley.transform.position);
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
        if (take.HasValue)
        {
            SetTarget(take.Value - transform.up * (SpreaderUpOffset - 0.2f));
        }
        else
        {
            StorageYard.transform.parent.gameObject.SendMessage("OperationFinished");
            CurState = State.Idle;
        }
    }

    private void AskPlace()
    {
        if(IsPickup)
        {
            var place = StorageYard.AskPlace();
            if (place.HasValue)
            {
                SetTarget(place.Value);

                if (HeldObj != null)
                {
                    ExportStatistics.GetTimeTable().Add(
                        HeldObj.GetInstanceID(), 
                        Time.time);
                }
            }
            else
            {
                CurState = State.Idle;
            }
        }
        else
        {
            SetTarget(TruckTarget);
            ExportStatistics.AddToStruct(HeldObj.GetInstanceID());
            ExportStatistics.GetTimeTable().Remove(HeldObj.GetInstanceID());    
        }
    }

    private void ResetSpreader()
    {
        Target = new Vector3(Target.x, InitialSpreaderPos, Target.z);
    }

    private void Pickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Spreader.transform.position, -Spreader.transform.up, out hit, 1.5f))
        {
            var obj = hit.collider.gameObject;
            if(obj.CompareTag("Container"))
            {
                HeldObj = obj;
                HeldObj.transform.parent = Spreader.transform;
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

    public void OnTruckArrival(bool pickup, Vector3 pos)
    {
        TruckQueue.Enqueue((pickup, pos));
    }

    void Start()
    {
        // Initialize the navigation planes.
        UpForward = new Plane(transform.up + transform.position, transform.right + transform.position, transform.position);
        UpRight = new Plane(Trolley.transform.up + Trolley.transform.position, Trolley.transform.forward + Trolley.transform.position, Trolley.transform.position);
        ForwardRight = new Plane(Spreader.transform.forward + Spreader.transform.position, Spreader.transform.right + Spreader.transform.position, Spreader.transform.position);

        TruckQueue = new Queue<(bool, Vector3)>();

        InitialSpreaderPos = Spreader.transform.position.y;

        ExportStatistics = GameObject.FindGameObjectWithTag("PauseCanvas").transform.GetChild(2).gameObject.transform.GetChild(5).gameObject.GetComponent<ExportStatistics>();
    }

    void Update()
    {
        if (!AutoControlActive)
        {
            return;
        }

        if(CurState != State.Idle)
        {
            MoveTowardsTarget();
        }
        else
        {
            if(TruckQueue.Count > 0)
            {
                var queueItem = TruckQueue.Dequeue();

                if(IsPickup = queueItem.Item1)
                {
                    if (!StorageYard.PeekPlace)
                    {
                        TruckQueue.Enqueue(queueItem);
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
