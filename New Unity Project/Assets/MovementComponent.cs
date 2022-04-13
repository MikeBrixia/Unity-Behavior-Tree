using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{

// In meters
public float Speed = 600;
public float JumpHeight = 300;
public float Friction = 0;
private Vector3 Velocity;
private Vector3 Gravity = new Vector3(0,0,-980);

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // Add movement for the owner in a given direction scaled by input
    public void AddMovementInput(Vector3 Direction, float Scale){
       
       Scale = Mathf.Clamp(Scale, 0, 1);
       // Scale movement input(if negative will invert the direction)
       Direction = Direction * Scale;
       
       Velocity = GetVelocityWithFriction(Direction) * Time.deltaTime;
       // Calculate new position
       Vector3 NewPlayerLocation = transform.root.position + Velocity;
       transform.root.position = NewPlayerLocation;
    }
    
    // Make the owner jump on the Y axis
    public void Jump(){
    
    }
    
    private Vector3 GetVelocityWithFriction(Vector3 Direction){
        
        // Make sure that Speed and Friction are not 0
        if(Speed == 0){
            Speed = 1;
        }
        if(Friction == 0){
            Friction = 1;
        }
        
        Vector3 InitialVelocity = Direction * Speed;
        
        // Calculate velocity with friction
        float NewSpeed = Mathf.Sqrt(Mathf.Pow(InitialVelocity.magnitude, 2) - 2 * (Friction * Gravity.y));
        return Direction * NewSpeed;
    }
    
    // Returns the Velocity in Meters per second
    public Vector3 GetVelocity() { return Velocity; }
}
