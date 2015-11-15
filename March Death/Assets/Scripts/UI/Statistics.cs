﻿using UnityEngine;
using System.Collections;
using Utils;

public class Statistics
{
    public WorldResources.Type _type { get; private set; }
    public int _time { private get; set; }
    public int _amount { private get; set; }

    private float _growth_speed;
    public float growth_speed {
        get { return ((float)_amount / (float)_time); }
        set { _growth_speed = value; }
    }

    /// <summary>
    /// Statistics constructor. time and amount are 0 by default.
    /// </summary>
    /// <param name="type">Type of the Resources.</param>
    public Statistics(WorldResources.Type type)
    {
        _time = 1;
        _amount = 0;
        _type = type;
    }

    /// <summary>
    /// Ststistics constructor.
    /// </summary>
    /// <param name="type">Type of the Resources.</param>
    /// <param name="time">Time of refresh.</param>
    /// <param name="amount">Amount of resource for each refresh cycle,</param>
    public Statistics(WorldResources.Type type, int time, int amount)
    {
        _time = time;
        _amount = amount;
        _type = type;
    }


    public static Statistics operator +(Statistics self, Statistics other)
    {
        int lcm = LCM(self._time, other._time);

        self._amount = (int)(((float)((lcm/(float)self._time) * self._amount)) + ((float)((lcm / (float)other._time) * other._amount)));
        self._time = lcm;

        return self;
    }

    public static Statistics operator -(Statistics self, Statistics other)
    {
        int lcm = LCM(self._time, other._time);

        self._amount = (int)(((float)((lcm / (float)self._time) * self._amount)) - ((float)((lcm / (float)other._time) * other._amount)));
        self._time = lcm;

        return self;
    }

    // Assistant methods

    private static int LCM(int a, int b)
    {
        int num1, num2;

        if (a > b)
        {
            num1 = a; num2 = b;
        }
        else
        {
            num1 = b; num2 = a;
        }

        for (int i = 1; i <= num2; i++)
        {
            if ((num1 * i) % num2 == 0)
            {
                return i * num1;
            }
        }

        return num2;
    }

}
