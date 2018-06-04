using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class MakuUtil : MonoBehaviour {
    public static System.Random rnd = new System.Random();
    static GameObject bloodTemplate;
    
    static MakuUtil FuckYouCSharp;
    
    static Dictionary<string,GameObject> parts = new Dictionary<string,GameObject>();
    
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
    
    /*public static Dictionary<string,Item> LoadItems() {
        string path = "Assets/Scripts/Items.yml";
        StreamReader reader = new StreamReader(path);
        Dictionary<string,Item> items = new Dictionary<string,Item>();
        string nextLine = reader.ReadLine();
        while(nextLine != null) {
            //int level = nextLine.TakeWhile(Char.IsWhiteSpace).Count();
            nextLine = nextLine.Trim();
            int separationIndex = nextLine.IndexOf(':');
            string simpleName = nextLine.Substring(separationIndex);
            items[simpleName] = Item;
            items[simpleName].simpleName = simpleName;
            
            nextLine = reader.ReadLine();
            while(nextLine != null && nextLine.TakeWhile(Char.IsWhiteSpace).Count() == 1) {
                nextLine = nextLine.Trim();
                int separationIndex = nextLine.IndexOf(": ");
                string propName = nextLine.Split(": ")[0].Trim();
                string propVal = nextLine.Substring(separationIndex+2,nextLine.Length).Replace("\"","").Trim();
                
                if(propName == "itemType") {
                    items[simpleName].itemType = propVal;
                }
                else if (propName == "nameUppercased") {
                    items[simpleName].nameUppercased = propVal;
                }
                else if (propName == "ingredients") {
                    propVal = propVal.Replace("{","").Replace("}","").Trim();
                    string[] ingredientPairs = propVal.Split(", ");
                    Dictionary<string, int> ingredients = new Dictionary<string,int>();
                    foreach(string ingredientPair in ingredientPairs) {
                        string component = ingredientPair.Split(": ")[0];
                        int count = Int32.ParseInt(ingredientPair.Split(": ")[1]);
                        ingredients[component] = count;
                    }
                    items[simpleName].ingredients = ingredients;
                }
                
                
                nextLine = reader.ReadLine();
            }
        }
    }*/
    
    public static void PlayBloodAtPoint(Vector3 position) {
       PlayParticlesAtPoint("Blood",position);
    }
    public static void PlayParticlesAtPoint( string particlesName,Vector3 position) {
        if(!parts.ContainsKey(particlesName)){
            parts[particlesName] = Instantiate(Resources.Load("InWorld/"+particlesName) as GameObject);
            parts[particlesName].SetActive(false);
        }
        FuckYouCSharp.StartCoroutine(PlayAndStopParticlesAtPoint(parts[particlesName],position));
    }
    static IEnumerator PlayAndStopParticlesAtPoint(GameObject particles,Vector3 position) {
        particles.SetActive(true);
        particles.transform.position = position;
        yield return new WaitForSeconds(.3f);
        particles.SetActive(false);
    }
    
    public static void ExecuteLine(string s) {
        //TODO execute s as a command
    }
}