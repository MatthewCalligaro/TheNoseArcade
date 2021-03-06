﻿using System;
using UnityEngine;

/// <summary>
/// Controls the player and player score
/// </summary>
public class Player : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////
    // Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Property to set a new multiplier to the player's speed
    /// </summary>
    public float SpeedMultiplier
    {
        get
        {
            return this.speedMultiplier;
        }
        set
        {
            this.speedMultiplier = value;
            this.speedCounter = speedMultiplierTime;
        }
    }



    ////////////////////////////////////////////////////////////////
    // Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Number of UI elements currently preventing pause
    /// </summary>
    public static int BlockingPause = 0;

    /// <summary>
    /// Upward velocity after jumping for JumpStyle.Velocity
    /// </summary>
    private const float jumpVelocity = 12;

    /// <summary>
    /// Upward force applied from a jump for JumpStyle.Force
    /// </summary>
    private const float jumpForce = 650;

    /// <summary>
    /// Upward force applied per second for JumpStyle.Jetpack
    /// </summary>
    private const float jumpJetpackForce = 2000;

    /// <summary>
    /// Time for which a speed multiplier lasts
    /// </summary>
    private const float speedMultiplierTime = 1.5f;

    /// <summary>
    /// The maximum percentage of the screen width that the player can get ahead
    /// </summary>
    private const float maxScreenX = 0.75f;

    /// <summary>
    /// Gravity scale of player's Rigidbody2D when active
    /// </summary>
    private const float gravityScale = 1.2f;

    /// <summary>
    /// Current player score
    /// </summary>
    private int score = 0;

    /// <summary>
    /// True if the player is receiving an upward force from jumping with JumpStyle.Jetpack
    /// </summary>
    private bool isJetpacking = false;

    /// <summary>
    /// Current multiplier applied to the player's speed
    /// </summary>
    private float speedMultiplier = 1.0f;

    /// <summary>
    /// Time remaining for the current speed multiplier
    /// </summary>
    private float speedCounter = 0;

    /// <summary>
    /// The player's velocity just before pausing
    /// </summary>
    private Vector2 lastVelocity;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles when the player recieves the command to jump
    /// </summary>
    /// <param name="magnitude">Additional multiplier to apply to the jump's force</param>
    public virtual void JumpEnter(float magnitude = 1.0f)
    {
        switch (Settings.JumpStyle)
        {
            case JumpStyle.Velocity:
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, jumpVelocity * Settings.JumpPower * magnitude);
                break;

            case JumpStyle.Force:
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce * Settings.JumpPower * magnitude));
                break;

            // Jetpack does not use magnitude
            case JumpStyle.Jetpack:
                this.isJetpacking = true;
                break;
        }
    }

    /// <summary>
    /// Handles when the player stops recieving the command to jump
    /// </summary>
    public void JumpExit()
    {
        this.isJetpacking = false;
    }

    /// <summary>
    /// Handles when the player recieves the command to pause the game
    /// </summary>
    public virtual void Pause()
    {
        if (BlockingPause <= 0)
        {
            PauseMenu.TogglePauseMenu();
            if (PauseMenu.Paused)
            {
                this.lastVelocity = this.GetComponent<Rigidbody2D>().velocity;
            }
        }
    }

    /// <summary>
    /// Applies a force vector to the player
    /// </summary>
    /// <param name="force">The force vector to apply to the player</param>
    public void ApplyForce(Vector2 force)
    {
        this.GetComponent<Rigidbody2D>().AddForce(force);
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Start()
    {
        BlockingPause = 0;
        HUD.UpdateScore(this.score);
        HUD.UpdateDistance((int)this.transform.position.x);
        Controller.SetPlayer(this);
    }

    protected virtual void Update()
    {
        // Use escape to pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.Pause();
        }

        if (Menu.InPlay)
        {
            // Remove the speedMultiplier when the timer expires
            if (this.speedCounter > 0)
            {
                this.speedCounter -= Time.deltaTime;
                if (this.speedCounter <= 0)
                {
                    this.speedMultiplier = 1.0f;
                    this.speedCounter = 0;
                }
            }

            // Use space bar to jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.JumpEnter();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                this.JumpExit();
            }

            // The player loses if they fall outside of the camera
            if (!God.InCamera(this.transform.position))
            {
                LossMenu.HandleLoss(this.score);
                Scoreboard.AddScore(new HighScore { Score = this.score, Distance = (int)this.transform.position.x, Date = DateTime.Now, Difficulty = Settings.Difficulty });
            }

            // Tell the camera to skip ahead if the player surpasses maxScreenX
            if (God.CameraCoords(this.transform.position).x > maxScreenX)
            {
                God.Skip(1 / (1 - God.CameraCoords(this.transform.position).x));
            }

            // Apply upward jetpack force if the player is currently jetpacking
            if (this.isJetpacking)
            {
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpJetpackForce * Time.deltaTime * Settings.JumpPower));
            }

            // Move forward with the current speed multiplier and update the HUD distance
            int lastX = (int)this.transform.position.x;
            this.transform.Translate(new Vector3(God.ScrollSpeed * this.SpeedMultiplier * Time.deltaTime, 0, 0));
            if ((int)this.transform.position.x > lastX)
            {
                HUD.UpdateDistance((int)this.transform.position.x);
            }
        }

        // Set the player to static when not in play and dynamic when in play
        if (this.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static && Menu.InPlay)
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            // Upon exiting pause, restore the previous velocity
            this.GetComponent<Rigidbody2D>().velocity = this.lastVelocity;
        }
        else if (this.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && !Menu.InPlay)
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.HandleCollision(collision.gameObject);
    }



    ////////////////////////////////////////////////////////////////
    // Protected and Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles collisions with any type of game object
    /// </summary>
    /// <param name="other">The game object with which the player collided</param>
    protected virtual void HandleCollision(GameObject other)
    {
        // If other is a consumable object, apply its relevant stats and remove it
        if (other.gameObject.GetComponent<Environment>() && other.gameObject.GetComponent<Environment>().EnvironmentType == EnvironmentType.Consumable)
        {
            Environment otherObs = other.gameObject.GetComponent<Environment>();
            this.score += otherObs.Score;
            HUD.UpdateScore(this.score);
            this.GetComponent<Rigidbody2D>().AddForce(otherObs.Force);
            this.SpeedMultiplier *= otherObs.SpeedMultiplier;
            God.RemoveEnvironmentObj(other.gameObject);
        }
    }
}
