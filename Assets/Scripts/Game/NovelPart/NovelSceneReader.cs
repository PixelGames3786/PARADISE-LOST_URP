using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NovelSceneReader 
{
    private TextController TC;

    public NovelSceneReader(TextController TC)
    {
        this.TC = TC;
    }

    public void ReadLines(NovelScene Scene)
    {
        if (Scene.Index >= Scene.Lines.Count) return;

        var line = Scene.GetCurrentLine();
        var text = "";

        if (line.Contains("#"))
        {
            while (true)
            {
                if (!line.Contains("#")) break;

                line = line.Replace("#", "");

                //しゃべっている人をセット
                if (line.Contains("speaker"))
                {
                    line = line.Replace("speaker=", "");
                    TC.SetSpeaker(line);
                }
                //背景をセット
                else if (line.Contains("background"))
                {
                    line = line.Replace("background=", "");

                    TC.SetBackGround(line);
                }
                //シーンチェンジ時の背景色設定
                else if (line.Contains("changeback"))
                {
                    line = line.Replace("changeback=","");

                    var BackColor = line.Split(',').Select(float.Parse).ToArray();

                    TC.BackColor = new Color(BackColor[0], BackColor[1], BackColor[2], BackColor[3]);
                }
                //シーンチェンジ設定
                else if (line.Contains("change"))
                {
                    BGMController.Controller.BGMFadeOut();

                    line = line.Replace("change=", "");

                    var SceneChange = line.Split(',');

                    SaveDataManager.DataManage.Data.AdventureScene = SceneChange[0];

                    SceneFader.FadeType FadeType;

                    Enum.TryParse(SceneChange[2],out FadeType);

                    if (SceneChange[1] == "Adventure")
                    {
                        TC.NC.SaveManege.Data.Scene = SaveData.SceneType.Adventure;

                        SceneController.CharaStatus Status;
                        Enum.TryParse(SceneChange[3], out Status);

                        bool trans = bool.Parse(SceneChange[7]);

                        List<string> Others = new List<string>();

                        if (SceneChange.Length > 8)
                        {
                            for (int i = 8; i < SceneChange.Length - 1; i++)
                            {
                                Others.Add(SceneChange[i]);
                            }
                        }

                        TC.ChangeToAdventure(SceneChange[0],FadeType,Status,int.Parse(SceneChange[4]),int.Parse(SceneChange[5]),int.Parse(SceneChange[6]),trans,Others);
                    }
                    else if (SceneChange[1] == "Novel")
                    {
                        TC.NC.SaveManege.Data.Scene = SaveData.SceneType.Novel;
                    }

                    TC.SetSceneChange(SceneChange[0],FadeType);
                }
                else if (line.Contains("image"))
                {
                    line = line.Replace("image_", "");
                    var splitted = line.Split('=');
                    //TC.SetImage(splitted[0], splitted[1]);
                }
                else if (line.Contains("next"))
                {
                    line = line.Replace("next=", "");
                    //TC.SetTCene(line);
                }
                else if (line.Contains("options"))
                {
                    /*
                    var options = new List<(string, string)>();
                    while (true)
                    {
                        s.GoNextLine();
                        line = line = s.Lines[s.Index];
                        if (line.Contains("{"))
                        {
                            line = line.Replace("{", "").Replace("}", "");
                            var splitted = line.Split(',');
                            options.Add((splitted[0], splitted[1]));
                        }
                        else
                        {
                            TC.SetOptions(options);
                            break;
                        }
                    }
                    */
                }

                Scene.GoNextLine();

                if (Scene.IsFinished()) break;

                line = Scene.GetCurrentLine();
            }
        }

        if (line.Contains('{'))
        {
            if (!TC.CanText)
            {
                return;
            }

            line = line.Replace("{", "");

            while (true)
            {
                if (line.Contains('}'))
                {
                    line = line.Replace("}", "");

                    text += line;

                    Scene.GoNextLine();

                    break;
                }
                else
                {
                    text += line;
                }

                Scene.GoNextLine();

                if (Scene.IsFinished()) break;

                line = Scene.GetCurrentLine();
            }

            TC.Texting = true;
            TC.NC.Text.text = "";

            TC.HyoujiText = text;

            //if (!string.IsNullOrEmpty(text)) sc.SetText(text);
        }
    }
}
