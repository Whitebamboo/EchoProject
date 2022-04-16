using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float castRadius = 1f;

    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] Transform itemRoot;
    [SerializeField] GameObject[] models;
    [SerializeField] GameObject hintArrow;
      
    CharacterController controller;
    Animator animator;
    Transform camTransform;

    bool isMoving;
    Vector3 movement;
    Vector3 camForward;

    int deviceId;
    int playerId;

    float horizontal; //Horizontal Input
    float vertical; //Vertical Input

    Item currItem;

    IIteractable currInteractable;

    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;

        controller = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;
    }

    public void SetupPlayerData(int playerId)
    {
        this.playerId = playerId;
        this.deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(playerId);

        GameObject modelView = Instantiate(models[playerId], transform);
        modelView.transform.localPosition = new Vector3(0, -0.88f, 0);
        animator = modelView.GetComponent<Animator>();
    }

    public void SetItem(Item item)
    {
        FantasyIngameCanvas canvas = UIManager.instance.FindScreen<FantasyIngameCanvas>();
        if (item == null)
        {
            currItem = null;
            canvas.DisableClothDesc(playerId);
            return;
        }

        currItem = item;
        item.transform.parent = itemRoot;
        item.transform.localPosition = Vector3.zero;
        item.transform.localEulerAngles = Vector3.zero;
        item.transform.localScale = Vector3.one;

        canvas.ShowClothDesc(playerId, item.data);
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        if(fromDeviceID != deviceId)
        {
            return;
        }

        if (data["joystick_left"] != null)
        {
            if (data["joystick_left"]["position"] != null)
            {
                isMoving = true;
                animator.SetBool("Walking", true);

                horizontal = float.Parse(data["joystick_left"]["position"]["x"].ToString());
                vertical = float.Parse(data["joystick_left"]["position"]["y"].ToString()) * -1;
            }

            if (data["joystick_left"]["touch"] != null)
            {
                isMoving = bool.Parse(data["joystick_left"]["touch"].ToString());
                if(!isMoving)
                {
                    movement = Vector3.zero;
                    horizontal = 0;
                    vertical = 0;
                    animator.SetBool("Walking", false);
                }
            }
        }

        if (data["action"] != null && data["action"].ToString().Equals("interact"))
        {
            Interact();
        }

        if (data["action"] != null && data["action"].ToString().Equals("check"))
        {
            Check();
        }
    }

    /**
     *  return the nearest interactive object 
     */
    IIteractable GetNearestObject()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, castRadius, 1 << 6);

        if(hit == null || hit.Length == 0)
        {
            return null;
        }

        IIteractable closeHit = null;
        float minDistance = int.MaxValue;
        foreach(Collider obj in hit)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closeHit = obj.gameObject.GetComponent<IIteractable>();
            }
        }
        return closeHit;
    }

    void Interact()
    {
        IIteractable currInteractable = GetNearestObject();

        if (currInteractable != null)
        {
            Debug.Log("get something");
            currInteractable.OnInteract(this, currItem);
        }
    }

    void Check()
    {
        IIteractable currInteractable = GetNearestObject();

        if (currInteractable != null)
        {
            ItemHolder holder = null;
            if (currInteractable is ItemHolder)
            {
                holder = (ItemHolder)currInteractable;
            }
            else
            {
                return;
            }

            if(holder.GetItem() == null)
            {
                return;
            }

            AirConsole.instance.Message(deviceId,
                string.Format("Fantasy;Cloth;{0};{1}", holder.GetItem().data.clothImage.name.Replace(" ",""), holder.GetItem().data.ToString()));
        }
    }

    void Update()
    {
        movement = camTransform.right * horizontal * playerSpeed * Time.deltaTime + camForward * vertical * playerSpeed * Time.deltaTime;
        controller.Move(movement);

        if(horizontal != 0 || vertical != 0)
        {
            Rotating(horizontal, vertical);
        }

        if (isMoving)
        {
            SearchingAround();
        }
    }

    void SearchingAround()
    {
        IIteractable obj = GetNearestObject();

        if(currInteractable != obj)
        {
            if(currInteractable != null)
            {
                currInteractable.OnHideHint(hintArrow);
            }

            currInteractable = obj;

            if (currInteractable != null)
            {
                currInteractable.OnShowHint(hintArrow);
            }
        }
    }

    void Rotating(float h, float v)
    {
        camForward = Vector3.Cross(camTransform.right, Vector3.up);

        Vector3 targetDir = camTransform.right * h + camForward * v;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void OnDestroy()
    {
        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, castRadius);
    }
}
