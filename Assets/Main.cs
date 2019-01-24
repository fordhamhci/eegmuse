using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	MuseIO muse;

	public float rate = 1.618f;
	public float div = 8.9f;
	public float amp = 5f;
	public int sample = 5;

	private float highGamma = 0;
	private float lowGamma = 0;
	private float highBeta = 0;
	private float lowBeta = 0;
	private float highAlpha = 0;
	private float lowAlpha = 0;
	private float highDelta = 0;
	private float lowDelta = 0;
	private float highTheta = 0;
	private float lowTheta = 0;

	private string[] label = new string[5] { "Delta","Theta","Alpha","Beta","Gamma" };
	private string[] samples = new string[5] { "ambi_haunted_hum","ambi_drone","perc_bell","elec_twang","elec_tick" };

	private float timer;
	private float pacer;

	//private SessionData data;
	//private SessionLog log;
	private string input;
	private float[] avgVals = new float[] { 0, 0, 0, 0, 0};


	// Use this for initialization
	void Start () {
		Debug.Log ("Main Instance working...");
		muse = GetComponent<MuseIO> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (muse == null) {
			Debug.Log ("Muse is null...");
		} else {
			HandleData(muse.Absolute, muse.SignalStatus);
			//HandleConcentrationData(muse.Concentration, muse.SignalStatus);
		}
	}

	void HandleConcentration(float[] vals, int[] stat){
		
	}


	void HandleData(float[][] vals, int[] stat){
		string c;
		string eegtext = "";

		for (int i = 0; i < 5; ++i) {
			float R = 7f + i;
			float val = channelsAvg(vals[i], stat);
			avgVals [i] = val;
			int j = sample;
			c = "#" + ((9 - 2 * i) * 11).ToString() + "30" + (19 * i + 10).ToString() + "ff";
			float d = timer + i * 0.05f;

			//dots[i].transform.position = new Vector3 (R * Mathf.Cos(d), amp * val, R * Mathf.Sin(d));
			eegtext = "<color=" + c + ">" + label[i] + "</color>: " + val.ToString("F2") + "   " + eegtext;
		}
		VisualizeCompassion (avgVals);
		//Debug.Log (text);

		GameObject uitext = GameObject.Find("eegValText");
		//uitext.GetComponent<Text> ().text = "";
		uitext.GetComponent<Text> ().text = eegtext;
	}

	void VisualizeCompassion(float[] avgVals){

		//grab the relative values
		float gamma = avgVals [4];
		float beta = avgVals [3];
		float alpha = avgVals [2];
		float theta = avgVals [0];
		float delta = avgVals [1];
		//Debug.Log ("Beta["+beta+"] Delta["+delta+"]");

		//record highs and lows
		if(gamma > highGamma){highGamma=gamma;}
		if(gamma < lowGamma){lowGamma=gamma;}
		if(beta > highBeta){highBeta=beta;}
		if(beta < lowBeta){lowBeta=beta;}
		if(alpha > highAlpha){highAlpha=alpha;}
		if(alpha < lowAlpha){lowAlpha=alpha;}
		if(theta > highTheta){highTheta=theta;}
		if(theta < lowTheta){lowTheta=theta;}
		if(delta > highDelta){highDelta=delta;}
		if(delta < lowDelta){lowDelta=delta;}
		GameObject uiHighLowtext = GameObject.Find("highlowText");
		string highLowTempText = "Gamma High["+highGamma+"] Gamma Low["+lowGamma+"]";
		//highLowTempText += "Beta High["+highBeta+"] Beta Low["+lowBeta+"]";
		//highLowTempText += "Alpha High["+highAlpha+"] Alpha Low["+lowAlpha+"]";
		//highLowTempText += "Theta High["+highTheta+"] Theta Low["+lowTheta+"]";
		//highLowTempText += "Delta High["+highDelta+"] Delta Low["+lowDelta+"]";

		//grab compassion average
		double deltaAdj = (double)ConvertRange (-0.3, 0.4, 0, 1, delta);
		double alphaAdj = (double)ConvertRange (-0.4, 1.3, 0, 1, alpha);
		double betaAdj = (double)ConvertRange (-0.8, 1.3, 0, 1, beta);
		//double gammaAdj = (double)ConvertRange (-.4, 2.5, 0, 1, gamma);
		double gammaAdj = gamma;
		double compAvgVisual = gammaAdj;

		highLowTempText += "</br>Gamma: " + gammaAdj;

		uiHighLowtext.GetComponent<Text> ().text = highLowTempText;

		//double arousal = (beta - theta) / (beta + theta);
		//compAvgVisual = arousal;
		//uiHighLowtext.GetComponent<Text> ().text = compAvgVisual.ToString();
		//uiHighLowtext.GetComponent<Text> ().text = gammaAdj.ToString();


		//CONCENTRATION TEST
		//float concentrationVals = muse.Concentration;
		//uiHighLowtext.GetComponent<Text> ().text = "CONCENTRATION: " + concentrationVals.ToString ();
		//uiHighLowtext.GetComponent<Text> ().text = "SIGNAL: " + 
		//compAvgVisual = concentrationVals;

		/*
		float compAvg = (delta + gamma + beta)/3;
		compAvg = delta;
		Debug.Log ("COMPAVG: " + compAvg);

		//change the range
		double originalStart = .11;
		double originalEnd = .20;
		double newStart = 0;
		double newEnd = 1;
		double value = (double)compAvg;
		double compAvgVisual = (double)ConvertRange (originalStart, originalEnd, newStart, newEnd, value);

		Debug.Log ("AvgVisual: " + compAvgVisual);
		*/

		//create a color threshold
		//Color lerpedColor = Color.white;
		//Color32 colorStart = new Color(0, 0, compAvgVisual, 1);
		//Color32 colorEnd = new Color(compAvgVisual, 0, 0, 1);
		//lerpedColor = Color.Lerp(colorStart, colorEnd,0);

		float redVal = (float)compAvgVisual;
		float blueVal = (float)(1 - compAvgVisual);
		if (blueVal < 0) {
			blueVal = 0;
		}

		Color currentColor = new Color (redVal, 0, blueVal, 1);

		//apply color to ball
		GameObject sphere = GameObject.Find("Sphere");
		if (muse.Blink == false) {
			sphere.GetComponent<Renderer> ().material.color = currentColor;
		}

	}

	public static double ConvertRange(
		double originalStart, double originalEnd, // original range
		double newStart, double newEnd, // desired range
		double value) // value to convert
	{
		double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
		return (double)(newStart + ((value - originalStart) * scale));
	}

	float channelsAvg(float[] vals, int[] stat) {
		float norm = 0f;
		float val = 0f;
		for (int j = 0; j < 4; ++j) {
			if (stat [j] > 0) { //SignalStatus
				//if (stat [j] < 3) { //HorseShoe
				val += vals [j];
				norm += 1f;
			}
		}

		if (norm == 0)
			return 0f;

		return val/norm;
	}



}