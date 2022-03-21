using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] Transform itemRoot;
    
    CharacterController controller;
    Transform camTransform;

    bool isMoving;
    Vector3 movement;
    Vector3 camForward;

    int deviceId;
    int playerId;

    float horizontal; //Horizontal Input
    float vertical; //Vertical Input

    IIteractable currInteractable;
    Item currItem;

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
    }

    public void SetItem(Item item)
    {
        if(item == null)
        {
            currItem = null;
            return;
        }

        currItem = item;
        item.transform.parent = itemRoot;
        item.transform.localPosition = Vector3.zero;
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

    void Interact()
    {
        if(currInteractable != null)
        {
            currInteractable.OnInteract(this, currItem);
        }
    }

    void Check()
    {
        if (currInteractable != null)
        {
            ItemHolder holder;
            if (currInteractable is ItemHolder)
            {
                holder = (ItemHolder)currInteractable;
            }
            else
            {
                return;
            }

            AirConsole.instance.Message(deviceId,
                string.Format("Fantasy;Cloth;{0};{1}", holder.GetItem().data.clothImage.name, holder.GetItem().data.description));
        }
    }

    void Update()
    {
        movement = camTransform.right * horizontal *playerSpeed * Time.deltaTime + camForward * vertical * playerSpeed * Time.deltaTime;
        controller.Move(movement);

        if(horizontal != 0 || vertical != 0)
        {
            Rotating(horizontal, vertical);
        }
    }

    void Rotating(float h, float v)
    {
        camForward = Vector3.Cross(camTransform.right, Vector3.up);

        Vector3 targetDir = camTransform.right * h + camForward * v;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        IIteractable interactable = collision.gameObject.GetComponent<IIteractable>();

        if (interactable != null)
        {
            currInteractable = interactable;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        currInteractable = null;
    }

    void OnDestroy()
    {
        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
