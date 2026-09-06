using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Day
{
    public int Year;
    public int DayOfYear;

    public Day(int year, int dayOfYear)
    {
        Year = year;
        DayOfYear = dayOfYear;
    }

    public bool IsEqualDay(DateTime dateTime)
    {
        return dateTime.Year == Year && dateTime.DayOfYear == DayOfYear;
    }
}

[Serializable]
public class DayEqualityComparer  : IEqualityComparer<Day>
{
    public bool Equals(Day x, Day y)
    {
        return x.Year == y.Year && x.DayOfYear == y.DayOfYear;
    }

    public int GetHashCode(Day obj)
    {
        return obj.Year ^ obj.DayOfYear;
    }
}

public static class DateTimeExtension
{
    public static Day GetDay(this DateTime dateTime)
    {
        return new Day(dateTime.Year, dateTime.DayOfYear);
    }
}
