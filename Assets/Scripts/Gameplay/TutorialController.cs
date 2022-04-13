using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 1f;

    bool isMoving;
    Vector2 movement;

    int deviceId;
    int playerId;

    float horizontal; //Horizontal Input

    RectTransform m_display;

    bool isComfirmed;

    Action onConfirmed;

    Vector2 initPos;

    float minPosition;
    float maxPosition;

    public float GetXpos() => m_display.anchoredPosition.x;

    public int GetDeviceId() => deviceId;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;

        minPosition = 0;
        maxPosition = transform.parent.GetComponent<RectTransform>().rect.width;
    }

    void Update()
    {
        if (horizontal != 0 && !isComfirmed)
        {
            movement = new Vector2(horizontal, 0);
            m_display.anchoredPosition += movement;
            float x = Mathf.Clamp(m_display.anchoredPosition.x, minPosition, maxPosition);
            m_display.anchoredPosition = new Vector2(x, m_display.anchoredPosition.y);
        }
    }

    public void SetupPlayer(int playerId, RectTransform displayIcon, Action confirmAction)
    {
        this.playerId = playerId;
        this.deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(playerId);

        m_display = displayIcon;
        onConfirmed = confirmAction;
        initPos = displayIcon.anchoredPosition;
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        if (fromDeviceID != deviceId)
        {
            return;
        }

        if (data["joystick_left"] != null)
        {
            if (data["joystick_left"]["position"] != null)
            {
                isMoving = true;

                horizontal = float.Parse(data["joystick_left"]["position"]["x"].ToString());
            }

            if (data["joystick_left"]["touch"] != null)
            {
                isMoving = bool.Parse(data["joystick_left"]["touch"].ToString());
                if (!isMoving)
                {
                    horizontal = 0;
                }
            }
        }

        if (data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            TutorialCanvas canvas = UIManager.instance.FindScreen<TutorialCanvas>();
            if(canvas != null && canvas.State == TutorialState.Play)
            {
                isComfirmed = !isComfirmed;
                m_display.transform.Find("Confirm").gameObject.SetActive(isComfirmed);
                onConfirmed.Invoke();
            }        
        }
    }

    public void ResetPlayer()
    {
        isComfirmed = false;
        m_display.transform.Find("Confirm").gameObject.SetActive(false);

        m_display.anchoredPosition = initPos;
    }

    private void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
