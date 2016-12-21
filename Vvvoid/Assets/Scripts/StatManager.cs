using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Com.Google.Android.Gms.Games;

public class StatManager : MonoBehaviour {

    public const double DEFALT_SPEED = 1.0;
    public const double DEFALT_MASS = 10;
    public double currentScaleStep
    {
        get
        {
            return _currentScaleStep;
        }
    }
    public double maxScaleStep
    {
        get
        {
            return _maxScaleStep;
        }
    }
    public double currentTechPoint
    {
        get
        {
            return _techPoint;
        }
    }

    double _techPoint = 0.0f;
    double _currentScaleStep = 1.0f;
    double _maxScaleStep = 10.0f;
    double _minScaleStep = 0.0f;
    double _velocity;
    public double distance { get; private set; }
    double _fuelAmout;
    public double _maxFuelAmout;
    double _mass;
    double _fuelConsumtionForEachTouch = 10.0;
    double food;

    public Player player = null;

    public Text _distanceUI = null;
    public Text _velocityUI = null;
    public Text _fuelUI = null;

    void Start () {
        
        _velocity = DEFALT_SPEED;
        distance = 0.0;
        _fuelAmout = _maxFuelAmout * 0.5;
        _mass = DEFALT_MASS;

        Social.ReportScore((long)distance, "CgkI5YeLpasSEAIQAg", (bool sucess) => { if (sucess) Debug.Log("Score Update Success"); else Debug.Log("Scored Update Fail"); });
    }

    string SetText(ref Text target, ref double targetValue, String formatStr)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Length = 0;
        sb.AppendFormat(formatStr, targetValue);
        target.text = sb.ToString();
        sb.Length = 0;
        return sb.ToString();
    }

    void Update () {

        distance += (double)Time.deltaTime * _velocity;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Length = 0;

        sb.AppendFormat("Dist : {0:N3} m", distance);
        _distanceUI.text = sb.ToString();
        sb.Length = 0;

        sb.AppendFormat("Speed : {0:N3} m/s", _velocity);
        _velocityUI.text = sb.ToString();
        sb.Length = 0;

        sb.AppendFormat("Fuel : {0:N1} / {1:N1} ", _fuelAmout, _maxFuelAmout);
        _fuelUI.text = sb.ToString();
        sb.Length = 0;

        if (Input.GetButtonDown("Fire1"))
        {
            double consumedEnergy = AccelCharacter(_fuelConsumtionForEachTouch);
            if (consumedEnergy > 0.0)
            {
                AnimateAccel(consumedEnergy);
            }else
            {
                AnimateNoFuel();
            }
        }
    }

    void AnimateNoFuel()
    {
        player.NoFuelEffect();
        return;
    }

    void AnimateAccel(double consumeAmount)
    {
        player.FireRocketEffect();
        return;
    }

    public float GetScrollSpeed() { return (float)(_velocity / Math.Pow(2.0, _currentScaleStep)); }

    //clicker game has to have stat by double
    public double GetRealSpeed() { return _velocity; }
    
    public void SetScaleStep(int targetStep)
    {
        _currentScaleStep = targetStep;
    }

    public double ZoomInOutByStep(double step) //times == 10 means 10 times. times == 0.1 means 10% zoom out
    {
        _currentScaleStep += step;
        if (_currentScaleStep > _maxScaleStep)
            _currentScaleStep = _maxScaleStep;
        if (_currentScaleStep < _minScaleStep)
            _currentScaleStep = _minScaleStep;

        Debug.Log("Scroll Speed : " + GetScrollSpeed());
        Debug.Log("Current Step : " + _currentScaleStep);

        return _currentScaleStep;
    }

    public void ZoomInOutRealScale(double times)
    {
        _currentScaleStep += Math.Log(times, 2);
    }

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

    public double AddFuel(double getAmout)
    {
        Debug.Log("Add Fuel");
        if (getAmout + _fuelAmout < _maxFuelAmout)
            _fuelAmout += getAmout;
        else
            _fuelAmout = _maxFuelAmout;

        return _fuelAmout;
    }

    public double AddTech(double getTechAmount)
    {
        _techPoint += getTechAmount;
        return _techPoint;
    }

    public double AddMass(double getMassAmount)
    {
        _mass += getMassAmount;
        return _mass;
    } 

}
