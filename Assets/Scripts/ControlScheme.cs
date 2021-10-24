using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ControlScheme
{
    Dictionary<InputCode, KeyCode> map;

    public ControlScheme(Dictionary<InputCode, KeyCode> map)
    {
        this.map = map;
    }

    public ControlScheme(XDocument document)
    {
        map = new Dictionary<InputCode, KeyCode>();

        XElement root = document.Root;

        foreach(XElement element in root.Elements())
		{
			string inputCodeString = element.Name.LocalName;
			InputCode inputCode = CowTools.StringToEnum<InputCode>(inputCodeString);

            string keyCodeString = element.Value;
            KeyCode keyCode = CowTools.StringToEnum<KeyCode>(keyCodeString);

            map.Add(inputCode, keyCode);
		}
    }

    public string ToString()
    {
        string result = "";

        foreach(InputCode key in map.Keys)
        {
            result += $"{key} : {map[key]}\n";
        }

        return result;
    }

    public KeyCode Convert(InputCode code){return map[code];}
}
