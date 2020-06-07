using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public TextAsset levelData;
    public Level currentLevel;

    /*private void Start()
    {
        levelData = (TextAsset) Resources.Load("Waves", typeof(TextAsset));
        LoadLevel(0);
    }*/

    #region Load Levels
    public void LoadLevel(int index)
    {
        if(levelData == null)
            levelData = (TextAsset)Resources.Load("Waves", typeof(TextAsset));

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(levelData.text);

        var levels = xmlDoc.SelectNodes("CampaignLevels/Level");

        //Debug.Log("Found "+levels.Count+" trying to select "+index);

        //Check all found levels for the id
        foreach (XmlNode levelNode in levels)
        {
            //The first Child Node after a Level declaration
            if(levelNode.FirstChild.Name == "ID" && levelNode.FirstChild.InnerText == ""+index)
            {
                int waveCount = 0;
                bool procedural = true;
                float growthFactor = 1.0f;
                Wave[] waves = new Wave[0];

                foreach (XmlNode levelDataNode in levelNode.ChildNodes)
                {
                    if (levelDataNode.Name == "WaveCount")
                    {
                        waveCount = int.Parse(levelDataNode.InnerText);
                    }
                    else if (levelDataNode.Name == "Procedural") procedural = bool.Parse(levelDataNode.InnerText);
                    else if (levelDataNode.Name == "GrowthFactor")
                    {
                        string gfText = levelDataNode.InnerText.Replace(",", ".");
                        float value = float.Parse(gfText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                        //Debug.Log(levelDataNode.InnerText + "_" + value);
                        growthFactor = value;
                    }
                    else
                    {
                        XmlNodeList wavesXML = levelDataNode.ChildNodes;

                        if (!procedural && wavesXML.Count != waveCount)
                        {
                            throw new InvalidDataException("Level " + index + " specifies an invalid amount of waves! Please fix this or set the level to procedural generation!");
                        }

                        waves = new Wave[waveCount];

                        for (int i = 1; i <= waveCount; i++)
                        {
                            foreach (XmlNode wave in wavesXML)
                            {
                                if (wave.FirstChild.InnerText == "" + i)
                                {
                                    List<Entry> spawns = new List<Entry>();
                                    foreach (XmlNode entryNode in wave.LastChild.ChildNodes)
                                    {
                                        Entry newEntry = new Entry();
                                        foreach (XmlNode entryDataNode in entryNode.ChildNodes)
                                        {
                                            if (entryDataNode.Name == "EnemyID") newEntry.enemyID = int.Parse(entryDataNode.InnerText);
                                            else if (entryDataNode.Name == "EnemyCount") newEntry.enemyCount = int.Parse(entryDataNode.InnerText);
                                            else if (entryDataNode.Name == "Delay")
                                            {
                                                string dText = entryDataNode.InnerText.Replace(",", ".");
                                                float value = float.Parse(dText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                                                //Debug.Log(levelDataNode.InnerText + "_" + value);
                                                newEntry.delay = value;
                                            }
                                        }
                                        spawns.Add(newEntry);
                                    }
                                    waves[i - 1] = new Wave(i, spawns);
                                }
                            }
                            if (waves[i - 1] == null) waves[i - 1] = new Wave(i);
                        }
                    }
                }

                currentLevel = new Level(index, waveCount, procedural, growthFactor, waves);

                return;
            }
        }

        throw new InvalidDataException("Requested level either doesn't exist or Level Data File has been corrupted!");
    }
    #endregion

    public Entry[] GetNextWave()
    {
        int waveIndex = GameMaster.gm.currentWave - 1;

        if(waveIndex >= currentLevel.waves.Length)
        {
            throw new System.Exception("Indexed wave not found! Index was "+waveIndex+" but Waves.Length was "+currentLevel.waves.Length);
        }

        if (!currentLevel.waves[waveIndex].procedural)
        {
            Entry[] entries = currentLevel.waves[waveIndex].spawns.ToArray();

            return entries;
        }
        else if (currentLevel.waves[waveIndex].procedural && waveIndex != 0)
        {
            List<Entry> lastWaveEntries = currentLevel.waves[waveIndex - 1].spawns;

            Entry[] entries = new Entry[lastWaveEntries.Count];

            for(int i = 0; i < entries.Length; i++)
            {
                entries[i] = lastWaveEntries[i];
                entries[i].enemyCount = (int)(entries[i].enemyCount * currentLevel.growthFactor);
                currentLevel.waves[waveIndex].spawns.Add(entries[i]);
            }

            //Debug.Log(waveIndex + "__" + entries.Length);

            return entries;
        }


        throw new System.Exception("Error occured while trying to select the next Wave (" + waveIndex + " of level " + currentLevel.levelID + ")!");
    }
}

[System.Serializable]
public class Level {

    public int levelID;
    public int waveCount;
    public bool procedural = false;
    public float growthFactor = 1.0f;
    public Wave[] waves;

    public Level(int ID, int waveCount, bool procedural, float growthFactor, Wave[] waves)
    {
        levelID = ID;
        this.waveCount = waveCount;
        this.procedural = procedural;
        this.growthFactor = growthFactor;
        this.waves = waves;
    }
}

[System.Serializable]
public class Wave {

    public int waveID;
    public bool procedural = false;
    public List<Entry> spawns = new List<Entry>();

    /// <summary>
    /// Generate non-procedural wave
    /// </summary>
    /// <param name="spawns"></param>
    public Wave(int ID, List<Entry> spawns)
    {
        waveID = ID;
        this.spawns = spawns;
        procedural = false;
    }

    /// <summary>
    /// Generate procedural wave
    /// </summary>
    public Wave(int ID)
    {
        waveID = ID;
        procedural = true;
    }
}

[System.Serializable]
public struct Entry
{
    public int enemyID;
    public int enemyCount;
    public float delay;
}
