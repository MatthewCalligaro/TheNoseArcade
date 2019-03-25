using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float acceleration = 10;
    private const float decayAcceleration = 2;
    private const float brakeAcceleration = 30;
    private const float turnSpeed = 180;

    private float velocity = 0;

    private bool drifting = false;

	// Use this for initialization
	void Start ()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (!this.drifting)
        {
            this.transform.position += this.transform.forward * this.velocity * Time.deltaTime;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                this.velocity += acceleration * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                this.velocity = Mathf.Max(0, this.velocity - brakeAcceleration * Time.deltaTime);
            }
            else
            {
                this.velocity = Mathf.Max(0, this.velocity - decayAcceleration * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
        }

        Debug.Log(this.velocity);
    }
}
