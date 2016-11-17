using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class StatManager : MonoBehaviour {

    public const double DEFALT_SPEED = 1.0;
    public const double DEFALT_MASS = 100;
    public double currentScale = 1.0;
    double _velocity;
    double _distance;
    double _fuelAmout;
    double _maxFuelAmout;
    double _mass;

    Text distanceUI = null;
    Text velocityUI = null;

    void Start () {
        _velocity = DEFALT_SPEED;
        _distance = 0.0;

        distanceUI = GameObject.Find("DistanceText").GetComponent<Text>();
        velocityUI = GameObject.Find("VelocityText").GetComponent<Text>();    
	}
	
	void Update () {
        _distance += (double)Time.deltaTime * _velocity;
        distanceUI.text = String.Format("Dist : {0:N3} m", _distance);
        velocityUI.text = String.Format("Speed : {0:N3} m/s", _velocity);
        
    }

    public float GetScrollSpeed() { return (float)(_velocity / Math.Pow(2.0, currentScale)); }

    //clicker game has to have stat by double
    public double GetRealSpeed() { return _velocity; }

    public double AccelCharacter(double energy)
    {
        double cur_energy = 0.5 * _velocity * _velocity;
        cur_energy += energy;
        _velocity = Math.Sqrt(cur_energy * 2);

        //it return result Velocity
        return GetRealSpeed();
    }

}
