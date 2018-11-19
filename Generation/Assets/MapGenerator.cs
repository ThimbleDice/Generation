using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

    public enum GenerationType
    {
        random, perlinNoise
    }
    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;

    public Tilemap tilemap;
    public TerrainType[] regions;
    public TerrainType[] minerals;

    private TileBase FindTileFromRegion(float valeur)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (valeur <= regions[i].value)
            {
                return regions[i].tile;
            }
        }
        return regions[0].tile;
    }

    private TileBase FindTileFromMinerals(float valeur)
    {
        for (int i = 0; i < minerals.Length; i++)
        {
            if (valeur <= minerals[i].value)
            {
                return minerals[i].tile;
            }
        }
        return minerals[0].tile;
    }

    public void SetTileMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTileMap[y * mapHeight + x]);
            }
        }
    }

    public void GenerateMapRandom()
    {
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = Random.Range(0f, 1f);
                customTileMap[y * mapHeight + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customTileMap);
    }

    public void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    public void GenerateMap()
    {
        if (generationType == GenerationType.random)
        {
            GenerateMapRandom();
        }
        else if(generationType == GenerationType.perlinNoise)
        {
            GenerateMapPerlinNoise();
        }
    }

    public void GenerateMapPerlinNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapMinerals = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + 1, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float value = noiseMap[x, y];
                if (value >= 0.8f)
                {
                    customTileMap[y * mapHeight + x] = FindTileFromMinerals(noiseMapMinerals[x, y]);
                }
                else
                {
                    customTileMap[y * mapHeight + x] = FindTileFromRegion(value);
                }
            }
        }
        SetTileMap(customTileMap);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float value;
    public TileBase tile;
}