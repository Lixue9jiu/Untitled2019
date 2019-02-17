using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class Scanner
{
    public char[] Tokens
    {
        set
        {
            m_token = value;
        }
    }

    private string m_text;
    private int m_position;

    private char[] m_token;

    public Scanner(string text) : this(text, new char[]{' ', '\n'})
    {
    }

    public Scanner(string text, char[] tokens)
    {
        m_text = text;
        m_token = tokens;
    }

    private string Read(char[] tokens)
    {
        StringBuilder builder = new StringBuilder();
        while (m_text.Length > m_position)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                if (m_text[m_position] == tokens[i])
                    continue;
            }
            builder.Append(m_text[m_position]);
            m_position++;
        }
        return builder.ToString();
    }

    public string Read()
    {
        return Read(m_token);
    }

    public bool ReadBool()
    {
        return bool.Parse(Read());
    }

    public int ReadInt()
    {
        return int.Parse(Read());
    }

    public float ReadFloat()
    {
        return float.Parse(Read());
    }

    public string ReadLine()
    {
        return Read(new char[] { '\n' });
    }
}
