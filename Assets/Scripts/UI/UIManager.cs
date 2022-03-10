using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : CSingletonMono<UIManager>
{
    Dictionary<string, UIScreenBase> _dicScreens = new Dictionary<string, UIScreenBase>();

    public int GetScreenNum()
    {
        return _dicScreens.Count;
    }

    public Dictionary<string, UIScreenBase> GetScreens()
    {
        return _dicScreens;
    }

    public void Destroy()
    {
        _dicScreens.Clear();
    }

    public T CreateScreen<T>() where T : UIScreenBase
    {
        string key = typeof(T).ToString();
        if (_dicScreens.ContainsKey(key))
        {
            if (((T)_dicScreens[key]).gameObject.activeSelf == false)
                ((T)_dicScreens[key]).gameObject.SetActive(true);
            return (T)_dicScreens[key];
        }

        GameObject prefab = GetPrefabFromType(typeof(T));

        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab);
#if UNITY_EDITOR
            obj.name = key;
#endif
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            T result = obj.GetComponent<T>();

            result.OnCloseAndDestroy += RemoveScreen;

            _dicScreens.Add(key, result);
            return result;
        }

        return null;
    }

    void RemoveScreen(UIScreenBase _handler)
    {
        // Debug.LogError("in remove");
        System.Type type = _handler.GetType();
        string key = type.ToString();
        if (_dicScreens.ContainsKey(key))
        {
            _dicScreens.Remove(key);
        }
        Destroy(_handler.gameObject);
    }

    public T FindScreen<T>() where T : UIScreenBase
    {
        string key = typeof(T).ToString();
        if (_dicScreens.ContainsKey(key))
        {
            return (T)_dicScreens[key];
        }

        return null;
    }

    GameObject GetPrefabFromType(System.Type _type)
    {
        return Resources.Load(_type.ToString()) as GameObject;
    }
}
