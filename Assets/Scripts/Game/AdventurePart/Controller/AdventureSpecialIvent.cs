using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ParticleSystemJobs;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class AdventureSpecialIvent : MonoBehaviour
{
    public AdventureController AC;

    public AdventureIventReader AIR;

    private Volume volume;

    [System.NonSerialized]
    public string IventScene;

    [System.NonSerialized]
    public string Others;

    [System.NonSerialized]
    public int IventNumber;

    [System.NonSerialized]
    public Transform HanyouTrans;

    IEnumerator SpecialIvent()
    {
        switch (IventScene)
        {
            case "SchoolZone":

                {
                    switch (IventNumber)
                    {
                        //SchoolZone ダンジョンにチェンジ1
                        case 1:

                            {
                                AC.Audio.PlayClips(0);

                                volume = FindObjectOfType<Volume>();

                                AC.CC.CanMove = false;

                                AC.CC.CharaInitialize();

                                //HDRPを変える
                                if (volume.profile.TryGet(out FilmGrain Grain))
                                {
                                    FilmGrain Gra = Grain;

                                    Grain.intensity.SetValue(new ClampedFloatParameter(1f, 0f, 1f, true));
                                }

                                yield return new WaitForSeconds(1f);

                                AC.Audio.Stop();

                                //HDRPを元に戻す
                                Grain.intensity.SetValue(new ClampedFloatParameter(0f, 0f, 1f, true));

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //SchoolZone ダンジョンにチェンジ2
                        case 2:

                            {
                                //音を止めてノイズを流す
                                BGMController.Controller.BGMPause();
                                AC.Audio.PlayClips(0);

                                volume = FindObjectOfType<Volume>();

                                AC.CC.CanMove = false;

                                AC.CC.CharaInitialize();

                                //HDRPを変える
                                if (volume.profile.TryGet(out FilmGrain Grain))
                                {
                                    FilmGrain Gra = Grain;

                                    Grain.intensity.SetValue(new ClampedFloatParameter(1f, 0f, 1f, true));
                                }

                                Transform TileMapParent = FindObjectOfType<Grid>().transform;
                                Transform BackGroundParent = GameObject.Find("BackGroundParent").transform;

                                //マップチップを変える
                                TileMapParent.GetChild(0).gameObject.SetActive(false);
                                TileMapParent.GetChild(1).gameObject.SetActive(true);

                                //背景を変える
                                BackGroundParent.GetChild(0).gameObject.SetActive(false);
                                BackGroundParent.GetChild(1).gameObject.SetActive(true);

                                yield return new WaitForSeconds(1f);

                                //BGMを元に戻す
                                BGMController.Controller.BGMUnPause();
                                AC.Audio.Stop();

                                //マップチップを元に戻す
                                TileMapParent.GetChild(0).gameObject.SetActive(true);
                                TileMapParent.GetChild(1).gameObject.SetActive(false);

                                //背景を元に戻す
                                BackGroundParent.GetChild(0).gameObject.SetActive(true);
                                BackGroundParent.GetChild(1).gameObject.SetActive(false);

                                Grain.intensity.SetValue(new ClampedFloatParameter(0f, 0f, 1f, true));

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //SchooZone ダンジョンにチェンジ3
                        case 3:

                            {
                                //音を止めてノイズを流す
                                BGMController.Controller.BGMPause();
                                AC.Audio.PlayClips(0);

                                volume = FindObjectOfType<Volume>();

                                AC.CC.CanMove = false;

                                AC.CC.CharaInitialize();

                                //HDRPを変える
                                if (volume.profile.TryGet(out FilmGrain Grain))
                                {
                                    FilmGrain Gra = Grain;

                                    Grain.intensity.SetValue(new ClampedFloatParameter(1f, 0f, 1f, true));
                                }

                                yield return new WaitForSeconds(3f);

                                GameObject.Find("Fader").GetComponent<Image>().DOFade(1f, 1f);


                                AIR.ReadLines(AC.NowIvent);

                            }

                            break;

                        //SchoolZone ベリー初登場
                        case 4:

                            {
                                GameObject Prefab = Resources.Load("Prefab/Charactor/NormalCharactor") as GameObject;
                                RuntimeAnimatorController BerryAnimator = Resources.Load("Animator/Berry/BerryNormalController") as RuntimeAnimatorController;

                                Transform Chara = Instantiate(Prefab).transform;

                                Chara.position = new Vector3(19f,-2.01f);

                                Chara.GetComponent<Animator>().runtimeAnimatorController = BerryAnimator;

                                yield return new WaitForSeconds(0.1f);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //SchoolZone ベリーポータルから帰ってくる
                        case 5:

                            {
                                AC.Audio.PlayClips(1);

                                GameObject Prefab = Resources.Load("Prefab/Charactor/NormalCharactor") as GameObject;
                                RuntimeAnimatorController BerryAnimator = Resources.Load("Animator/Berry/BerryNormalController") as RuntimeAnimatorController;

                                Material PortalNoise= Resources.Load("Material/PortalTeleport") as Material;

                                PortalNoise.SetFloat("_Mask", 1);
                                PortalNoise.SetColor("_Color",new Color(8,8,8));

                                Transform Chara = Instantiate(Prefab).transform;

                                Chara.name = "BerryObject";
                                Chara.position = new Vector3(19.6f, -2.01f);
                                Chara.localScale = new Vector3(0f,2f);

                                Chara.GetComponent<Animator>().runtimeAnimatorController = BerryAnimator;

                                Chara.GetComponent<SpriteRenderer>().material = PortalNoise;

                                Chara.DOScaleX(2f,0.5f);

                                //Portalのパーティクルを止める
                                ParticleSystem Portal = GameObject.Find("Portal").GetComponent<ParticleSystem>();
                                ParticleSystem.EmissionModule emission = Portal.emission;

                                emission.enabled = false;

                                yield return new WaitForSeconds(2.5f);

                                PortalNoise.DOFloat(-0.01f,"_Mask",1f);
                                PortalNoise.DOColor(new Color(1,1,1),1f);

                                yield return new WaitForSeconds(1.1f);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;
                    }
                }

                break;

            case "FirstDangeon_1":

                {
                    switch (IventNumber)
                    {
                        //FirstDangeon:刀に触る前にキャラの場所をセット
                        case 1:

                            {
                                Transform Chara = GameObject.Find("Charactor").transform;

                                Chara.position = new Vector3(18.5f, -2f, 0);

                                Chara.localScale = new Vector3(2, 2, 1);
                            }

                            break;

                        //FirstDangeon:黄金の林檎とアップルのアニメーションをセット
                        case 2:

                            {
                                GameObject.Find("DangeonFlontGround").transform.Find("GoldenAppleParent").gameObject.SetActive(true);

                                Animator Chara = GameObject.Find("Charactor").GetComponent<Animator>();

                                RuntimeAnimatorController ChangeAnimator = Resources.Load<RuntimeAnimatorController>("Animator/Apple/AppleIventController");

                                Chara.runtimeAnimatorController = ChangeAnimator;

                                Chara.applyRootMotion = true;

                                Chara.SetTrigger("OdorokiTrigger");
                            }

                            break;

                        //FirstDangeon:敵が現れる
                        case 3:

                            {
                                GameObject Enemy, Effect;

                                Transform EnemyParent = GameObject.Find("EnemyParent").transform;

                                GameObject Prefab = Resources.Load("Prefab/Enemy/Alpha01_Enemy") as GameObject;
                                GameObject EffectPrefab = Prefab.GetComponent<EnemyStateAbstract>().RiseEffect;

                                //敵生成
                                Enemy = Instantiate(Prefab, EnemyParent);

                                Enemy.transform.position = new Vector3(16, -3, 0);
                                Enemy.GetComponent<Alpha01StateManage>().MoveDirection = false;
                                Enemy.GetComponent<SpriteRenderer>().sortingOrder = 0;

                                Enemy.GetComponent<Alpha01StateManage>().State = EnemyStateAbstract.EnemyState.Stop;

                                Enemy = Instantiate(Prefab, EnemyParent);

                                Enemy.transform.position = new Vector3(26, -3, 0);
                                Enemy.GetComponent<SpriteRenderer>().sortingOrder = 1;

                                Enemy.GetComponent<Alpha01StateManage>().State = EnemyStateAbstract.EnemyState.Stop;

                                yield return new WaitForSeconds(4f);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //FirstDangeon:敵が撃ってくる
                        case 4:

                            {
                                AC.ATC.CanText = false;

                                GameObject Enemy = GameObject.Find("EnemyParent").transform.GetChild(1).gameObject;

                                Enemy.GetComponent<Animator>().SetTrigger("AttackTrigger");
                                Enemy.GetComponent<EnemyStateAbstract>().CharaTrans = GameObject.Find("Charactor").transform;

                                yield return new WaitForSeconds(3f);

                                AC.ATC.CanText = true;

                                AIR.ReadLines(AC.NowIvent);

                            }

                            break;

                        //FirstDangeon:日本刀を削除
                        case 5:

                            {
                                Destroy(GameObject.Find("KatanaSaya"));
                            }

                            break;

                        //FirstDangeon:壁を壊す
                        case 6:

                            {
                                GameObject Prefab = Resources.Load("Prefab/Battle/Effect/WallDestroyParticle01") as GameObject;

                                GameObject Effect = Instantiate(Prefab);

                                Camera.main.DOShakePosition(4.0f, 0.1f);

                                BGMController.Controller.BGMFadeOut();

                                AC.Audio.PlayClips(0);

                                yield return new WaitForSeconds(3.0f);

                                MaterialAlphaUndo WallMaterial = GameObject.Find("RightWall").GetComponent<MaterialAlphaUndo>();

                                WallMaterial.Materials[0].DOFloat(0f, "_Alpha", 1f);
                                WallMaterial.Materials[1].DOFloat(0f, "_Alpha", 1f);
                                WallMaterial.Materials[2].DOFloat(0f, "_Alpha", 1f);

                                MaterialOkiba EffectMaterial = Prefab.GetComponent<MaterialOkiba>();

                                EffectMaterial.SiyouMaterial[0].DOFloat(0f, "_Alpha", 1f);
                                EffectMaterial.SiyouMaterial[1].DOFloat(0f, "_Alpha", 1f);

                                yield return new WaitForSeconds(2.0f);

                                //壁を壊しマテリアル等を元に戻す
                                Destroy(Effect);
                                Destroy(GameObject.Find("RightWall"));
                                Destroy(GameObject.Find("RightWallCollider"));

                                WallMaterial.Undo();

                                EffectMaterial.SiyouMaterial[0].SetFloat("_Alpha", 1f);
                                EffectMaterial.SiyouMaterial[1].SetFloat("_Alpha", 1f);

                                AIR.ReadLines(AC.NowIvent);

                            }

                            break;
                    }
                }

                break;

            case "FirstDangeon_2":

                {
                    switch (IventNumber)
                    {
                        //FirstDangeon_2:ダッシュについての解説を入れる
                        case 1:

                            {
                                GameObject SetumeiPrefab = Resources.Load("Prefab/UI/Window/DashSetumeiParent") as GameObject;

                                GameObject Setumei = Instantiate(SetumeiPrefab, AC.BC.UICanvas);

                                Setumei.GetComponent<WindowController>().Special = 2;
                            }

                            break;
                    }
                }

                break;

            case "FirstDangeon_4":

                {
                    switch (IventNumber)
                    {
                        //FirstDangeon_4:DeltaGN出現
                        case 1:

                            {
                                GameObject DeltaGNEgg = GameObject.Find("DeltaGNEgg");
                                GameObject DeltaGNPrefab = Resources.Load("Prefab/Enemy/DeltaGN_Enemy") as GameObject;

                                DeltaGNEgg.transform.DOMoveY(0f, 1.5f);

                                yield return new WaitForSeconds(3f);

                                float Kaiten = 45f;
                                Rigidbody2D EggRB = DeltaGNEgg.GetComponent<Rigidbody2D>();
                                Material mate = DeltaGNEgg.GetComponent<SpriteRenderer>().material;

                                Camera.main.transform.DOMove(new Vector3(25, 0, -10), 8f);
                                Camera.main.DOOrthoSize(3.5f, 8f);

                                GameObject Effect = Instantiate(DeltaGNPrefab.GetComponent<DeltaGNController>().RiseEffectPre);

                                AC.Audio.PlayClips(0);

                                //回転を始める
                                while (true)
                                {
                                    EggRB.angularVelocity = Kaiten;

                                    yield return new WaitForSeconds(0.05f);

                                    Kaiten += 5f;

                                    if (Kaiten >= 720f)
                                    {
                                        AC.Audio.PlayClips(1);

                                        break;
                                    }
                                }

                                //回転終わった後にすっごい輝いて孵化する

                                float Factor = Mathf.Pow(2, 20);
                                Color color = new Color(0.5f * Factor, 0.3f * Factor, 0.7f * Factor);

                                mate.DOColor(color, 0.5f);
                                Camera.main.transform.DOMove(new Vector3(25, -2, -10), 0.5f);
                                Camera.main.DOOrthoSize(5f, 0.5f);

                                volume = FindObjectOfType<Volume>();

                                //HDRPを変える
                                if (volume.profile.TryGet(out Bloom bloom))
                                {
                                    Bloom Blo = bloom;

                                    Blo.scatter.SetValue(new ClampedFloatParameter(1f, 0f, 1f, true));

                                    Blo.intensity.SetValue(new ClampedFloatParameter(100f, 0f, 100f, true));

                                    Blo.threshold.SetValue(new MinFloatParameter(0f, 0f));
                                }

                                yield return new WaitForSeconds(0.5f);

                                Destroy(Effect);

                                GameObject DeltaGN = Instantiate(DeltaGNPrefab, GameObject.Find("EnemyParent").transform);

                                DeltaGN.transform.localPosition = new Vector3(25, 1);

                                DeltaGN.GetComponent<DeltaGNController>().Special = 1;

                                //元に戻す
                                mate.DOColor(new Color(3f, 1.7f, 4.2f), 0.5f).OnComplete(() => 
                                {
                                    Bloom Blo = bloom;

                                    Blo.scatter.SetValue(new ClampedFloatParameter(0.2f, 0f, 1f, true));

                                    Blo.intensity.SetValue(new ClampedFloatParameter(1f, 0f, 100f, true));

                                    Blo.threshold.SetValue(new MinFloatParameter(1f, 0f));

                                    Destroy(DeltaGNEgg);
                                });

                                yield return new WaitForSeconds(0.8f);

                                DeltaGN.transform.DOLocalMoveY(-1.5f, 0.5f);

                                yield return new WaitForSeconds(1f);

                                AIR.ReadLines(AC.NowIvent);

                                yield return new WaitForSeconds(1.5f);

                                DeltaGN.GetComponent<DeltaGNController>().enabled = true;

                                BGMController.Controller.BGMPlay(1);
                            }

                            break;
                    }
                }

                break;

            case "ClassRoom":

                {
                    switch (IventNumber)
                    {
                        //ClassRoom:シーン入り
                        case 1:

                            {

                                GameObject.Find("ExteriorBackGround").GetComponent<SpriteRenderer>().DOFade(0f, 1f);

                                yield return new WaitForSeconds(4f);

                                Camera.main.transform.DOMove(new Vector3(8.23f, -3f, -10f), 1f);

                                yield return new WaitForSeconds(2f);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //ClassRoom:黄金の林檎出てくる
                        case 2:

                            {
                                Transform GoldenApple = GameObject.Find("GoldenAppleParent").transform;

                                GoldenApple.GetChild(0).gameObject.SetActive(true);
                                GoldenApple.GetChild(1).gameObject.SetActive(true);

                                GoldenApple.DOLocalMove(new Vector3(7.61f, -1.54f, 0f), 1f);

                                yield return new WaitForSeconds(1f);

                                AC.CC.GetComponent<Animator>().SetTrigger("Suwari02Trigger");

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //ClassRoom:先生入れ替え
                        case 3:

                            {
                                SpriteRenderer Teacher = GameObject.Find("Teacher").GetComponent<SpriteRenderer>();

                                Teacher.sprite = Teacher.GetComponent<SpriteOkiba>().Okiba[0];

                                yield return new WaitForSeconds(0.2f);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //ClassRoom:黄金の林檎が隠れる
                        case 4:

                            {
                                Transform GoldenApple = GameObject.Find("GoldenAppleParent").transform;

                                GoldenApple.DOLocalMove(new Vector3(8.4f, -2.215f, 0f), 1f);

                                yield return new WaitForSeconds(1f);

                                GoldenApple.GetChild(0).gameObject.SetActive(false);
                                GoldenApple.GetChild(1).gameObject.SetActive(false);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //ClassRoom:授業終わり
                        case 5:

                            {
                                Destroy(GameObject.Find("Mob02"));
                                Destroy(GameObject.Find("Mob03"));
                                Destroy(GameObject.Find("Mob04"));

                                Destroy(GameObject.Find("Teacher"));

                                GameObject.Find("Chair05").GetComponent<SpriteRenderer>().sortingLayerName = "BackHaikei";

                                Camera.main.transform.position = new Vector3(8.23f, 0f, -10f);
                                Camera.main.GetComponent<CameraFollower>().enabled = true;

                                //キャラクター設定
                                AC.CC.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/Apple/AppleNormalController");

                                AC.CC.transform.position = new Vector2(8.5f, -1f);

                                AC.CC.enabled = true;
                                AC.CC.CanMove = false;

                                //黄金の林檎設定
                                Transform GoldenApple = GameObject.Find("GoldenAppleParent").transform;

                                GoldenApple.GetChild(0).gameObject.SetActive(true);
                                GoldenApple.GetChild(1).gameObject.SetActive(true);

                                GoldenApple.position = new Vector2(9.4f, -1.3f);

                                GoldenApple.GetComponent<GoldenAppleTracking>().enabled = true;
                                GoldenApple.GetComponent<GoldenAppleTracking>().Target = AC.CC.transform;

                                //チャイムを流す
                                AC.Audio.PlayClips(0);

                            }

                            break;

                        //ClassRoom:教室から廊下へ
                        case 6:

                            {
                                Transform BackGroundParent = GameObject.Find("BackGroundParent").transform;

                                Transform MoveDoor;

                                BackGroundParent.GetChild(0).gameObject.SetActive(true);

                                //教室の画像をフェードアウト
                                foreach (Transform child in BackGroundParent.GetChild(1))
                                {
                                    foreach (Transform grandchild in child)
                                    {
                                        if (grandchild.GetComponent<SpriteRenderer>())
                                        {
                                            grandchild.GetComponent<SpriteRenderer>().DOFade(0f, 1f);
                                        }
                                    }

                                    if (child.GetComponent<SpriteRenderer>())
                                    {
                                        child.GetComponent<SpriteRenderer>().DOFade(0f, 1f);
                                    }
                                }

                                GameObject.Find("TileMapGrid").transform.GetChild(0).GetComponent<TileMapFade>().Fade(0f, 1f, true);

                                GameObject.Find("TileMapGrid").transform.GetChild(1).gameObject.SetActive(true);
                                GameObject.Find("TileMapGrid").transform.GetChild(1).GetComponent<TileMapFade>().Fade(1f, 1f, false);


                                BackGroundParent.GetChild(0).gameObject.SetActive(true);

                                //廊下の画像をフェードイン
                                foreach (Transform child in BackGroundParent.GetChild(0))
                                {
                                    if (child.GetComponent<SpriteRenderer>())
                                    {
                                        child.GetComponent<SpriteRenderer>().DOFade(1f, 1f);
                                    }
                                }

                                //ドアを動かす
                                AC.Audio.PlayClips(1);

                                switch (Others)
                                {
                                    case "Left":

                                        {
                                            MoveDoor = BackGroundParent.GetChild(1).Find("LeftDoorParent").GetChild(0);

                                            MoveDoor.DOLocalMoveX(-3.64f,1f);

                                            yield return new WaitForSeconds(2f);

                                            MoveDoor.transform.localPosition = new Vector2(-6.37f,-1.4f);
                                        }

                                        break;

                                    case "Right":

                                        {
                                            MoveDoor = BackGroundParent.GetChild(1).Find("RightDoorParent").GetChild(0);

                                            MoveDoor.DOLocalMoveX(12.83f, 1f);

                                            yield return new WaitForSeconds(2f);

                                            MoveDoor.transform.localPosition = new Vector2(15.55f, -1.4f);

                                        }

                                        break;
                                }

                                


                                BackGroundParent.GetChild(1).gameObject.SetActive(false);

                                AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //ClassRoom:廊下から教室へ
                        case 7:

                            {
                                Transform BackGroundParent = GameObject.Find("BackGroundParent").transform;

                                Transform MoveDoor;


                                //廊下の画像をフェードアウト
                                foreach (Transform child in BackGroundParent.GetChild(0))
                                {
                                    if (child.GetComponent<SpriteRenderer>())
                                    {
                                        child.GetComponent<SpriteRenderer>().DOFade(0f, 1f);
                                    }
                                }

                                Transform SaveObject = BackGroundParent.GetChild(0).GetChild(2);

                                SaveObject.GetComponent<ParticleSystemRenderer>().material.DOFloat(0, "_Alpha", 1f);
                                SaveObject.GetChild(0).GetComponent<ParticleSystemRenderer>().material.DOFloat(0, "_Alpha", 1f);

                                GameObject.Find("TileMapGrid").transform.GetChild(1).GetComponent<TileMapFade>().Fade(0f, 1f, true);

                                GameObject.Find("TileMapGrid").transform.GetChild(0).gameObject.SetActive(true);
                                GameObject.Find("TileMapGrid").transform.GetChild(0).GetComponent<TileMapFade>().Fade(1f, 1f, false);


                                BackGroundParent.GetChild(1).gameObject.SetActive(true);

                                //教室の画像をフェードイン
                                foreach (Transform child in BackGroundParent.GetChild(1))
                                {
                                    foreach (Transform grandchild in child)
                                    {
                                        if (grandchild.GetComponent<SpriteRenderer>())
                                        {
                                            grandchild.GetComponent<SpriteRenderer>().DOFade(1f, 1f);
                                        }
                                    }

                                    if (child.GetComponent<SpriteRenderer>())
                                    {
                                        child.GetComponent<SpriteRenderer>().DOFade(1f, 1f);
                                    }

                                }

                                //ドアを動かす
                                AC.Audio.PlayClips(1);

                                switch (Others)
                                {
                                    case "Left":

                                        {
                                            MoveDoor = BackGroundParent.GetChild(1).Find("LeftDoorParent").GetChild(0);

                                            MoveDoor.DOLocalMoveX(-3.64f, 1f);

                                            yield return new WaitForSeconds(2f);

                                            MoveDoor.DOLocalMoveX(-6.37f, 1f);
                                        }

                                        break;

                                    case "Right":

                                        {
                                            MoveDoor = BackGroundParent.GetChild(1).Find("RightDoorParent").GetChild(0);

                                            MoveDoor.DOLocalMoveX(12.83f, 1f);

                                            yield return new WaitForSeconds(2f);

                                            MoveDoor.DOLocalMoveX(15.55f, 1f);

                                        }

                                        break;
                                }

                                yield return new WaitForSeconds(2f);

                                BackGroundParent.GetChild(0).gameObject.SetActive(false);

                                SaveObject.GetComponent<ParticleSystemRenderer>().material.SetFloat("_Alpha", 1);
                                SaveObject.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetFloat("_Alpha", 1);

                                AIR.ReadLines(AC.NowIvent);

                            }

                            break;
                    }
                }

                break;

            case "RingoRoom":

                {
                    switch (IventNumber)
                    {
                        //林檎起床
                        case 1:

                            {
                                Destroy(GameObject.Find("Charactor"));

                                GameObject CharaPrefab = Resources.Load("Prefab/Charactor/NormalCharactor") as GameObject;

                                RuntimeAnimatorController AniCon = Resources.Load("Animator/Ringo/RingoPajamaNormalController") as RuntimeAnimatorController;

                                Transform Chara = Instantiate(CharaPrefab).transform;

                                Chara.position = new Vector3(5.18f,-2.21f);
                                Chara.localScale = new Vector3(-2,2,1);

                                Chara.GetComponent<Animator>().runtimeAnimatorController = AniCon;

                                Chara.name = "Charactor";
                                Chara.GetComponent<NormalCharaController>().enabled = true;

                                AC.CC = Chara.GetComponent<NormalCharaController>();
                            }

                            break;

                        //ドアが開く
                        case 2:

                            {
                                AC.Audio.PlayClips(0);

                                GameObject.Find("Door").transform.DOScaleX(6.6f,0.5f);

                                GameObject.Find("BackGround01").GetComponent<SpriteRenderer>().DOFade(0f,0.5f);
                                GameObject.Find("BackGround02").GetComponent<SpriteRenderer>().DOFade(1f,0.5f);
                                GameObject.Find("RefrigeratorDoor").GetComponent<SpriteRenderer>().DOFade(1f, 0.5f);

                                yield return new WaitForSeconds(0.5f);

                                AC.AIR.ReadLines(AC.NowIvent);

                            }

                            break;

                        //冷蔵庫開く
                        case 3:

                            {

                                GameObject Refrigerator= GameObject.Find("RefrigeratorDoor");

                                Refrigerator.transform.DOScaleX(0.1f,0.5f);

                                Refrigerator.GetComponent<SpriteRenderer>().sprite = Refrigerator.GetComponent<SpriteOkiba>().Okiba[0];

                                yield return new WaitForSeconds(1f);

                                AC.AIR.ReadLines(AC.NowIvent);
                            }

                            break;

                        //食べる時にいろいろ
                        case 4:

                            {
                                GameObject.Find("Door").transform.localScale = new Vector3(0,6.6f,1f);

                                GameObject Refrigerator = GameObject.Find("RefrigeratorDoor");

                                Refrigerator.transform.localScale = new Vector3(1,1,1);

                                Refrigerator.GetComponent<SpriteRenderer>().sprite = Refrigerator.GetComponent<SpriteOkiba>().Okiba[1];

                            }

                            break;
                    }
                }

                break;

            case "None":

                {
                    switch (IventNumber)
                    {
                        //CharactorControllerオンにする
                        case 1:

                            {

                                FindObjectOfType<NormalCharaController>().enabled = true;

                                yield return new WaitForSeconds(0.5f);

                                AIR.ReadLines(AC.NowIvent);

                            }

                            break;

                        //いったん中断
                        case 2:

                            {
                                yield return new WaitForSeconds(2f);

                                GameObject.Find("BackToTitle").GetComponent<CanvasGroup>().DOFade(1f,2f);

                                GameObject.Find("BackToTitle").GetComponent<BackToTitle>().enabled=true;
                            }

                            break;
                    }
                }

                break;
        }

        AC.ATC.Iventing = false;

        yield return null;
    }
}
