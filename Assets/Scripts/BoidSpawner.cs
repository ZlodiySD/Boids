using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BoidSpawner : MonoBehaviour
{
  public BoidBehaviour boidPrefab;
  public Transform prefabHolder;

  public BoidManager Manager;
  public WallBehaviour wall;
  
  private List<BoidBehaviour> _boidBehaviours = new List<BoidBehaviour>();

  [Range(1,500)]
  public int Count;

  private BoundHelper _boundHelper;
  
  private void Start()
  {
    _boundHelper = new BoundHelper(wall.Collider.bounds);
    //SpawnBoids(Count);
    SpawnTest();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.R))
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    
    //return;
    foreach (BoidBehaviour boid in _boidBehaviours)
    {
      Vector3 position = boid.transform.position;
      if (!wall.BoxContains(position))
      {
        boid.transform.position = wall.Collider.ClosestPoint((-position));
      }
    }
  }

  private void SpawnTest()
  {
    int count = 2;
    BoidBehaviour boid = SpawnBoid(Vector3.zero, Quaternion.LookRotation(-Vector3.forward));
      
    boid.Init(Manager.AgentData, _boundHelper);
    boid.SetMaxBoids(count);
    _boidBehaviours.Add(boid);
    
    BoidBehaviour boid3 = SpawnBoid(Vector3.up * 3, Quaternion.LookRotation(-Vector3.forward));
      
    boid3.Init(Manager.AgentData, _boundHelper);
    boid3.SetMaxBoids(count);
    _boidBehaviours.Add(boid3);
    
    
    BoidBehaviour boid2 = SpawnBoid(Vector3.forward * 3, Quaternion.LookRotation(-Vector3.forward));
      
    boid2.Init(Manager.AgentData, _boundHelper);
    boid2.SetMaxBoids(count);
    _boidBehaviours.Add(boid2);
  }

  public void SpawnBoids(int count)
  {
    for (int i = 0; i < count; i++)
    {
      BoidBehaviour boid = SpawnBoid(RandomPointInBounds(wall.Collider.bounds), Random.rotation);
      
      boid.Init(Manager.AgentData, _boundHelper);
      boid.SetMaxBoids(count);
      _boidBehaviours.Add(boid);
    }
  }

  public BoidBehaviour SpawnBoid(Vector3 position, Quaternion rotation) => 
    Instantiate(boidPrefab,position , rotation, prefabHolder);
  
  public Vector3 RandomPointInBounds(Bounds bounds) {
    return new Vector3(
      Random.Range(bounds.min.x, bounds.max.x),
      Random.Range(bounds.min.y, bounds.max.y),
      Random.Range(bounds.min.z, bounds.max.z)
    );
  }
}