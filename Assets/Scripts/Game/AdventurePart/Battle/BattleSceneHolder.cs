using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneHolder 
{
    private BattleController BC;

    //バトルシーン置き場
    public List<BattleScene> BattleScenes = new List<BattleScene>();

    //バトルノベルシーン置き場
    public List<BattleNovelIvent> BattleNIvents = new List<BattleNovelIvent>();


    public BattleSceneHolder(BattleController bc)
    {
        BC = bc;

        BattleSceneLoad();
    }

    public void BattleSceneLoad()
    {
        var csvdata = LoadCSV(BC.AC.BattleTextAsset);
        var Ncsvdata = LoadCSV(BC.AC.BattleNovelAsset);

        BattleScenes = BSParse(csvdata);
        BattleNIvents = BNIParse(Ncsvdata);
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

    public List<BattleScene> BSParse(List<string> list)
    {
        var BattleScenes = new List<BattleScene>();
        var BattleScene = new BattleScene();
        var BattleIvent = new BattleIvent();

        int Count = 0;

        foreach (string line in list)
        {
            if (line.Contains("#BattleScene"))
            {
                var ID = line.Replace("#BattleScene=", "");

                BattleScene = new BattleScene(ID);
                BattleScenes.Add(BattleScene);
            }
            else if (line.Contains("#btivent"))
            {
                var ID = line.Replace("#btivent=", "");

                BattleIvent = new BattleIvent(ID);

                BattleScene.Ivents.Add(BattleIvent);
            }
            else if (line.Contains("#btscene"))
            {
                var ID = line.Replace("#btscene=", "");

                BattleIvent.BattleIventScenes.Add(ID, BattleIvent.Lines.Count - 1);
            }
            else if (line.Contains("#BattleNovelText"))
            {
                var Novel = line.Replace("#BattleNovelText=", "");

                BattleIvent.BattleMessages.Add(Novel);
            }
            else if (BattleIvent.ID != "" && line != "")
            {
                BattleIvent.Lines.Add(line);
            }

            Count++;
        }

        return BattleScenes;
    }

    public List<BattleNovelIvent> BNIParse(List<string> list)
    {
        var BattleNIvents = new List<BattleNovelIvent>();
        var BattleNIvent = new BattleNovelIvent();

        foreach (string line in list)
        {
            if (line.Contains("#BattleNovelIvent"))
            {
                var ID = line.Replace("#BattleNovelIvent=","");

                BattleNIvent = new BattleNovelIvent(ID);
                BattleNIvents.Add(BattleNIvent);
            }
            else if (BattleNIvent.ID != "" && line != "")
            {
                BattleNIvent.Lines.Add(line);
            }
        }

        return BattleNIvents;
    }
}
