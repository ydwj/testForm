using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ConstHandler
{
    public static readonly string GameDataFileName = "gameData.data";

    public static readonly eRedDotType[] ArrReddotType =
        Enum.GetValues(typeof(eRedDotType)).OfType<eRedDotType>().ToArray();

    /// <summary>
    /// "/"
    /// </summary>
    public static readonly string a = "/";

    /// <summary>
    /// "{0}/{1}"
    /// </summary>
    public static readonly string b = "{0}/{1}";

    /// <summary>
    /// "Lv {0}"
    /// </summary>
    public static readonly string c = "Lv.{0}";

    /// <summary>
    /// "x{0}"
    /// </summary>
    public static readonly string d = "x{0}";

    /// <summary>
    /// "{0}%"
    /// </summary>
    public static readonly string e = "{0:#.#}%";

    /// <summary>
    /// "+{0}"
    /// </summary>
    public static readonly string f = "+{0}";


    //Animator Hash
    public static string Anim_Idle_Str = "stand01";
    public static int Anim_Idle = Animator.StringToHash(Anim_Idle_Str);
    
    public static string Anim_Walk_Str = "move";
    public static int Anim_Walk = Animator.StringToHash(Anim_Walk_Str);
    
    public static string Anim_Run_Str = "move";
    public static int Anim_Run = Animator.StringToHash(Anim_Run_Str);
    
    public static string Anim_Attack1_Str = "attack01";
    public static int Anim_Attack1 = Animator.StringToHash(Anim_Attack1_Str);
    
    public static string Anim_Attack2_Str = "attack02";
    public static int Anim_Attack2 = Animator.StringToHash(Anim_Attack2_Str);
    
    public static string Anim_Death_Str = "death";
    public static int Anim_Death = Animator.StringToHash(Anim_Death_Str);

    public static string Anim_Stand03_Str= "stand03";
    public static int Anim_Stand03_9 = Animator.StringToHash(Anim_Stand03_Str);

    public static int Anim_Sit = Animator.StringToHash("common_sit");
    
    //Building
    public static int Anim_Building_DragUp = Animator.StringToHash("Building_DragUp");
    public static int Anim_Building_Drop = Animator.StringToHash("Building_Drop");
    public static int Anim_Building_Scale1 = Animator.StringToHash("Building_Scale1");
    public static int Anim_Building_Scale2 = Animator.StringToHash("Building_Scale2");

    //Shader Hash
    public static int ShaderHash_MainTex = Shader.PropertyToID("_MainTex");
    public static int ShaderHash_GroundCheckTex = Shader.PropertyToID("_GroundCheckTex");

    #region 顾客动画

    public static int Anim_CusWalk = Animator.StringToHash("common_walk_man");
    public static int Anim_CusStand = Animator.StringToHash("common_stand");
    public static int Anim_CusHappy= Animator.StringToHash("common_happy");

    public static int Anim_CusAngry = Animator.StringToHash("common_Angry");

    public static int Anim_Custhink3 = Animator.StringToHash("think");//3
    public static int Anim_CusSet01_6= Animator.StringToHash("set01");

    public static int Anim_CusStand02_8 = Animator.StringToHash("stand02");
    public static int Anim_CusBuyIdle= Animator.StringToHash("common_BuyIdle");//求购默认表现动作
    public static int Anim_CusBuyIdleSp = Animator.StringToHash("common_buy_Sp");//特殊求购表现动作
    #endregion

    #region 主角动画

    public static int Anim_Role1Stand = Animator.StringToHash("role001_Stand");

    public static int Anim_Role1Stand2 = Animator.StringToHash("role001_Stand2");

    public static int Anim_Role1Move = Animator.StringToHash("role001_Move");

    public static int Anim_Role1Caiji = Animator.StringToHash("role001_Caiji");

    public static int Anim_Role1Kanshu = Animator.StringToHash("role001_Kanshu");

    public static int Anim_Role1Xiufu = Animator.StringToHash("role001_Xiufu");

    #endregion

    #region 狗动画

    public static int Anim_DogStand= Animator.StringToHash("dog_Stand");

    public static int Anim_DogMove = Animator.StringToHash("dog_Move");

    public static int Anim_DogDagun = Animator.StringToHash("dog_Fangun");

    #endregion


    #region 船动画


    public static int Anim_Boat_Yangfan = Animator.StringToHash("Act_make01");
    #endregion
    //ResourceName
    public static string ResName_Eff_Gold = "Eff_GetGold";
    public static string ResName_Eff_Smoke = "Eff_Smoke";
    public static string ResName_Eff_Die = "Eff_Die";
    public static string ResName_Eff_SetOnGrid = "Eff_SetOnGrid";
    public static string ResName_Eff_Leaf = "Effect_scene_gather_trees";
    public static string ResName_Eff_Stone = "Effect_scene_gather_stone";
    public static string ResName_Eff_ExtendFinish = "Effect_scene_gather_success";
    public static string ResName_Eff_MapPointFocus = "Eff_MapPointFocus";

    public static string ResName_MatTransparent = "Mat_Transparent";
    public static string ResName_MatFocus = "Mat_Focus";
    private static string QualityBgPre = "quality_bg_";
    public static string ResName_QualityBg_0 = QualityBgPre + (int)eQualityType.Gray;
    public static string ResName_QualityBg_1 = QualityBgPre + (int)eQualityType.Green;
    public static string ResName_QualityBg_2 = QualityBgPre + (int)eQualityType.Blue;
    public static string ResName_QualityBg_3 = QualityBgPre + (int)eQualityType.Purple;
    public static string ResName_QualityBg_4 = QualityBgPre + (int)eQualityType.Orange;
    public static string ResName_QualityBg_5 = QualityBgPre + (int)eQualityType.Red;
    
    private static string QualityBgPre_Hero1 = "quality_bg_hero1_";
    public static string ResName_QualityBg_Hero1_0 = QualityBgPre_Hero1 + (int)eQualityType.Gray;
    public static string ResName_QualityBg_Hero1_1 = QualityBgPre_Hero1 + (int)eQualityType.Green;
    public static string ResName_QualityBg_Hero1_2 = QualityBgPre_Hero1 + (int)eQualityType.Blue;
    public static string ResName_QualityBg_Hero1_3 = QualityBgPre_Hero1 + (int)eQualityType.Purple;
    public static string ResName_QualityBg_Hero1_4 = QualityBgPre_Hero1 + (int)eQualityType.Orange;
    public static string ResName_QualityBg_Hero1_5 = QualityBgPre_Hero1 + (int)eQualityType.Red;
    
    private static string QualityBgPre_Hero2 = "quality_bg_hero2_";
    public static string ResName_QualityBg_Hero2_0 = QualityBgPre_Hero2 + (int)eQualityType.Gray;
    public static string ResName_QualityBg_Hero2_1 = QualityBgPre_Hero2 + (int)eQualityType.Green;
    public static string ResName_QualityBg_Hero2_2 = QualityBgPre_Hero2 + (int)eQualityType.Blue;
    public static string ResName_QualityBg_Hero2_3 = QualityBgPre_Hero2 + (int)eQualityType.Purple;
    public static string ResName_QualityBg_Hero2_4 = QualityBgPre_Hero2 + (int)eQualityType.Orange;
    public static string ResName_QualityBg_Hero2_5 = QualityBgPre_Hero2 + (int)eQualityType.Red;
    
    //GlobalData
    public static int nTmpSoldierCount = 12;
    public static float fCheckSkillReleaseTime = 8;
    public static float fCheckFightDialogTime = 8;
    public static float fCheckSkillReleaseRate = 1f;

    public static int MapPoint_OccCreateResMaxMinutes = 60;
    //public static float fFightBeginningCavalryMoveDistance = 17f;

    //ADPosID
}