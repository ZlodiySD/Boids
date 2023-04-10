using UnityEngine;

public class BoundHelper
{
  private Bounds _bounds;

  public BoundHelper(Bounds bounds) => 
    _bounds = bounds;

  public bool BoundContains(Vector3 point) => 
    _bounds.Contains(point);

  public Vector3 ClosestBoundPoint(Vector3 from) => 
    _bounds.ClosestPoint(@from);
}