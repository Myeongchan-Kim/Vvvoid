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

    public GameObject PlayerShopScrollview;
    public GameObject PilotShopScrollview;
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
        

        foreach(UpgradeInfo upgrade in playerShopUpgrades)
        {
            GameObject menu = Instantiate(upgradeMenu);
            var viewport = PlayerShopScrollview.transform.GetChild(0);
            var content = viewport.transform.GetChild(0);
            menu.transform.parent = content;
            menu.SetActive(true);
            
            Button b = menu.transform.GetChild(0).GetComponent<Button>();
            b.onClick.AddListener(delegate () { Debug.Log("=========="); });
        }

        foreach(UpgradeInfo upgrade in pilotShopUpgrades)
        {

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
