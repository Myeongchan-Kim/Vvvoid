using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class StatManager : MonoBehaviour {

    public const double DEFALT_SPEED = 1.0;
    public const double DEFALT_MASS = 10;
    public double currentScaleStep = 0.0;
    double _velocity;
    double _distance;
    double _fuelAmout;
    public double _maxFuelAmout;
    double _mass;
    double _fuelConsumtionForEachTouch = 1.0;

    Text _distanceUI = null;
    Text _velocityUI = null;
    Text _fuelUI = null;

    void Start () {
        _velocity = DEFALT_SPEED;
        _distance = 0.0;
        _fuelAmout = _maxFuelAmout * 0.5;
        _mass = DEFALT_MASS;

        _distanceUI = GameObject.Find("DistanceText").GetComponent<Text>();
        _velocityUI = GameObject.Find("VelocityText").GetComponent<Text>();
        _fuelUI = GameObject.Find("FuelText").GetComponent<Text>();
	}
	
	void Update () {
        _distance += (double)Time.deltaTime * _velocity;

        // Use String.Builder!!!!!
        // Like this.
        // _distanceUI.text = String.Format("Dist : {0:N3} m", _distance);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Length = 0;
        sb.AppendFormat("Dist : {0:N3} m", _distance);
        _distanceUI.text = sb.ToString();
        sb.Length = 0;
        // ...

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
    
}
