using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtyCalc.Data
{
    public class BatteryData
    {
        public string Name;
        public string Callsign;

        public string Weapon;
        public CoordinateData Coordinates;
        public string DirectionOfFire;

        public string TargetNumberPrefix;
        public int TargetNumberStart;

        public List<PointData> Observers;
        public List<PointData> KnownPoints;
        public List<MissionData> Missions;
    }

    public class CoordinateData
    {
        public int Format;

        public double GridX;
        public double GridY;
        public double Altitude;
    }

    public class PointData
    {
        public string Name;
        public CoordinateData Coordinates;
    }

    public abstract class MissionData
    {
        public string TargetNumber;
        public string TargetDescription;
        public double Radius;
        public double Length;
        public string Attitude;
        public bool DangerClose;
        public string Note;
        public string Ammunition;
        public string Fuze;
        public int AdjustPieces;
        public int AdjustRounds;
    }
}
