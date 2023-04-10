using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
  public BoxCollider Collider;

  public bool BoxContains(Vector3 point) => 
    Collider.bounds.Contains(point);
}