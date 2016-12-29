using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ResourseType
{
    MAX_FUEL = 0,
    MAX_MASS,
    MAX_TECH,
    MAX_LEVEL,
    SUCK_RANGE,
    AUTO_RANGE,
    OBSERVE_MUL,
    SUCK_MUL,
    ENGINE_POWER,
};

[System.Serializable]
public class UpgradeInfo
{
    public string name;
    public string description;

    public double costFuel;
    public double costMass;
    public double costTech;

    public double requireTech;
    public double requireMass;

    //this upgrade can that multiply 'type' resource 'mul' times.
    public double mul;

    public ResourseType type;

    //delegate void UpgradeFunc();
    //UpgradeFunc callback;
    
}

public class ShopManager : MonoBehaviour {

    public GameObject playerShopScrollview;
    public GameObject pilotShopScrollview;
    public GameObject shops;

    [SerializeField]
    private StatManager _statmanager;

    [SerializeField]
    private Sucker sucker;

    [SerializeField]
    private GameObject upgradeMenu;

    public List<UpgradeInfo> playerShopUpgrades;
    public List<UpgradeInfo> pilotShopUpgrades;

    void ApplyUpgrade(UpgradeInfo upInfo)
    {
        if( _statmanager.CurrentTechPoint > upInfo.requireTech && 
            _statmanager.CurrentMassPoint > upInfo.requireMass &&
            _statmanager.CurrentFuelPoint > upInfo.costFuel 
            )
        {
            _statmanager.ConsumeFuel(upInfo.costFuel);
            _statmanager.ConsumeTech(upInfo.costTech);
            _statmanager.ConsumeMass(upInfo.costMass);

            if(upInfo.type == ResourseType.MAX_FUEL)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
                _statmanager._maxFuelAmout *= upInfo.mul;
            }else if (upInfo.type == ResourseType.MAX_MASS)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
                //
            }
            else if (upInfo.type == ResourseType.MAX_TECH)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
                //
            }
            else if (upInfo.type == ResourseType.MAX_LEVEL)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
                _statmanager.PlusMaxScaleStep((int)upInfo.mul);
            }
            else if (upInfo.type == ResourseType.SUCK_RANGE)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
                sucker.AddUpgrade(upInfo.name, upInfo.mul);
            }
            else if (upInfo.type == ResourseType.AUTO_RANGE)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
            }
            else if (upInfo.type == ResourseType.OBSERVE_MUL)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
            }
            else if (upInfo.type == ResourseType.SUCK_MUL)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
            }
            else if (upInfo.type == ResourseType.ENGINE_POWER)
            {
                Debug.Log("Upgrade! : " + upInfo.name + " mul:" + upInfo.mul);
            }
            

        }
    }

    void Start()
    {
        Transform ts = shops.transform;

        foreach(Transform t in ts)
        {
            Transform scrollview = t.GetChild(1);
            scrollview.gameObject.SetActive(false);
        }

        float yPos = 0;
        foreach(UpgradeInfo upgrade in playerShopUpgrades)
        {
            MakeMenu(playerShopScrollview, upgrade, yPos);
            yPos += -300;
        }

        yPos = 0;
        foreach(UpgradeInfo upgrade in pilotShopUpgrades)
        {
            MakeMenu(pilotShopScrollview, upgrade, yPos);
            yPos += -300;
        }
    }

    void MakeMenu(GameObject scrollview, UpgradeInfo upgrade, float intervalY)
    {
        Debug.Log("======== + " + scrollview);
        var viewport = scrollview.transform.GetChild(0);
        var content = viewport.transform.GetChild(0);

        GameObject menu = Instantiate(upgradeMenu);
        menu.transform.SetParent(content, false);
        Vector3 pos = menu.transform.localPosition;
        pos.y += intervalY;
        menu.transform.localPosition = pos;
        
        Button b = menu.transform.GetChild(0).GetComponent<Button>();
        b.onClick.AddListener(() => { ApplyUpgrade(upgrade); });

        Text t = b.transform.GetChild(0).gameObject.GetComponent<Text>();
        t.text = upgrade.name;

        Text desc = menu.transform.GetChild(1).gameObject.GetComponent<Text>();
        desc.text = upgrade.description;
    }
    
    public void ToggleMenu(GameObject obj)
    {   
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
