using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NovelSceneHolder
{
    public List<NovelScene> NovelScenes = new List<NovelScene>();

    private TextController TC;

    public NovelSceneHolder(TextController TC)
    {
        this.TC = TC;

        NovelSceneLoad();
    }

    public void NovelSceneLoad()
    {
        //var itemFile = Resources.Load("Texts/Scenario") as TextAsset;
        var csvData = LoadCSV(TC.NC.Scenario);

        NovelScenes = Parse(csvData);
    }

    public List<string> LoadCSV(TextAsset file)
    {
        StringReader reader = new StringReader(file.text);

        var list = new List<string>();

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();

            list.Add(line);
        }
        return list;
    }

    public List<NovelScene> Parse(List<string> list)
    {
        var Novelscenes = new List<NovelScene>();
        var Novelscene = new NovelScene();

        foreach (string line in list)
        {
            if (line.Contains("#scene"))
            {
                var ID = line.Replace("#scene=", "");

                Novelscene = new NovelScene(ID);
                Novelscenes.Add(Novelscene);
            }
            else
            {
                Novelscene.Lines.Add(line);
            }
        }

        return Novelscenes;
    }
}
