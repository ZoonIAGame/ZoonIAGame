using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool generateTrees = false;
    public enum DrawMode
    {
        NoiseMap, ColorMap, Mesh, FalloffMap
    }
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range (0,6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool AutoUpdate;

    public bool useFalloff;

    public bool UseBiomes;
    public bool HeightColorOverride;

    public float biomeHeightTone;
    
    public float rainNoiseScale;
    public int rainOctaves;
    [Range(0, 1)]
    public float rainPersistance;
    public float rainLacunarity;

    public int rainSeed;
    public Vector2 rainOffset;

    public float heatNoiseScale;
    public int heatOctaves;
    [Range(0, 1)]
    public float heatPersistance;
    public float heatLacunarity;

    public int heatSeed;
    public Vector2 heatOffset;

    public TerrainType[] regions;

    public TerrainType[] waterRegions;

    public BiomeType[] biomes;

    private float[,] falloffMap;

    private GameManager manager;

    private float contador = 0;

    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        GenerateMap();
    }

    private void Update()
    {
        if (generateTrees && (!Application.isEditor || Application.isPlaying))
        {
            if (contador >= (1f / manager.udtPorSegundo) * 150)
            {
                this.GetComponent<TreeBushGenerator>().generarArbustos();
                contador = 0;
            }
            contador += Time.deltaTime;
        }
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] rainMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, rainSeed, rainNoiseScale, rainOctaves, rainPersistance, rainLacunarity, rainOffset);
        float[,] heatMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, heatSeed, heatNoiseScale, heatOctaves, heatPersistance, heatLacunarity, heatOffset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        Color[] waterColorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                float currentRain = rainMap[x, y];
                float currentHeat = heatMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        if (UseBiomes)
                        {
                            if (HeightColorOverride)
                            {
                                for (int j = biomes.Length - 1; j >= 0; j--)
                                {
                                    if (currentHeat <= biomes[j].temperatureHeight)
                                    {
                                        if (currentRain <= biomes[j].rainfallHeight)
                                        {
                                            colorMap[y * mapChunkSize + x] = biomes[j].color;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (!HeightColorOverride && regions[i].height > 0.5f && regions[i].height <= 0.67f)
                            {
                                for (int j = biomes.Length - 1; j >= 0; j--)
                                {
                                    if (currentHeat <= biomes[j].temperatureHeight)
                                    {
                                        if (currentRain <= biomes[j].rainfallHeight)
                                        {
                                            colorMap[y * mapChunkSize + x] = biomes[j].color * (1 - noiseMap[x, y] + biomeHeightTone);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

                for (int k = 0; k < waterRegions.Length; k++)
                {
                    if (currentHeight <= waterRegions[k].height)
                    {
                        waterColorMap[y * mapChunkSize + x] = waterRegions[k].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }else if (drawMode == DrawMode.Mesh)
        {
            MeshData meshdata = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
            display.DrawMesh(meshdata, TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
            //display.DrawWaterTexture(TextureGenerator.TextureFromColorMap(waterColorMap, mapChunkSize, mapChunkSize));
            display.waterDrawMesh(MeshGenerator.GenerateWaterMesh(noiseMap, levelOfDetail), TextureGenerator.TextureFromColorMap(waterColorMap, mapChunkSize, mapChunkSize));
            if (generateTrees && (!Application.isEditor || Application.isPlaying))
            {
                TreeBushGenerator treeBushGenerator = this.GetComponent<TreeBushGenerator>();
                treeBushGenerator.generarPlantaciones(meshdata);
                treeBushGenerator.generarArbolesYArbustos();
            }
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

[System.Serializable]
public struct BiomeType
{
    public string name;
    public float rainfallHeight;
    public float temperatureHeight;
    public Color color;
}