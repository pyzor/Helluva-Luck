using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTileResources : MonoBehaviour {
    public static WorldTileResources Instance { get; private set; }

    // Sadly, didn't work with the editor
    //[SerializeField] public struct TileResource<T> {
    //    [SerializeField] public string Name;
    //    [SerializeField] public T Data;
    //}

    [Serializable] public struct TileParticle {
        [SerializeField] public string Name;
        [SerializeField] public ParticleSystem ParticleSystem;
    }

    [SerializeField] private List<TileParticle> TileParticles;


    public ParticleSystem GetTileParticles(string name) {
        for(int i = 0; i < TileParticles.Count; i++) {
            if (TileParticles[i].Name.Equals(name)) {
                return TileParticles[i].ParticleSystem;
            }
        }
        return null;
    }

    void Awake() {
        if(TileParticles == null)
            TileParticles = new List<TileParticle>();
        
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        } else {
            Destroy(transform.root.gameObject);
        }
    }


    void Start() {
    }

    void Update() {

    }
}
