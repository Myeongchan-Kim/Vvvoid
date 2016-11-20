using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class StatManager : MonoBehaviour {

    public const double DEFALT_SPEED = 1.0;
    public const double DEFALT_MASS = 10;
    public double currentScaleStep = 0.0;
    double _velocity;
    public double distance { get; private set; }
    double _fuelAmout;
    public double _maxFuelAmout;
    double _mass;
    double _fuelConsumtionForEachTouch = 10.0;
    int resource;

    Text _distanceUI = null;
    Text _velocityUI = null;
    Text _fuelUI = null;

    void Start () {
        _velocity = DEFALT_SPEED;
        distance = 0.0;
        _fuelAmout = _maxFuelAmout * 0.5;
        _mass = DEFALT_MASS;

        _distanceUI = GameObject.Find("DistanceText").GetComponent<Text>();
        _velocityUI = GameObject.Find("VelocityText").GetComponent<Text>();
        _fuelUI = GameObject.Find("FuelText").GetComponent<Text>();
	}
	
	void Update () {
        distance += (double)Time.deltaTime * _velocity;
        _distanceUI.text = String.Format("Dist : {0:N3} m", distance);
        _velocityUI.text = String.Format("Speed : {0:N3} m/s", _velocity);
        _fuelUI.text = String.Format("Fuel : {0:N1} / {1:N1} ", _fuelAmout, _maxFuelAmout);
        
        if(Input.GetButtonDown("Fire1"))
        {
            double consumedEnergy = AccelCharacter(_fuelConsumtionForEachTouch);
            if (consumedEnergy > 0.0)
            {
                AnimateAccel(consumedEnergy);
            }
        }
    }

    IEnumerator AnimateAccel(double consumeAmount)
    {
        yield return null;
    }

    public float GetScrollSpeed() { return (float)(_velocity / Math.Pow(2.0, currentScaleStep)); }

    //clicker game has to have stat by double
    public double GetRealSpeed() { return _velocity; }

    public double AccelCharacter(double energy)
    {
        if (_fuelAmout - energy > 0)
        {
            _fuelAmout -= energy;
        }
        else
        {
            _fuelAmout = 0;
            energy = _fuelAmout;
        }

        _velocity = AddSpeed(_velocity, _mass, energy);
        //it return consumed energy
        return energy;
    }

    double AddSpeed(double curVelocity , double targetMass, double energyToAdd)
    {
        double cur_energy = 0.5 * curVelocity * curVelocity * targetMass;
        cur_energy += energyToAdd;
        curVelocity = Math.Sqrt(cur_energy * 2 / targetMass);

        return curVelocity;
    }

    public double GetFuel(double getAmout)
    {
        if (getAmout + _fuelAmout < _maxFuelAmout)
            _fuelAmout += getAmout;
        else
            _fuelAmout = _maxFuelAmout;

        return _fuelAmout;
    }

    public void AddResource(int resource)
    {
        this.resource += resource;
    }

    public int GetResource()
    {
        return resource;
    }
    
}
