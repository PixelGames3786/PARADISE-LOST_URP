using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleSceneReader : MonoBehaviour
{
    private BattleController BC;

    public bool GekihaWaiting;

    private List<string> EnemyAddress;

    //初期化
    public BattleSceneReader(BattleController bc)
    {
        BC = bc;

        EnemyAddressData Base=Resources.Load("DataBase/EnemyNames") as EnemyAddressData;

        EnemyAddress = Base.GetAddress();
    }

    //待ち系
    public void GeneralWaiting()
    {
        if (GekihaWaiting)
        {
            GekihaWait();
        }
    }

    private void GekihaWait()
    {
        //もしもう敵がいなかったら
        if (BC.EnemyParent.transform.childCount<=0)
        {
            GekihaWaiting = false;

            ReadLines(BC.NowIvent);
        }
    }

    //Readerの本体のようなもの
    public void ReadLines(BattleIvent Ivent)
    {
        if (Ivent.Index >= Ivent.Lines.Count) return;
        if (GekihaWaiting) return;

        var line = Ivent.Lines[Ivent.Index];

        if (line.Contains("#"))
        {
            //ここからイベントを一行ずつ実行
            while (true)
            {
                print(line);

                line = line.Replace("#", "");

                //いったん中止
                if (line.Contains("IventStop"))
                {
                    return;
                }

                //イベントを開始させる
                else if (line.Contains("AdIventStart"))
                {
                    line = line.Replace("AdIventStart=","");

                    var content = line.Split(',');

                    AdventureIvent AdIvent = BC.AC.NowScene.Ivents.Find((ivent) => ivent.ID == content[0]);

                    BC.AC.IventStart(AdIvent);

                    if (content[1]=="0")
                    {
                        Ivent.Index++;

                        break;
                    }
                }

                //バトルノベル始める
                else if (line.Contains("BattleNovelStart"))
                {
                    line = line.Replace("BattleNovelStart=","");

                    BC.BattleNovelStart(int.Parse(line));

                }

                //敵を全員倒すまで待つ
                else if (line.Contains("GekihaWait"))
                {
                    Ivent.Index++;

                    GekihaWaiting = true;

                    break;
                }
                //敵を全員動かす
                else if (line.Contains("EnemyAllMove"))
                {
                    for (int i = 0; i < BC.EnemyParent.transform.childCount; i++)
                    {

                        EnemyStateAbstract EnemyState = BC.EnemyParent.transform.GetChild(i).GetComponent<EnemyStateAbstract>();

                        EnemyState.State = EnemyStateAbstract.EnemyState.Idle;

                        EnemyState.StateChange(EnemyStateAbstract.EnemyState.Move);
                    }
                }
                //敵作成
                else if (line.Contains("MakeEnemy"))
                {
                    line = line.Replace("MakeEnemy=", "");

                    var content = line.Split(',');

                    //Resourceでの敵のプレハブがあるところを取得
                    string Address = EnemyAddress[int.Parse(content[0])];

                    GameObject EnemyPrefab = Resources.Load(Address) as GameObject;

                    GameObject Enemy;

                    float PositionX;

                    int EnemyNum = BC.EnemyParent.transform.childCount;

                    //設定した数だけ敵を作るよ
                    for (int i=0;i<int.Parse(content[1]);i++)
                    {
                        //敵のX位置を設定した値を元にランダムに生成
                        PositionX = UnityEngine.Random.Range(float.Parse(content[2]), float.Parse(content[3]));

                        Enemy = Instantiate(EnemyPrefab,BC.EnemyParent.transform);

                        Enemy.transform.position = new Vector3(PositionX,float.Parse(content[4]),0);

                        Enemy.GetComponent<SpriteRenderer>().sortingOrder = EnemyNum + i+1;
                    }

                    if (content[5]=="0")
                    {
                        Ivent.Index++;

                        break;
                    }
                }

                //キャラの動きを止める
                else if (line.Contains("CharaStop"))
                {
                    BC.BCC.CharactorStop();
                }
                //キャラが歩いてたり走ってたりするのをとめる
                else if (line.Contains("CharaInitialize"))
                {
                    BC.BCC.CharaInitialize();
                }

                //BGMを流す
                else if (line.Contains("BGMStart"))
                {
                    line = line.Replace("BGMStart=", "");

                    BGMController.Controller.BGMPlay(int.Parse(line));
                }

                //フラグ関連
                //フラグ追加
                else if (line.Contains("FlagAdd"))
                {
                    line = line.Replace("FlagAdd=", "");

                    var Content = line.Split(',');

                    //もうそのフラグがなかったら追加する あったら中身を変える
                    if (!SaveDataManager.DataManage.Data.GameFlags.ContainsKey(Content[0]))
                    {
                        SaveDataManager.DataManage.Data.GameFlags.Add(Content[0], Convert.ToBoolean(Content[1]));
                    }
                    else
                    {
                        SaveDataManager.DataManage.Data.GameFlags[Content[0]] = Convert.ToBoolean(Content[1]);
                    }

                }
                //フラグ削除
                else if (line.Contains("FlagDelete"))
                {
                    line = line.Replace("FlagDelete=", "");

                    //そのフラグを削除する
                    if (SaveDataManager.DataManage.Data.GameFlags.ContainsKey(line))
                    {
                        SaveDataManager.DataManage.Data.GameFlags.Remove(line);
                    }
                }
                //フラグ真偽確認
                else if (line.Contains("FlagCheck"))
                {
                    line = line.Replace("FlagCheck=", "");

                    //0:フラグ名前 1:true時移動 2:false時移動
                    var Content = line.Split(',');

                    //(0の場合は移動なし)

                    //そのキーが存在していたら
                    if (SaveDataManager.DataManage.Data.GameFlags.ContainsKey(Content[0]))
                    {
                        //true時
                        if (SaveDataManager.DataManage.Data.GameFlags[Content[0]])
                        {
                            if (Content[1] != "0")
                            {
                                Ivent.Index = Ivent.BattleIventScenes[Content[1]];
                            }
                        }
                        //false時
                        else
                        {
                            if (Content[2] != "0")
                            {
                                Ivent.Index = Ivent.BattleIventScenes[Content[2]];

                            }
                        }
                    }
                    //存在していない場合はfalse扱いにする
                    else
                    {
                        if (Content[2] != "0")
                        {
                            Ivent.Index = Ivent.BattleIventScenes[Content[2]];
                        }
                    }



                }

                //ゲームオブジェクトを破壊
                else if (line.Contains("DestroyObject"))
                {

                    line = line.Replace("DestroyObject=", "");

                    Destroy(GameObject.Find(line));
                }

                //壁を作る
                else if (line.Contains("MakeBattleWall"))
                {
                    line = line.Replace("MakeBattleWall=","");

                    var Content = line.Split(',');

                    BC.MakeBattleWall(Content);
                }
                //壁を壊す
                else if (line.Contains("DestroyBattleWall"))
                {
                    BC.DestroyBattleWall();
                }


                //バトル終了
                else if (line.Contains("BattleEnd"))
                {
                    BC.AC.Battleing = false;
                }

                //イベント終了
                else if (line.Contains("IventEnd"))
                {
                    Ivent.Index = 0;

                    return;
                }

                Ivent.Index++;

                line = Ivent.Lines[Ivent.Index];

            }
        }
        
    }
}
