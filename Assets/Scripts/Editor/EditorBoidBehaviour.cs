using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorBoidBehaviour : Editor
{
  [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.Selected)]
  public static void RenderCustomGizmo(BoidBehaviour boid, GizmoType gizmo)
  {
    Gizmos.color = Color.gray;
    Gizmos.DrawWireSphere(boid.transform.position, boid.Data.Radius);
  }
}
