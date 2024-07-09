using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class NovelScene
{
    public string ID { get; private set; }

    public List<string> Lines { get; private set; } = new List<string>();

    public int Index { get; set; } = 0;

    public NovelScene(string ID = "")
    {
        this.ID = ID;
    }

    public NovelScene Clone()
    {
        return new NovelScene(ID)
        {
            Index = 0,
            Lines = new List<string>(Lines)
        };
    }

    public bool IsFinished()
    {
        return Index >= Lines.Count;
    }

    public string GetCurrentLine()
    {
        return Lines[Index];
    }

    public void GoNextLine()
    {
        Index++;
    }
}
