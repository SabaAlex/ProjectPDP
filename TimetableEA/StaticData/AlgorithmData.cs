using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableEA.StaticData
{
    static public class AlgorithmData
    {
        public static Dictionary<int, string> TimeDecoded = new Dictionary<int, string>()
        {
            { 0, "8::00" },
            { 1, "9::00" },
            { 2, "10::00" },
            { 3, "11::00" },
            { 4, "12::00" },
            { 5, "13::00" },
            { 6, "14::00" },
            { 7, "15::00" },
            { 8, "16::00" },
            { 9, "17::00" },
        };
        public static Dictionary<int, string> DayDecoded = new Dictionary<int, string>()
        {
            { 0, "Monday" },
            { 1, "Tuesday" },
            { 2, "Wednesday" },
            { 3, "Thursday" },
            { 4, "Friday" },
        };

        public static int LocationNumber = 3;

        public static int CoursesNumber = 3;
        public static int GroupsNumber = 3;
    }
}
