using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class CSVWrite : MonoBehaviour
{
    string filename = "";
    //int indexCounter = 0;

    public class CheatStats
    {
        public int trackTime;
        public float triggerTime;
        public float flickTime;
    }

    public class StatList
    {
        public CheatStats[] statList = new CheatStats[100];
    }

    public StatList newStatList = new StatList();

    private void Start()
    {
        filename = Path.Combine(Application.dataPath, "mlstats.csv");
        Debug.Log("CSV will be saved to: " + filename);

        try
        {
            // Write headers
            using (StreamWriter tw = new StreamWriter(filename, false))
            {
                tw.WriteLine("Time Tracked Through Walls, Trigger Reaction Time, Time to Flick to Target");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to write CSV headers: " + e.Message);
        }
    }

    public void WriteCSV(int trackTime, int triggerTime, float flickTime)
    {
        Debug.Log("Attempting to Write");
        string line = $"{trackTime},{triggerTime},{flickTime}";
        Debug.Log("Writing line: " + line);

        try
        {
            using (StreamWriter tw = new StreamWriter(filename, true))
            {
                tw.WriteLine(line);
            }
            Debug.Log("Write successful");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to write CSV: " + e.Message);
        }
    }

    /*
    public void LogData(int track, float trigger, float flick)
    {
        Debug.Log("Logging Data");

        CheatStats newStat = new CheatStats();
        newStat.trackTime = track;
        newStat.triggerTime = trigger;
        newStat.flickTime = flick;
        
        if (indexCounter >= newStatList.statList.Length)
        {
            Debug.LogWarning("Stat list full, cannot log more data");
            return;
        }

        newStatList.statList[indexCounter] = newStat;
        indexCounter++;

        WriteCSV();
    }
    */
}   