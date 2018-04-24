using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class MakuUtil : MonoBehaviour {
    public static System.Random rnd = new System.Random();
    static GameObject bloodTemplate;
    
    static MakuUtil FuckYouCSharp;
    
    void Awake() {
        FuckYouCSharp = this;
    }
    
    public static Dictionary<string,Dictionary<string,int>> LoadRecipeFile(string path) {
        Dictionary<string,Dictionary<string,int>> recipes = new Dictionary<string,Dictionary<string,int>>();
        //string path = "Assets/SettingsFiles/CraftingRecipes.txt";
        StreamReader reader = new StreamReader(path); 
        string currentLine;//
        while(true){
            currentLine = reader.ReadLine();
            if(currentLine != null){
                string toCreate = currentLine.Split('=')[0];
                string remainder = currentLine.Split('=')[1];
                string[] components = remainder.Split(',');
                recipes[toCreate] = new Dictionary<string,int>();
                for(int i=0;i<components.Length;i++) {
                    string comp = components[i];
                    int count = int.Parse(comp.Split('*')[0]);
                    string compMat = comp.Split('*')[1];
                    recipes[toCreate][compMat] = count;
                }
            }
            else{
                break;
            }
        }
        reader.Close();
        return(recipes);
    }
    
    
    public static void PlayBloodAt(Vector3 position) {
       FuckYouCSharp.StartCoroutine(PlayAndStopBloodAt(position));
    }
    static IEnumerator PlayAndStopBloodAt(Vector3 position) {
        bloodTemplate = bloodTemplate??Resources.Load("InWorld/Blood") as GameObject;
        GameObject blood = Instantiate(bloodTemplate);
        blood.transform.position = position;
        yield return new WaitForSeconds(.3f);
        Destroy(blood);
    }
}