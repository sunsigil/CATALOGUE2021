using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Generates and maintains a mapping
/// of KeyCodes to InputCodes from an
/// XML control-scheme file
/// </summary>
public class ControlScheme
{
    Dictionary<InputCode, KeyCode> map;
    Dictionary<InputCode, string> string_map;

    /// <summary>
    /// Assume existing mapping
    /// </summary>
    /// <param name="map"></param>
    public ControlScheme(Dictionary<InputCode, KeyCode> map)
    {
        this.map = map;
    }

    /// <summary>
    /// Generate mapping from XML file
    /// </summary>
    /// <param name="scheme_file"></param>
    public ControlScheme(TextAsset scheme_file)
    {
        map = new Dictionary<InputCode, KeyCode>();
        string_map = new Dictionary<InputCode, string>();

        XDocument document = XDocument.Parse(scheme_file.text);
        XElement root = document.Root;

        foreach(XElement element in root.Elements())
		{
			string input_code_string = element.Name.LocalName;
			InputCode input_code = EnumTools.FromString<InputCode>(input_code_string);

            string key_code_string = element.Value;
            KeyCode key_code = EnumTools.FromString<KeyCode>(key_code_string);

            map.Add(input_code, key_code);
            string_map.Add(input_code, key_code_string);
		}
    }

    public KeyCode GetKeyCode(InputCode code){return map[code];}
    public string GetString(InputCode code){return string_map[code];}
}
