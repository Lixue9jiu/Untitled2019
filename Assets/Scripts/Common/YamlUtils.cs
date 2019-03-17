using System;
using System.Reflection;
using System.Collections.Generic;
using YamlSerializer;

public static class YamlUtils
{
    private static Dictionary<Type, Func<YamlNode, object>> converters;

    static YamlUtils()
    {
        converters = new Dictionary<Type, Func<YamlNode, object>> {
            {typeof(string), (n) => (n as YamlScalar).Value},
            {typeof(bool), (n) => bool.Parse((n as YamlScalar).Value)},
            {typeof(int), (n) => int.Parse((n as YamlScalar).Value)},
            {typeof(float), (n) => float.Parse((n as YamlScalar).Value)},
            {typeof(ValuesDictionary), (n) => ValuesDictionary.FromYAML(n)}
        };
    }

    public static List<BlockDataGroup> LoadBlockGroups(string str)
    {
        Dictionary<string, YamlMapping> templates = new Dictionary<string, YamlMapping>();
        List<BlockDataGroup> groups = new List<BlockDataGroup>();
        YamlSequence seq = (YamlSequence)YamlNode.FromYaml(str)[0];
        for (int i = 0; i < seq.Count; i++)
        {
            YamlMapping node = seq[i] as YamlMapping;
            switch(seq[i].Tag)
            {
                case "!templates":
                    LoadTemplates(node, templates);
                    break;
                case "!blockGroup":
                    ApplyTemplates(templates, node);
                    groups.Add(Deserialize<BlockDataGroup>(node));
                    break;
            }
        }
        return groups;
    }

    public static T Deserialize<T>(YamlNode map)
    {
        return (T)DeserializeNode(map, typeof(T));
    }

    private static object DeserializeNode(YamlNode node, Type type)
    {
        if (converters.ContainsKey(type))
            return converters[type].Invoke(node);
        if (node is YamlMapping)
            return DeserializeMap(node as YamlMapping, type);
        if (node is YamlSequence)
            return DeserializeSeq(node as YamlSequence, type);
        throw new Exception("cannot serialize node " + node);
    }

    private static object DeserializeSeq(YamlSequence seq, Type type)
    {
        if (!type.IsArray)
            return null;
        Array obj = (Array)Activator.CreateInstance(type, seq.Count);
        for (int i = 0; i < seq.Count; i++)
        {
            obj.SetValue(DeserializeNode(seq[i], type.GetElementType()), i);
        }
        return obj;
    }

    private static object DeserializeMap(YamlMapping map, Type type)
    {
        object obj = Activator.CreateInstance(type);
        FieldInfo[] infos = type.GetTypeInfo().GetFields();
        foreach (FieldInfo f in infos)
        {
            if (map.ContainsKey(f.Name))
            {
                f.SetValue(obj, DeserializeNode(map[f.Name], f.FieldType));
            }
        }
        return obj;
    }

    private static void LoadTemplates(YamlMapping templates, Dictionary<string, YamlMapping> target)
    {
        foreach (YamlNode n in templates.Keys)
        {
            if (target.ContainsKey(templates[n].Tag))
                ApplyTemplates(target, templates[n]);
            target['!' + Deserialize<string>(n)] = templates[n] as YamlMapping;
        }
    }

    private static void ApplyTemplates(Dictionary<string, YamlMapping> templates, YamlNode target)
    {
        if (target is YamlScalar)
            return;
        if (target is YamlMapping)
        {
            var map = target as YamlMapping;
            foreach (YamlNode node in map.Values)
            {
                ApplyTemplates(templates, node);
            }
            if (templates.ContainsKey(target.Tag))
            {
                ApplyTemplate(templates[target.Tag], map);
            }
        }
        else if (target is YamlSequence)
        {
            foreach (YamlNode node in (YamlSequence)target)
            {
                ApplyTemplates(templates, node);
            }
        }
    }

    private static void ApplyTemplate(YamlMapping template, YamlMapping target)
    {
        foreach (YamlNode n in template.Keys)
        {
            if (!target.ContainsKey(n))
            {
                target[n] = template[n];
            }
        }
    }
}