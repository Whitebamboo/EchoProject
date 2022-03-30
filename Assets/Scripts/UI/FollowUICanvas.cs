using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUICanvas : UIScreenBase
{
    [SerializeField] ClothPopup clothPopup;

    public ClothPopup GenerateClothPopup(Transform attached)
    {
        ClothPopup _instance = Instantiate(clothPopup, transform);

        _instance.GetComponent<UI_Follow3D>().cam_3d = Camera.main;
        _instance.GetComponent<UI_Follow3D>().targetTransform = attached;
        _instance.GetComponent<UI_Follow3D>().offset3D = new Vector3(0, 3, 0);

        return _instance;
    }
}
