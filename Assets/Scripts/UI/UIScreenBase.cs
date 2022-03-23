using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenBase : MonoBehaviour
{
    protected bool m_Initialized = false; // Prevent unintentional, double initialization.

    public delegate void OnScreenHandlerEventHandler(UIScreenBase screenHandler);
    public event OnScreenHandlerEventHandler OnCloseAndDestroy;

    // Use this for initialization
    void Awake()
    {
        if (m_Initialized == false)
        {
            Init();
        }
    }

    public virtual void Init()
    {
        m_Initialized = true;
    }

    public virtual void CloseScreen()
    {
        if (OnCloseAndDestroy != null)
            OnCloseAndDestroy(this);
    }
}
