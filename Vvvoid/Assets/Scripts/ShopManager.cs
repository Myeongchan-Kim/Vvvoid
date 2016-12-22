using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour {

    //public GameObject PlayerShopScrollview;
    //public GameObject PilotShopScrollview;
    //public GameObject WarmHolerShopScrollview;
    public GameObject shops;

    void Start()
    {
        Transform ts = shops.transform;

        foreach(Transform t in ts)
        {
            Transform scrollview = t.GetChild(1);
            scrollview.gameObject.SetActive(false);
        }
    }
    
    public void ToggleMenu(GameObject obj)
    {
        Debug.Log("======== Toggle SHop");
        toggleActive(obj);
    }

    void toggleActive(GameObject obj)
    {
        if (obj.activeSelf)
            obj.SetActive(false);
        else
            obj.SetActive(true);
    }
}
