using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RGBLineCoreLib.StageData;


namespace RGBLineCoreLib
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

            public static List<Guid> GetRegionIDs()
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable.Keys.ToList();
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
                if(!BIsRegionDataValid(new Tuple<Guid, StageData.RegionData>(targetRegionID, targetRegionData)))
                {
                    return false;
                }

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
            public static StageData.RegionData GetAttachedRegionData(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[StageDataBuffer.Instance.StageData.LineDataTable[targetLineID].AttachedRegionID];
            }
            public static StageData.LineData GetLineData(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[targetLineID];
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
            public static StageData.NoteData GetNoteData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID];
            }
            #endregion
        }
        public static class StageConfigDataInterface
        {
            #region Getter
            public static StageData.StageConfigData GetStageMetadata()
            {
                return StageDataBuffer.Instance.StageData.StageConfig;
            }
            #endregion
        }


        public static bool BIsStageDataValid()
        {
            return StageDataBuffer.Instance.StageData.BIsValid();
        }
    }
}