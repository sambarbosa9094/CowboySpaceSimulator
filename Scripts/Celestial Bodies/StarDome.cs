using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDome : MonoBehaviour {
    public int renderDistance = 16;
    public Star starPrefab;

    private Vector3 maxCoord;
    private Vector3 minCoord;
    private List<Star> stars;
    private Vector3 oldGridCoords;

    void Start() {
        stars = new List<Star>();
        oldGridCoords = Vector3.zero;
        maxCoord = Vector3.zero;
        minCoord = Vector3.zero;
        GenerateStars(32);
        Debug.Log(CoordsUpdate.gridCoords.ToString());
        foreach(Star star in stars) Debug.Log(star.gridCoords.ToString());
    }

    void GenerateStars(int distance) {
        for(int i = 0; i < distance; i++)
            for(int j = 0; j < distance; j++)
                for(int k = 0; k < distance; k++) {
                    Vector3 starGridCoords = CoordsUpdate.gridCoords + new Vector3(i,j,k) - (distance * Vector3.one / 2);
                    int val = Random.Range(0,2500);
                    if(IsOutOfBounds(starGridCoords) && 1 == val) {
                        Debug.Log(val);
                        CreateStar(starGridCoords);
                    }
                }
    }

    bool IsOutOfBounds(Vector3 coord) {
        maxCoord = SignedMax(coord, maxCoord);
        minCoord = SignedMin(coord, minCoord);
        return (maxCoord.Equals(coord) || minCoord.Equals(coord));
    }

    Vector3 SignedMax(Vector3 v1, Vector3 v2) {
        return (v1.x > v2.x || v1.y > v2.y || v1.z > v2.z) ? v1 : v2;
    }

    Vector3 SignedMin(Vector3 v1, Vector3 v2) {
        return (v1.x < v2.x || v1.y < v2.y || v1.z < v2.z) ? v1 : v2;
    }

    void CreateStar(Vector3 starGridCoords) {
        float mass = StarMassSetter();
        float radius = mass >= 1f ? Mathf.Sqrt(mass) : Mathf.Pow(mass, .8f);
        Star star = Instantiate(starPrefab);
        star.solarMass = mass;
        star.solarRadius = radius;
        star.gridCoords = starGridCoords;
        stars.Add(star);
    }

    float StarMassSetter() {
        float rand = 100f * Random.value;
        float mass = 0;
        if (rand <= .5f) mass = Random.Range(16f, 120f);
        else if (rand <= 1.5f) mass = Random.Range(2.1f, 16f);
        else if (rand <= 4f) mass = Random.Range(1.4f, 2.1f);
        else if (rand <= 8f) mass = Random.Range(1.04f, 1.4f);
        else if (rand <= 15f) mass = Random.Range(.8f, 1.04f);
        else if (rand <= 30f) mass = Random.Range(.45f, .8f);
        else mass = Random.Range(.08f, .45f);
        return mass;
    }

    void Update() {
        if(Vector3.Distance(oldGridCoords, CoordsUpdate.gridCoords) >= 1) {
            oldGridCoords = CoordsUpdate.gridCoords;
            GenerateStars(renderDistance);
            CheckRenderStars();
        }
    }

    void CheckRenderStars() {
        foreach (Star star in stars) {
            if(!star.obj.activeSelf) {
                if(Vector3.Distance(star.gridCoords, CoordsUpdate.gridCoords) < 1f) {
                    star.obj.SetActive(true);
                    star.body.position = Universe.lightyear * (star.gridCoords - CoordsUpdate.gridCoords);
                }
                else star.obj.SetActive(false);

                for(int i = 0; i < star.planets.Length; i++)
                        star.planets[i].obj.SetActive(star.obj.activeSelf);
            }
        }
    }
}
