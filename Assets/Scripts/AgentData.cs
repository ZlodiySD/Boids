using System;
using UnityEngine;

[Serializable]
public class AgentData
{
  public float Speed;
  public float Radius;
  public float RotationSpeed;
  public float MaxVelocity;
  public float SeparationRadius;
  public float SeparationForce;
  public float AlignmentForce;
  public float CohesionForce;
  public float ViewAngle;
  public float InterpolationFramesCount;
}