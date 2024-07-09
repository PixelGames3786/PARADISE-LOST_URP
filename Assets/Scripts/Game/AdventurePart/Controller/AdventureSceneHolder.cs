using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdventureSceneHolder
{
    private AdventureController AC;

    public List<AdventureScene> AdventureScenes = new List<AdventureScene>();

    public AdventureSceneHolder(AdventureController Controller)
    {
        AC = Controller;

        AdventureSceneLoad();
    }

    public void AdventureSceneLoad()
    {
        var csvdata = LoadCSV(AC.IventTextAsset);

        AdventureScenes = Parse(csvdata);

        AC.NowScene= AdventureScenes.Find((scene) => scene.ID == SceneManager.GetActiveScene().name);
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

    public List<AdventureScene> Parse(List<string> list)
    {
        var AdventureScenes = new List<AdventureScene>();
        var AdventureScene = new AdventureScene();
        var AdventureIvent = new AdventureIvent();

        int Count=0;

        foreach (string line in list)
        {

            if (line.Contains("#adscene"))
            {
                var ID=line.Replace("\t","");

                ID = ID.Replace("#adscene=", "");

                AdventureScene = new AdventureScene(ID);
                AdventureScenes.Add(AdventureScene);
            }
            else if (line.Contains("#title"))
            {
                var Line = line.Replace("\t","");
                Line = Line.Replace("#title=", "");

                var Name = Line.Split(',');

                AdventureScene.Name = Name;
            }
            else if (line.Contains("#target"))
            {
                var Target = line.Replace("\t","");
                Target = Target.Replace("#target=", "");

                AdventureScene.Targets.Add(Target);
            }
            else if (line.Contains("#ivent"))
            {
                var ID = line.Replace("\t","");
                ID = ID.Replace("#ivent=", "");

                AdventureIvent = new AdventureIvent(ID);

                AdventureScene.Ivents.Add(AdventureIvent);
            }
            else if (line.Contains("#ivscene"))
            {
                var ID = line.Replace("\t","");
                ID = ID.Replace("#ivscene=", "");

                AdventureIvent.IventScene.Add(ID, AdventureIvent.Lines.Count - 1);
            }
            else if (AdventureIvent.ID != "" && line != "")
            {
                AdventureIvent.Lines.Add(line);
            }

            Count++;
        }

        return AdventureScenes;
    }



}
