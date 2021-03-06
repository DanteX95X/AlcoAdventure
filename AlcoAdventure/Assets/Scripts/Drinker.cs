﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Drinker//: //MonoBehaviour
{
	public static bool isInitialized = false;

	//user input required
	public static float bodyMass; //kg
	public static float bodyHeight; //cm
	public static bool isMale;
	public static bool isStomachEmpty = true;
	public static string myName = "";

	//calculated by program
	static float liquidMass;
	const float MAX_ALCOHOL_ELIMINATION = 10;
	static float alcoholDrunk = 0;
	static float alcoholAbsorbed = 0;
	static int absorptionTime; //in minutes
	static int absorptionProgress = 0;
	static float bmi;

	public static DateTime soberingTime;
	public static DateTime startTime;
	public static Dictionary<DateTime, float> alcoholFunction = new Dictionary<DateTime, float>();

	static float timePassed = 0;
	

	static void SimulateSobering()
	{
		float alcoholDrunkCopy = alcoholDrunk;
		float alcoholAbsorbedCopy = alcoholAbsorbed;
		int absorptioProgressCopy = absorptionProgress;

		startTime = DateTime.Now;
		alcoholFunction.Clear();

		double counter = 0;

		for( int i = 0; i < absorptionTime; ++i)
		{
			AbsorbAlcohol();
			EliminateAlcohol();
			alcoholFunction[startTime.AddMinutes(counter)] = CalculatePromils();
			++counter;
		}

		if (CalculatePromils() <= 0.2)
			counter = 0; //if you will never get drunk

		while(CalculatePromils() > 0.2)
		{
			AbsorbAlcohol();
			EliminateAlcohol();
			alcoholFunction[startTime.AddMinutes(counter)] = CalculatePromils();
			++counter;
		}

		soberingTime = startTime.AddMinutes(counter);

		alcoholAbsorbed = alcoholAbsorbedCopy;
		alcoholDrunk = alcoholDrunkCopy;
		absorptionProgress = absorptioProgressCopy;
	}
	
	public static string Initialize(float newBodyMass, float newBodyHeight, bool newIsMale, bool newIsStomachEmpty, string newName)
	{
		string message = "";

		bodyMass = newBodyMass;
		bodyHeight = newBodyHeight;
		isMale = newIsMale;
		isStomachEmpty = newIsStomachEmpty;
		myName = newName;

		message += CalculateBMI();
		message += CalculateAbsorptionTime();
		message += CalculateLiquids();

		isInitialized = true;

		return message;
	}

	public static string UpdateStatus()
	{
		string message = "";
		timePassed += Time.deltaTime;
		if (timePassed >= 60)
		{
			timePassed = 0;
			if (alcoholDrunk > 0)
				message += AbsorbAlcohol();
			if (alcoholAbsorbed > 0)
				message += EliminateAlcohol();
		}
		return message;
	}

	public static float CalculatePromils()
	{
		return alcoholAbsorbed / liquidMass;
	}

	public static string CalculateAbsorptionTime()
	{
		if (isStomachEmpty)
			absorptionTime = 30;
		else
			absorptionTime = 60;


		return("Absorption Time = " + absorptionTime);
	}
	
	public static string CalculateBMI()
	{
		bmi = bodyMass / ((bodyHeight/100) * (bodyHeight/100));
		return ("BMI = " + bmi);
	}

	static string CalculateLiquids()
	{
		float multiplier;
		if (isMale)
			multiplier = 0.68f;
		else
			multiplier = 0.55f;

		float overweight = CalculateOverweight();
		float liquidExcess = 0.15f * overweight;

		liquidMass = multiplier * (bodyMass - overweight) + liquidExcess;
		return (bodyMass - overweight + " " + liquidMass);
	}

	static float CalculateOverweight()
	{
		float bmiDifference;
		if (bmi > 25)
			bmiDifference = bmi - 25;
		else if (bmi < 18.5)
			bmiDifference = bmi - 18.5f;
		else
			bmiDifference = 0;

		return bmiDifference * ((bodyHeight / 100) * (bodyHeight / 100));
    }

	static string AbsorbAlcohol()
	{
		if (absorptionProgress < absorptionTime)
		{
			alcoholAbsorbed += alcoholDrunk / (absorptionTime - absorptionProgress);
			alcoholDrunk -= alcoholDrunk / (absorptionTime - absorptionProgress);
			++absorptionProgress;
		}
		else
			absorptionProgress = 0;
		return("absorbed " + alcoholAbsorbed + " \nProgress" + absorptionProgress);
	}

	static string EliminateAlcohol(float divider = 60f)
	{
		float alcoholEliminated = (MAX_ALCOHOL_ELIMINATION * alcoholAbsorbed) / (4.2f + alcoholAbsorbed) / divider;
		alcoholAbsorbed -= alcoholEliminated;
		if (alcoholAbsorbed < 0)
			alcoholAbsorbed = 0;
		return("eliminated " + alcoholEliminated + "remaining " + alcoholAbsorbed);
	}

	static public string Drink(float alcoholQuantity)
	{
		alcoholDrunk += alcoholQuantity;
		absorptionProgress = 0;

		SimulateSobering();
		return alcoholFunction[soberingTime.AddMinutes(-1)].ToString();  //"Sober by: " + soberingTime;
	}

}