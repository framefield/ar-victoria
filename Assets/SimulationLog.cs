//﻿using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;
//
//public class SimulationLog : MonoBehaviour
//{
//    [SerializeField]
//    float _logRate = 1f;
//
//    void Start()
//    {
////        _simulation.OnAgentSpawned += HandleSpawnedAgent;
//        StartCoroutine(WriteTimeIntoFile());
//    }
//
//    private IEnumerator WriteTimeIntoFile()
//    {
//
//        using (StreamWriter sw = new StreamWriter(_csvPath))
//        {
//            sw.Write(GenerateHeader(_interestCategoriesInProject));
//
//            while (true)
//            {
//                _timeSinceLastLog += Time.deltaTime;
//                if (_timeSinceLastLog > _logRate)
//                {
//                    _timeSinceLastLog -= _logRate;
//                    foreach (var agentLogData in LoggedAgents.Values)
//                    {
//                        var csvLine = agentLogData.LogSlice(_interestCategoriesInProject);
//                        sw.Write(csvLine);
//                    }
//                }
//                yield return null;
//            }
//        }
//    }
//
//
//    private static string GenerateHeader(List<InterestCategory> categories)
//    {
//        var header = "";
//        header += "AgentID\t";
//        header += "SimulationTimeInSeconds\t";
//        header += "PosX\t";
//        header += "PosY\t";
//        header += "PosZ\t";
//        header += "AgentCategory\t";
//        header += "LockedInterest\t";
//
//        foreach (var category in categories)
//        {
//            header += "Interest." + category.name + "\t";
//        }
//        return header + "\n";
//    }
//
//    private const string SUBFOLDER_PATH = "logs/";
//
//    private static string _csvPath
//    {
//        get
//        {
//            bool exists = System.IO.Directory.Exists(SUBFOLDER_PATH);
//            if (!exists)
//                System.IO.Directory.CreateDirectory(SUBFOLDER_PATH);
//
//            var filename = DateTime.Now.ToString("yyyyMMdd_hhmmss");
//            return SUBFOLDER_PATH + filename + ".csv";
//        }
//    }
//
//    private Color _defaultGizmoColor { get { return new Color(0.5f, 0.5f, 0.5f, _gizmoAlpha); } }
//
//    private float _timeSinceLastLog;
//    
//    public string ToCSVString(List<InterestCategory> interestCategories)
//    {
//        var csvLine = "";
//        csvLine += _agent.id + "\t"
//                             + SimulationTimeInSeconds + "\t"
//                             + Position.x + "\t"
//                             + Position.y + "\t"
//                             + Position.z + "\t"
//                             + _agent.AgentCategory.name + "\t";
//
//        var lockedInterest = LockedInterest != null ? LockedInterest.name : "-";
//        csvLine += lockedInterest + "\t";
//
//        foreach (var category in interestCategories)
//        {
//            if (CurrentInterests.ContainsKey(category))
//            {
//                csvLine += CurrentInterests[category] + "\t";
//                continue;
//            }
//
//            if (CurrentSocialInterests.ContainsKey(category))
//            {
//                csvLine += CurrentSocialInterests[category] + "\t";
//                continue;
//            }
//            csvLine += "0.0" + "\t";
//        }
//        csvLine += "\n";
//        return csvLine;
//    }
//
//}
