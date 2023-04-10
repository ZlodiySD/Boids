using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class BoidBehaviour : MonoBehaviour
{
  public MeshRenderer renderer;
  public Vector3 Velocity { get; private set; }
  public AgentData Data { get => _data; private set => _data = value; }

  public Vector3 PositionThreshold;
  public Vector3 VelocityThreshold;
  
  private AgentData _data;

  private int _maxBoids;
  private Collider[] _flock;

  private int _elapsedFrames = 0;
  
  private BoundHelper _boundHelper;

  private Vector3 _aligment;
  private Vector3 _cohesion;
  private Vector3 _separation;


  public void Init(AgentData data, BoundHelper boundHelper)
  {
    _boundHelper = boundHelper;
    Velocity = transform.forward;
    _data = data;
  }

  public void SetMaxBoids(int maxBoids)
  {
    _maxBoids = maxBoids;
    
    _flock = new Collider[_maxBoids];
  }
  
  
  private void FixedUpdate()
  {
    _aligment = Vector3.zero;
    _cohesion = Vector3.zero;
    _separation = Vector3.zero;
    
    PositionThreshold = transform.position;
    VelocityThreshold = Velocity;
    
    CalculateVelocity();
    //Velocity = Velocity + _aligment + _cohesion + _separation;
    //BoxAvoidance();
    Velocity = Vector3.ClampMagnitude(Velocity, _data.MaxVelocity);
    
    RotateToVelocity();

    if (Velocity.sqrMagnitude == 0)
      Velocity = VelocityThreshold;

    // float interpolationRatio = (float)_elapsedFrames / _data.InterpolationFramesCount;
    // var _velocity = Vector3.Lerp(VelocityThreshold, Velocity, interpolationRatio);
    // _elapsedFrames = (int)((_elapsedFrames + 1) % (_data.InterpolationFramesCount + 1));
    // transform.position += _velocity * _data.Speed * Time.fixedDeltaTime;
    
    //Velocity = Vector3.Lerp(VelocityThreshold, Velocity, 0.01f);
    
    DrawDirection(transform.position, Velocity, Color.green, Velocity.magnitude * 2);
    transform.position += Velocity * _data.Speed * Time.fixedDeltaTime;
  }

  private void RotateToVelocity()
  {
    Vector3 targetDirection = (transform.position - Velocity) - transform.position;
    Vector3 newDirection = Vector3.RotateTowards(transform.forward, -targetDirection, _data.RotationSpeed * Time.fixedDeltaTime, 0.0f);
    
    transform.rotation = Quaternion.LookRotation(newDirection);
  }

  private void CalculateVelocity()
  { 
    int counter = 0;
    int layerMask = 1 << 6;

    int count = Physics.OverlapSphereNonAlloc(transform.position, _data.Radius, _flock, layerMask);
    
    if (count <= 1)
      return;

    Vector3 alignmentAverage = Vector3.zero;
    Vector3 cohesionAverage = Vector3.zero;
    Vector3 separationAverage = Vector3.zero;
    
    for (int i = 0; i < count; i++)
    {
      BoidBehaviour boid = _flock[i].GetComponentInParent<BoidBehaviour>();
      
      if (boid.gameObject == gameObject)
        continue;

      if(!Visible(boid.PositionThreshold))
        continue;
      else
        counter++;

      alignmentAverage += boid.VelocityThreshold;
      cohesionAverage += boid.PositionThreshold;

      Vector3 diff = transform.position - boid.PositionThreshold;
      float magnitude = Vector3.Magnitude(diff);
      if (magnitude == 0)
        magnitude = 0.1f;
      diff *= 1/magnitude;
      separationAverage += diff;
    }
    
    if(counter == 0)
      return;

    SeparationApply(separationAverage, counter);
    CohesionApply(cohesionAverage, counter);
    AlignmentApply(alignmentAverage, counter);
    
    DrawDirection(transform.position, _aligment + _cohesion + _separation, Color.yellow, (_aligment + _cohesion + _separation).magnitude);
  }
  
  private void AlignmentApply(Vector3 alignmentAverage, int counter)
  {
    alignmentAverage /= counter;
    //alignmentAverage *= _data.Speed * 0.2f;
    alignmentAverage -= Velocity;
    Vector3.ClampMagnitude(alignmentAverage, _data.MaxVelocity);
    
    alignmentAverage *= _data.AlignmentForce;
    
    Velocity += alignmentAverage;
    DrawDirection(transform.position,  alignmentAverage, Color.blue, alignmentAverage.magnitude);
    _aligment = alignmentAverage;
  }

  private void CohesionApply(Vector3 cohesionAverage, int counter)
  {
    cohesionAverage /= counter;
    cohesionAverage -= transform.position;
    //cohesionAverage *= _data.Speed * 0.2f;
    cohesionAverage -= Velocity;
    Vector3.ClampMagnitude(cohesionAverage, _data.MaxVelocity);

    cohesionAverage *= _data.CohesionForce;
    DrawDirection(transform.position, cohesionAverage, Color.magenta, cohesionAverage.magnitude);
    Velocity += cohesionAverage;

    _cohesion = cohesionAverage;
  }

  private void SeparationApply(Vector3 separationAverage, int counter)
  {
    separationAverage /= counter;
    //separationAverage *= _data.Speed * 0.2f;
    separationAverage -= Velocity;
    Vector3.ClampMagnitude(separationAverage, _data.MaxVelocity);
    //Vector3.Lerp(Velocity, separationAverage, 0.5f);

    separationAverage *= _data.SeparationForce;
    DrawDirection(transform.position, separationAverage, Color.red, separationAverage.magnitude);
    Velocity += separationAverage;
    _separation = separationAverage;
  }

  private void BoxAvoidance()
  {
    if(_boundHelper.BoundContains(transform.position))
      return;
    
    Vector3 point = _boundHelper.ClosestBoundPoint(transform.position);
    Vector3 direction = transform.position - point;

    //Debug.DrawLine(transform.position, direction, Color.red);
    
    direction *= _data.Speed * 0.2f;
    Vector3.ClampMagnitude(direction, _data.MaxVelocity);
    
    Velocity -= direction;
  }
  
  private bool Visible(Vector3 at)
  {
    Vector3 toTarget =  at - transform.position;
    float angle = Vector3.Angle(transform.forward, toTarget.normalized);
    bool visible = _data.ViewAngle > angle;
    
    return visible;
  }

  private void DrawDirection(Vector3 from, Vector3 to, Color color, float lenght)
  {
    Vector3 direction = to - from;
    Vector3 target = from + to;//(direction.normalized * lenght);
    
    Debug.DrawLine(from, target, color);
  }
  
  private Vector3 SelfDirection() => 
    transform.position - transform.forward;
}