#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public class TextGenerator : MonoBehaviour
{
    public List<GameObject> characters;

    public string inputWord;
    
    [Range(0f, 1.5f), SerializeField]
    private float spacing = 0.7f;

    private Dictionary<string, int> dictionary = new Dictionary<string, int>(){
            { "a", 0},
            { "b", 1},
            { "c", 2},
            { "d", 3},
            { "e", 4},
            { "f", 5},
            { "g", 6},
            { "h", 7},
            { "i", 8},
            { "j", 9},
            { "k", 10},
            { "l", 11},
            { "m", 12},
            { "n", 13},
            { "o", 14},
            { "p", 15},
            { "q", 16},
            { "r", 17},
            { "s", 18},
            { "t", 19},
            { "u", 20},
            { "v", 21},
            { "w", 22},
            { "x", 23},
            { "y", 24},
            { "z", 25},
            { "A", 26},
            { "B", 27},
            { "C", 28},
            { "D", 29},
            { "E", 30},
            { "F", 31},
            { "G", 32},
            { "H", 33},
            { "I", 34},
            { "J", 35},
            { "K", 36},
            { "L", 37},
            { "M", 38},
            { "N", 39},
            { "O", 40},
            { "P", 41},
            { "Q", 42},
            { "R", 43},
            { "S", 44},
            { "T", 45},
            { "U", 46},
            { "V", 47},
            { "W", 48},
            { "X", 49},
            { "Y", 50},
            { "Z", 51},
            { "0", 52},
            { "1", 53},
            { "2", 54},
            { "3", 55},
            { "4", 56},
            { "5", 57},
            { "6", 58},
            { "7", 59},
            { "8", 60},
            { "9", 61},
            { "!", 62},
            { "'", 63},
            { ",", 64},
            { ".", 65},
            { "?", 66}
        };

    [MyBox.ButtonMethod]
    private void GenerateWord()
    {
        GameObject word = new GameObject(); //characters parent
        string name = "newWord";
        if (inputWord.Length > 12) name = inputWord.Substring(0, 10) + "...";
        else name = inputWord;
        word.transform.SetParent(this.transform);
        word.name = name;
        int position = 0;
        foreach(char c in inputWord){ //cycle input characters
            int value;
            bool hasValue = dictionary.TryGetValue(c.ToString(), out value);
            if (hasValue)
            {
                GameObject character = characters[value]; 
                character.name = c.ToString();
                character.transform.localPosition = new Vector3(position * spacing * -1,0f,0f); //apply spacing
                Instantiate<GameObject>(character, word.transform); //instatiate character object
            }
            else
            {
                Debug.Log("Key not present");
            }
            position++;
        }
    }

}

#endif