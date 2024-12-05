using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkinHandler {

    private string ballSkin;
    private string[] unlockedSkins;
    private Dictionary<int, string> skinUnlockRequirement = new Dictionary<int, string>();

    // ca_ballSkin -> the skin that will be first loaded
    // ca_unlockedSkins -> string array that holds the unlocked skins
    public SkinHandler(string ca_ballSkin, string[] ca_unlockedSkins) {
        this.ballSkin = ca_ballSkin;
        this.unlockedSkins = ca_unlockedSkins;
        
        // Sets the check mark to the currently selected skin
        GameObject checkmark = GameObject.Find("CheckMarkImage");
        if(checkmark != null) {
            checkmark.transform.position = GameObject.Find(ca_ballSkin+"BasketballImage").transform.position;
        }

        // A loop that removes all of the lock images form ball skins and replaces the unlock requirements with the individual skin names
        for (int i = 1; i < this.unlockedSkins.Length; i++) {
            string _skinLockImageName = this.unlockedSkins[i]+"LockImage";
            string _skinUnlockTextName = this.unlockedSkins[i]+"UnlockText";

            GameObject lockImage = GameObject.Find(_skinLockImageName);
            GameObject unlockText = GameObject.Find(_skinUnlockTextName);

            if(lockImage != null) {
                lockImage.SetActive(false);
            }

            if(unlockText != null) {
                unlockText.GetComponent<TMP_Text>().text = this.unlockedSkins[i];
            }
        }

        // Building the dictionary with the scores required for unlock (key; left) and skin names (value ; right)
        skinUnlockRequirement.Add(10, "Pink");
        skinUnlockRequirement.Add(20, "Green");
        skinUnlockRequirement.Add(30, "Blue");
        skinUnlockRequirement.Add(40, "Red");
    }

    // Checks wether the input score unlocks a ball skin
    // a_score -> int value representing a score
    // returns a boolean representing wether the input score unlocs a skin
    public bool CanUnlockNewSkin(int a_score) {
        return this.skinUnlockRequirement.ContainsKey(a_score);
    }

    // For getting a skin name based on input score
    // a_score -> int ; representing a score
    // returns a string representing a skin name
    public string GetSkinFromScore(int a_score) {
        if(this.CanUnlockNewSkin(a_score)) {
            return this.skinUnlockRequirement[a_score];
        }
        return null;
    }

    //  Method used for unlocking skins
    // a_skin -> string ; the name of a skin
    // returns a string representing the name of the unlocked skin
    public string UnlockSkin(string a_skin) {
        if(a_skin == null) {
            return "";
        }
        
        // So that the skin does not get pushed multiple times
        if(this.IsSkinUnlocked(a_skin)) {
            return "";
        }
        
        // Code for pushing a new value to the array
        Array.Resize(ref this.unlockedSkins, this.unlockedSkins.Length + 1);
        this.unlockedSkins[this.unlockedSkins.GetUpperBound(0)] = a_skin;
        Debug.Log("Unlocked the '" + a_skin + "' skin");        

        return a_skin;
    }

    //  Method used for checking wether a skin is already unlocked
    // a_skin -> string ; the name of a skin
    // returns a boolean representing wether the input skin is unlocked
    public bool IsSkinUnlocked(string a_skin) {
        return Array.IndexOf(this.unlockedSkins, a_skin) != -1;
    }

    //  Method used for changing the current ball skin
    // a_skin -> string ; the name of a skin
    public void ChangeBallSkin(string a_skin) {
        if(!this.IsSkinUnlocked(a_skin)) {
            Debug.Log("You havent yet unlocked the '" + a_skin + "' skin");
            return;
        }
        this.ballSkin = a_skin;
        GameObject.Find("CheckMarkImage").transform.position = GameObject.Find(a_skin+"BasketballImage").transform.position;
        Game.skinChanged = false;
    }

    //  Getter for the current skin name
    //  returns a string
    public string GetBallSkin() {
        return this.ballSkin;
    }

    //  Getter for the unlocked skins array
    //  returns a string array
    public string[] GetUnlockedSkins() {
        return this.unlockedSkins;
    }

    //  Getter for the skin unlock requirements dictionary
    //  returns a dictionary
    public Dictionary<int, string> GetSkinUnlockRequirements() {
        return this.skinUnlockRequirement;
    }
}
