using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    public static class StageDataInterface
    {
        public static class RegionDataInterface
        {
            #region Getter
            public static StageData.RegionData GetRegionData(in Guid targetRegionID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[targetRegionID];
            }

            public static Guid[] GetRegionIDs()
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable.Keys.ToArray();
            }

#if FOR_EDITOR
            public
#else
            internal
#endif
            static Guid GetNextRegionID(in Guid curRegionID)
            {
                Guid nextRegionID = Guid.Empty;
                Guid[] regionIDs = GetRegionIDs();

                StageData.RegionData curRegionData = GetRegionData(curRegionID);
                foreach (Guid regionID in regionIDs)
                {
                    StageData.RegionData candidateRegionData = GetRegionData(regionID);
                    if (candidateRegionData.StartOffsetFrame > curRegionData.StartOffsetFrame)
                    {
                        nextRegionID = regionID;
                        break;
                    }
                }

                return nextRegionID;
            }

            internal static int GetRegionDataIndex(in Guid targetRegionID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable.Keys.ToList().IndexOf(targetRegionID);
            }
            internal static bool BIsRegionDataValid(in Tuple<Guid, StageData.RegionData> targetRegionData)
            {
                if (targetRegionData.Item1 == Guid.Empty)
                {
                    return false;
                }

                int targetRegionDataIndex = GetRegionDataIndex(targetRegionData.Item1);
                if (targetRegionDataIndex == 0 && targetRegionData.Item2.StartOffsetFrame != 0)
                {
                    return false;
                }
                else if (StageDataBuffer.Instance.StageData.RegionDataTable.Values.ToList()[targetRegionDataIndex - 1].StartOffsetFrame > targetRegionData.Item2.StartOffsetFrame)
                {
                    return false;
                }

                if (targetRegionData.Item2.MinorOffsetTime < -0.5f || 0.5f < targetRegionData.Item2.MinorOffsetTime)
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(StageData.RegionData.ColorType), targetRegionData.Item2.CurColorType))
                {
                    return false;
                }

                return true;
            }
#endregion

#if FOR_EDITOR
            #region Setter
            public static bool TryAddRegionData(in Guid targetRegionID, in StageData.RegionData targetRegionData)
            {
                //if(!BIsRegionDataValid(new Tuple<Guid, StageData.RegionData>(targetRegionID, targetRegionData)))
                //{
                //    return false;
                //}

                if (StageDataBuffer.Instance.StageData.RegionDataTable.ContainsKey(targetRegionID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.RegionDataTable.Add(targetRegionID, targetRegionData);
                return true;
            }
            #endregion
#endif
        }
        public static class LineDataInterface
        {
            #region Getter
            public static Guid GetAttachedRegionID(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[targetLineID].AttachedRegionID;
            }
            public static StageData.RegionData GetAttachedRegionData(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[StageDataBuffer.Instance.StageData.LineDataTable[targetLineID].AttachedRegionID];
            }

            public static StageData.LineData GetLineData(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[targetLineID];
            }

            public static Guid[] GetLineIDs()
            {
                return StageDataBuffer.Instance.StageData.LineDataTable.Keys.ToArray();
            }

            internal static bool BIsLineDataValid(in Tuple<Guid, StageData.LineData> targetLineData)
            {
                if(targetLineData.Item1 == Guid.Empty)
                {
                    return false;
                }

                if (targetLineData.Item2.AttachedRegionID == Guid.Empty)
                {
                    return false;
                }
                else if(!RegionDataInterface.GetRegionIDs().Contains(targetLineData.Item2.AttachedRegionID))
                {
                    return false;
                }

                if (targetLineData.Item2.CurvedLinePoints.Count < 2)
                {
                    return false;
                }
                int prevLinePointYPos = -1;
                foreach (HalfFloatVector2 point in targetLineData.Item2.CurvedLinePoints)
                {
                    if (point.Y < prevLinePointYPos)
                    {
                        return false;
                    }
                    prevLinePointYPos = point.Y;
                }

                if (targetLineData.Item2.CurvedLinePoints.Count != targetLineData.Item2.MinorOffsetTimes.Count)
                {
                    return false;
                }
                foreach (float minorOffsetTime in targetLineData.Item2.MinorOffsetTimes)
                {
                    if (minorOffsetTime < -0.5f || 0.5f < minorOffsetTime)
                    {
                        return false;
                    }
                }

                if (targetLineData.Item2.LineWidth < 0.0f)
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(StageData.LineData.LineSmoothType), targetLineData.Item2.CurLineSmoothType))
                {
                    return false;
                }

                return true;
            }
            #endregion
        }
        public static class NoteDataInterface
        {
            #region Getter
            public static StageData.RegionData GetAttachedRegionData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[StageDataBuffer.Instance.StageData.LineDataTable[StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID].AttachedLineID].AttachedRegionID];
            }
            public static StageData.LineData GetAttachedLineData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID].AttachedLineID];
            }

            public static bool BIsNoteIDValid(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable.ContainsKey(targetNoteID);
            }
            public static StageData.NoteData GetNoteData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID];
            }

            public static Guid[] GetNoteIDs()
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable.Keys.ToArray();
            }
            #endregion
        }
        public static class StageConfigDataInterface
        {
            #region Getter
            public static StageData.StageConfigData GetStageConfigData()
            {
                return StageDataBuffer.Instance.StageData.StageConfig;
            }
            #endregion
        }


        public static bool TryLoadStageData(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            return StageDataBuffer.Instance.TryLoadStageData(targetStageName, majorDifficulty);
        }
        public static void DisposeStageData()
        {
            StageDataBuffer.Instance.Dispose();
        }

        public static bool BIsStageDataValid()
        {
            return StageDataBuffer.Instance.StageData.BIsValid();
        }
    }
}