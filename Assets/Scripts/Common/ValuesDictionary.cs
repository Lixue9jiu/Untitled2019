using UnityEngine;
using System.Collections.Generic;
using YamlSerializer;

public class ValuesDictionary
{
    public readonly string Tag;
    Dictionary<string, object> m_dictionary = new Dictionary<string, object>();

    public ValuesDictionary(string tag)
    {
        Tag = tag;
    }

    public object GetValue(string name)
    {
        if (m_dictionary.ContainsKey(name))
            return m_dictionary[name];
        return null;
    }

    public T GetValue<T>(string name)
    {
        return (T)GetValue(name);
    }

    public void SetValue(string name, object value)
    {
        m_dictionary[name] = value;
    }

    public static ValuesDictionary FromYAML(YamlNode node)
    {
        var map = node as YamlMapping;
        if (map != null)
        {
            var dict = new ValuesDictionary(node.Tag);
            foreach (YamlNode key in map.Keys)
            {
                var value = map[key];
                if (value is YamlMapping)
                {
                    dict.SetValue((key as YamlScalar).Value, FromYAML(value));
                }
                else
                {
                    dict.SetValue((key as YamlScalar).Value, (value as YamlScalar).Value);
                }
            }
            return dict;
        }
        return null;
    }
}
