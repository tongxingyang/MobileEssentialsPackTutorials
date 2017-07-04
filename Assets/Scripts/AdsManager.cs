//This script handles showing advertisements and then rewarding the player.


//Since the ad test image will only show if the Unity editor is set to build for
//iOS or Android, we need to simulate the ad being shown on any other platform

using UnityEngine;

public class AdsManager : MonoBehaviour 
{
	//This method will be called by the "Watch Ad" button on the Loss Screen
	public void ShowRewardedAd()
	{

		//Write that we are simulating to the console
		Debug.Log("Build platform is not set to iOS or Android. Simulating Ad view");

		if (GameManager.instance != null)
			GameManager.instance.AddMoreGameTime (30f);
	}
}
