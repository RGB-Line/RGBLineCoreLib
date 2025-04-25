using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    internal static class StageDataExtension
    {
        internal static bool BIsValid(this StageData targetStageData)
        {
            #region Check Region
            foreach (Guid regionID in targetStageData.RegionDataTable.Keys)
            {
                if (regionID == Guid.Empty)
                {
                    return false;
                }
            }

            Guid[] regionIDs = targetStageData.RegionDataTable.Keys.ToArray();
            for (int regionIDIndex = 0; regionIDIndex < regionIDs.Length; regionIDIndex++)
            {
                StageData.RegionData curRegionData = targetStageData.RegionDataTable[regionIDs[regionIDIndex]];

                if (curRegionData.StartOffsetFrame < 0)
                {
                    return false;
                }
                else if (regionIDIndex + 1 < regionIDs.Length && curRegionData.StartOffsetFrame >= targetStageData.RegionDataTable[regionIDs[regionIDIndex + 1]].StartOffsetFrame)
                {
                    return false;
                }

                if (curRegionData.MinorOffsetTime < -0.5f || 0.5f < curRegionData.MinorOffsetTime)
                {
                    return false;
                }
                else if (!Enum.IsDefined(typeof(StageData.RegionData.ColorType), curRegionData.CurColorType))
                {
                    return false;
                }
            }
            #endregion

            #region Check Line
            foreach (Guid lineID in targetStageData.LineDataTable.Keys)
            {
                if (lineID == Guid.Empty)
                {
                    UnityEngine.Debug.Log("1");
                    return false;
                }
            }

            Guid[] lineIDs = targetStageData.LineDataTable.Keys.ToArray();
            for (int lineIDIndex = 0; lineIDIndex < lineIDs.Length; lineIDIndex++)
            {
                StageData.LineData curLineData = targetStageData.LineDataTable[lineIDs[lineIDIndex]];

                if (curLineData.AttachedRegionID == Guid.Empty)
                {
                    return false;
                }
                else if (!regionIDs.Contains(curLineData.AttachedRegionID))
                {
                    return false;
                }

                if (curLineData.CurvedLinePoints.Count < 2)
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(StageData.LineData.LineSmoothType), curLineData.CurLineSmoothType))
                {
                    return false;
                }
                switch (targetStageData.RegionDataTable[curLineData.AttachedRegionID].CurColorType)
                {
                    case StageData.RegionData.ColorType.Green:
                        if (curLineData.CurLineSmoothType != StageData.LineData.LineSmoothType.Curved)
                        {
                            return false;
                        }
                        break;

                    case StageData.RegionData.ColorType.Red:
                        if (curLineData.CurLineSmoothType != StageData.LineData.LineSmoothType.Linear)
                        {
                            return false;
                        }
                        break;

                    case StageData.RegionData.ColorType.Blue:
                        if (curLineData.CurLineSmoothType != StageData.LineData.LineSmoothType.Linear)
                        {
                            return false;
                        }
                        else if (curLineData.CurvedLinePoints.Count != 2)
                        {
                            return false;
                        }
                        break;
                }

                int prevLinePointYPos = -1;
                foreach (HalfFloatVector2 point in curLineData.CurvedLinePoints)
                {
                    if (point.Y < prevLinePointYPos)
                    {
                        return false;
                    }
                    prevLinePointYPos = point.Y;
                }

                if (curLineData.CurvedLinePoints.Count != curLineData.MinorOffsetTimes.Count)
                {
                    return false;
                }
                foreach (float minorOffsetTime in curLineData.MinorOffsetTimes)
                {
                    if (minorOffsetTime < -0.5f || 0.5f < minorOffsetTime)
                    {
                        return false;
                    }
                }

                HalfFloatVector2 prevPoint = -HalfFloatVector2.one;
                switch (targetStageData.RegionDataTable[curLineData.AttachedRegionID].CurColorType)
                {
                    case StageData.RegionData.ColorType.Red:
                    case StageData.RegionData.ColorType.Green:
                        foreach (HalfFloatVector2 point in curLineData.CurvedLinePoints)
                        {
                            if (prevPoint == -HalfFloatVector2.one)
                            {
                                prevPoint = point;
                            }
                            else
                            {
                                if (prevPoint.X == point.X)
                                {
                                    return false;
                                }
                                prevPoint = point;
                            }
                        }
                        break;

                    case StageData.RegionData.ColorType.Blue:
                        foreach (HalfFloatVector2 point in curLineData.CurvedLinePoints)
                        {
                            if (prevPoint == -HalfFloatVector2.one)
                            {
                                prevPoint = point;
                            }
                            else
                            {
                                if (prevPoint.X != point.X)
                                {
                                    return false;
                                }
                                prevPoint = point;
                            }
                        }
                        break;
                }

                if (curLineData.LineWidth < 0.0f)
                {
                    return false;
                }
            }

            // Key - RegionID, Value - List of Attached LineID
            Dictionary<Guid, List<Guid>> attachedLineIDTable = new Dictionary<Guid, List<Guid>>();
            foreach (Guid lineID in lineIDs)
            {
                if (!attachedLineIDTable.ContainsKey(targetStageData.LineDataTable[lineID].AttachedRegionID))
                {
                    attachedLineIDTable.Add(targetStageData.LineDataTable[lineID].AttachedRegionID, new List<Guid>());
                }
                attachedLineIDTable[targetStageData.LineDataTable[lineID].AttachedRegionID].Add(lineID);
            }

            foreach (Guid regionID in attachedLineIDTable.Keys)
            {
                if (targetStageData.RegionDataTable[regionID].CurColorType == StageData.RegionData.ColorType.Blue)
                {
                    for (int mainLineIndex = 0; mainLineIndex < attachedLineIDTable[regionID].Count; mainLineIndex++)
                    {
                        for (int subLineIndex = 0; subLineIndex < attachedLineIDTable[regionID].Count; subLineIndex++)
                        {
                            if (mainLineIndex == subLineIndex)
                            {
                                continue;
                            }

                            if (targetStageData.LineDataTable[attachedLineIDTable[regionID][mainLineIndex]].CurvedLinePoints[0].X == targetStageData.LineDataTable[attachedLineIDTable[regionID][subLineIndex]].CurvedLinePoints[0].X)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Check Note
            foreach (Guid noteID in targetStageData.NoteDataTable.Keys)
            {
                if (noteID == Guid.Empty)
                {
                    return false;
                }
            }

            Guid[] noteIDs = targetStageData.NoteDataTable.Keys.ToArray();
            for (int noteIDIndex = 0; noteIDIndex < noteIDs.Length; noteIDIndex++)
            {
                StageData.NoteData curNoteData = targetStageData.NoteDataTable[noteIDs[noteIDIndex]];

                if (curNoteData.AttachedLineID == Guid.Empty)
                {
                    return false;
                }
                else if (!lineIDs.Contains(curNoteData.AttachedLineID))
                {
                    return false;
                }

                if (curNoteData.MinorOffsetTime < -0.5f || 0.5f < curNoteData.MinorOffsetTime)
                {
                    return false;
                }
                else if (!Enum.IsDefined(typeof(StageData.NoteData.NoteType), curNoteData.CurNoteType))
                {
                    return false;
                }

                switch(curNoteData.CurNoteType)
                {
                    case StageData.NoteData.NoteType.Common:
                        if (curNoteData.NoteLength != 0)
                        {
                            return false;
                        }
                        break;

                    case StageData.NoteData.NoteType.Long:
                        if (curNoteData.NoteLength <= 0)
                        {
                            return false;
                        }
                        break;

                    case StageData.NoteData.NoteType.Double:
                        if (curNoteData.NoteLength != 0)
                        {
                            return false;
                        }
                        break;
                }
            }

            // Key - LineID, Value - List of Attached NoteID
            Dictionary<Guid, List<Guid>> attachedNoteIDTable = new Dictionary<Guid, List<Guid>>();
            foreach (Guid noteID in noteIDs)
            {
                if (!attachedNoteIDTable.ContainsKey(targetStageData.NoteDataTable[noteID].AttachedLineID))
                {
                    attachedNoteIDTable.Add(targetStageData.NoteDataTable[noteID].AttachedLineID, new List<Guid>());
                }
                attachedNoteIDTable[targetStageData.NoteDataTable[noteID].AttachedLineID].Add(noteID);
            }

            foreach (Guid lineID in attachedNoteIDTable.Keys)
            {
                foreach (Guid noteID in attachedNoteIDTable[lineID])
                {
                    foreach(Guid searchNoteID in attachedNoteIDTable[lineID])
                    {
                        if(noteID == searchNoteID)
                        {
                            continue;
                        }

                        // Cur Note Frame Position
                        float curNoteFramePos = 0.0f;

                        StageData.RegionData curRegionData = targetStageData.RegionDataTable[targetStageData.LineDataTable[targetStageData.NoteDataTable[noteID].AttachedLineID].AttachedRegionID];
                        curNoteFramePos += curRegionData.StartOffsetFrame + targetStageData.NoteDataTable[noteID].StartOffsetFrame + targetStageData.NoteDataTable[noteID].MinorOffsetTime;

                        StageData.LineData curLineData = targetStageData.LineDataTable[targetStageData.NoteDataTable[noteID].AttachedLineID];
                        curNoteFramePos += curLineData.CurvedLinePoints[0].Y;

                        curNoteFramePos += targetStageData.NoteDataTable[noteID].StartOffsetFrame + targetStageData.NoteDataTable[noteID].MinorOffsetTime;

                        // Search Note Frame Position
                        float searchNoteFramePos = 0.0f;

                        StageData.RegionData searchNoteData = targetStageData.RegionDataTable[targetStageData.LineDataTable[targetStageData.NoteDataTable[searchNoteID].AttachedLineID].AttachedRegionID];
                        searchNoteFramePos += curRegionData.StartOffsetFrame + targetStageData.NoteDataTable[searchNoteID].StartOffsetFrame + targetStageData.NoteDataTable[searchNoteID].MinorOffsetTime;

                        StageData.LineData searchLineData = targetStageData.LineDataTable[targetStageData.NoteDataTable[searchNoteID].AttachedLineID];
                        searchNoteFramePos += curLineData.CurvedLinePoints[0].Y;

                        searchNoteFramePos += targetStageData.NoteDataTable[searchNoteID].StartOffsetFrame + targetStageData.NoteDataTable[searchNoteID].MinorOffsetTime;

                        switch (targetStageData.NoteDataTable[noteID].CurNoteType)
                        {
                            case StageData.NoteData.NoteType.Common:
                            case StageData.NoteData.NoteType.Double:
                                if (curNoteFramePos == searchNoteFramePos)
                                {
                                    return false;
                                }
                                break;

                            case StageData.NoteData.NoteType.Long:
                                if ((curNoteFramePos <= searchNoteFramePos) && (searchNoteFramePos <= curNoteFramePos + targetStageData.NoteDataTable[noteID].NoteLength))
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }
            }
            #endregion

            #region Check Stage Config
            if (targetStageData.StageConfig.BPM <= 0)
            {
                return false;
            }
            else if (targetStageData.StageConfig.BitSubDivision <= 0)
            {
                return false;
            }
            else if (targetStageData.StageConfig.LengthPerBit <= 0.0f)
            {
                return false;
            }
            else if (targetStageData.StageConfig.MusicStartOffsetTime < 0.0f)
            {
                return false;
            }
            #endregion

            return true;
        }

        internal static void Dispose(this StageData targetStageData)
        {
            #region Dispose Region
            targetStageData.RegionDataTable.Clear();
            targetStageData.RegionDataTable = null;
            #endregion

            #region Dispose Line
            Dictionary<Guid, StageData.LineData>.KeyCollection lineIDs = targetStageData.LineDataTable.Keys;
            foreach (Guid lineID in lineIDs)
            {
                targetStageData.LineDataTable[lineID].CurvedLinePoints.Clear();
                targetStageData.LineDataTable[lineID].MinorOffsetTimes.Clear();
            }

            targetStageData.LineDataTable.Clear();
            targetStageData.LineDataTable = null;
            #endregion

            #region Dispose Note
            targetStageData.NoteDataTable.Clear();
            targetStageData.NoteDataTable = null;
            #endregion
        }
    }
}