using Sirenix.OdinInspector;

public enum GameMsg
{
    Update_Main,
    Update_Exp,
    Update_Main_Star,
    Main_in,
    Main_out,
    Main_AllIn,
    Main_Allout,
    Main_ShowGetGoldFlyEff,
    Main_ShowGetDiamondFlyEff,
    Main_ShowGetStarFlyEff,
    Main_ShowGetExpEff,
    Main_EnableTouch,
    Main_CheckGuide,
    NaveTips_Show,
    Main_ShowGetBaseFlyEff,
    Main_ShowGetEquipFlyEff,
    Main_ShowGetTrustFlyEff,
    Main_UpdateBaseResources,
    Main_UpdataEquipResources,
    Main_LvlupBuilding,
    Main_Task,
    Main_FalseAll,

    Update_FormFightCamp,
    Update_FormWareHouse,
    Update_FormOrder,
    Update_FormFight_VSProgress,
    Update_FormFight_VSFightingVal,
    Update_HeroPage,//更新英雄信息栏
    Update_HeroFragCount,//更新英雄碎片属性
    Update_FormChangeEquip,//更行装备变更页面
    Update_SoldierPage,
    Update_FormPet,
    Update_FormPetLvlAddExp,//更新士兵刷新经验
    Update_FormSoildierTrain,//更新士兵刷新界面
    Update_FormPlayDia,
    Update_Energy,
    Update_Trust,
    Update_HUDOrderState,
    Update_FormMakingFactoryTotal,
    Update_FormMapWildMonster,
    Update_FormStorage,
    Update_FormEquipRepair,
    
    Fight_BeginFight,
    SimulateCustomerAni,
    StopTrainSoldier,
    FormGuideDoNext,
    FormGuideSetCanvas,
    FormMakingFactoryTotal_PlayEquipFly,
    FormHeroPage_PlayValScroll,

    PlayTimeline,//播放当前Timeline 
    RegisterTrackObj,
    CloseFormPlayDia,//关闭对话流程，回归Timeline播放
    StartCountKillPlant,
    CloseFormExtendDemand,
    ShowMainTask,
    DoDogJumpBack,
    DogMove,
    FalseGirl

}

public enum eResultType
{
    Fail,
    Success,

    Fail_Lack_Gold,
    Fail_Lack_Diamond,
    Fail_Lack_Item,
    Fail_Lack_Star,
    Fail_Lack_Exp,
    Fail_Lack_Frag,
    Fail_Lack_Energy,
    Fail_Lack_Trust,
    Fail_LvlMax,
    Fail_StarMax,
    Fail_Condition,
}

public enum eItemType
{
    Null,
    BaseItem,               //1-基础材料
    SpecialItem,            //2-特殊材料
    Paper,                  //3-图纸       
    Ingredient,             //4-食材     
    Food,                   //5-食品        
    HeroFrag,               //6-英雄碎片
    TreasureBox,            //7-宝箱
    Weapon = 101,           //101-武器
    Head = 201,             //201-头
    Armour = 301,           //301-甲
    Shoes = 401,            //401-靴
    Necklace = 501,         //501-项链
    Ring = 601,             //601-戒指
    Scroll = 701,           //701-卷轴
    Other = 1001            //1001-其他道具
}

public enum eFoodType
{
    Meat = 1,
    Fruit = 2,
    Seafood = 3,
}

public enum eRedDotType
{
    Turntable,
    NewItem_Obsolete,
    StorageNewItem = 2,
    MainTask,
    BranchTask,
}
public enum eFuncUnlock
{
    FormMain_Top = 0,//主界面资源栏
    FormMain_MainTask = 1,
    FormMain_BtnBuilding = 2,
    FormMain_Down = 3,//主角面下方按钮栏
    Hero = 4,
    Storage = 5,
    Pet = 6,
    Map = 7,
    Shop = 8,
    MapPVE = 9,
    MapPVP = 10,
    FormSale_Diccount=11,
    FormSale_RisePrice=12,
    FormSale_Recommend=13,
}

//=================Game

public enum SceneType
{
    Logo,
    Enter,
    FightScene,
    MainScene,
    SkillDebugScene,
    Count,
}


//场景装饰类型
public enum eSceneDeviceType
{
    Building,
    Decoration,
}

//建筑类型
public enum eBuildingtype
{
    Null,
    Building,               //1资源设施
    Workshop,               //2工坊
    Storage,                //3仓库
    Hero,                   //4英雄建筑
    PetTrainingGround,      //5宠物训练场
    SoldierTrainingGround,  //6士兵训练场
    BlackMarket,            //7黑市
    Order,                  //8订单
    Smelter,                //9熔炉
    InhabitantHouse,        //10居民屋
    Camp,                   //11营地
    Wharf,                  //12码头
    WatchTower,             //13瞭望塔
    Bridge
}
//建筑解锁类型
public enum eBuildingUnlockCondition
{
    Null,
    AccountLvl = 1,//玩家等级
    PlaceBuilding = 2,//修建某建筑
    Prosperity = 3,//繁荣度
    PaperUnlock = 4,//图纸解锁
    CampLvlReach = 5,//营地等级达到N级
}
//建筑状态
public enum eBuildingStatus
{
    Empty,//空闲
    Making,//生产中
    Output,//产出
    Building,//修建中
    Upgrading,//升级中
}
//装备类型
public enum eEquipType
{
    Null,
    Weapon = 101,           //101-武器
    Head = 201,             //201-头     
    Armour = 301,           //301-甲     
    Shoes = 401,            //401-靴     
    Necklace = 501,         //501-项链 
    Ring = 601,             //601-戒指        
    Scroll = 701,           //701-卷轴
}

//植物（地块）类型
public enum eGroundType
{
    Null,
    Wood,           //木材
    Stones,         //石材
    Grass,          //草料
    Leather,        //皮革
    FuncBuilding,   //功能建筑
    GetPet,         //获得宠物
    Fight,          //战斗
}

public enum eGroundStatus
{
    CantUse,//不可用
    Lock,   //未解锁
    Extending,//扩建中
    Empty,  //空闲
}
//通用加速弹窗加速类型
public enum eSpeedUpType
{
    ExtendGround,  //扩建土地
    Building,      //修建建筑
    MakeEquipItem, //制造装备
    SoldierLvlUp,  //士兵升级
    Map_Dispatch   //大地图派遣
}
//
public enum eTimeType
{
    Hour,
    Minute,
    Second,
}
//熟练度效果类型
public enum eMaturityEffectType
{
   UnlockDrawing,   //1-解锁图纸
   UpSellingPrice,  //2-售价提升
   DecreaseLoss,    //3-耗材减少
   UpQuality,       //4-品质提升

}
//品质类型
public enum eQualityType
{
    [LabelText("白")]
    Gray,   //白
    [LabelText("绿")]
    Green,  //绿
    [LabelText("蓝")]
    Blue,   //蓝
    [LabelText("紫")]
    Purple, //紫
    [LabelText("橙")]
    Orange, //橙
    [LabelText("红")]
    Red,    //红
}

//任务类型
public enum eTaskType
{
    Null,
    //主线
    GatherItem        = 1,         //收集资源
    CleanBarrier      = 2,       //清除障碍物
    MakeEquip         = 3,          //制造装备
    Building          = 4,           //建造
    Decoration        = 5,         //装饰物
    FinishPassSmall   = 6,    //通关小关卡
    FinishPassBig     = 7,      //通关Boss战
    HeroLvlUp         = 8,          //英雄升级
    WearEquip         = 9,          //穿戴装备
    SaleEquip         = 10,          //售卖装备
    LvlUp             = 11,              //升级
 
    //支线 
    Interaction       = 12,        //互动
    KillMonster       = 13,        //击杀野怪
    GatherSpecialItem = 14,  //收集特殊资源
    OpenBox           = 15,   //开宝箱
    MainSceneInitBuildingExtend = 16,   //修复主场景固定建筑
    BuildingLvlup     = 17,       //将任意xx建筑升级至N级
    MapBuildingUnlock = 18,       //将大地图xx建筑解锁
    ConfirmOrder      = 19,       //完成n个订单
}

//兵种
public enum eArmyType
{
    [LabelText("步兵")]
    Infantry = 101,   //步兵
    [LabelText("骑兵")]
    Cavalry = 102,    //骑兵
    [LabelText("弓兵")]
    Archer = 103,     //弓兵
    [LabelText("法师")]
    Wizard = 104,     //法师
    [LabelText("宠物")]
    Pet = 201,        //宠物
}

//士兵升级状态
public enum eSoldierLvlUpState
{ 
    Null,
    StartTrain,//准备训练
    Training,//训练中
    Success//训练完成
}

//顾客动作动画ID
public enum eCusTomAniType
{ 
    Null,
    Happy=1,
    Angry,
    Think,
    Stand01,//等待
    Angry2,
    Set01,
    Move,
    Stand02,//普通求购待机
    Stand03//求购特殊待机（n秒一次）
}


public enum eCustomBehaviour
{ 
    
}
//顾客行为
public enum eCusActive
{
    active_idle,
    active_move,
    active_wait,
    active_refuse,
    active_angry,
    active_discount,
    active_rise,
    active_sell,
    active_purchase,
    active_happy,
    active_idle_sp,//特殊求购待机行为
    active_buyIdle,
    active_disappointed,
    active_leave

}


public enum eChangeTrust
{ 
    Discount,
    RisePrice,
    Reject,
    Sale,  //普通售卖
    Recommend,
    ChatHappy,
    ChatAngry,
    SpecialDelivery //特殊顾客售卖
}

//顾客到达闲逛点后，下一次的行为
public enum eCusNextBehaviuour
{ 
    GoStrollPoint,
    Stay,
    Interaction,
}

public enum eMapPointType
{
    Fixed_Fight = 0,    //固定战斗
    Fixed_Gather = 1,   //固定采集
    Fixed_Event = 2,    //固定事件
    Fixed_Occupy = 3,   //占领资源点
    Random_Fight = 4,   //随机战斗
    Random_Gather = 5,  //随机采集
    Random_Event = 6,   //随机事件
}

public enum eMapFuncBuildingLvlEffectType
{
    Null,
    FuncUnlock = 1,
    GetHero = 2,
    AddHeroLvlMax = 3,
    AddHeroProperty = 4,
    AddBuildingMax = 5,
    MinusMapPointTime = 6,
    GetPet = 7,
    AddPetProperty = 8,
    Shop_UnlockItem = 9,
    Shop_MinusPrice = 10,
}

public enum eMapEventType
{
    Direct,     //0:直接触发结果
    Option,     //1:选项
    GiveItem,   //2:提交物品
}

public enum eFightType
{
    Map_MainFight,      //大地图_固定战斗
    Map_GatherUnlock,   //大地图_固定采集第一次战斗解锁该点
    Map_FixedEventFight, //大地图_事件结果触发战斗
    Map_RandomFight,    //大地图_随机点战斗(包括随机事件触发的战斗)
    Map_OccupyFight,    //大地图_占领点战斗
}

public enum eFightResult
{
    None,
    Win,
    Lose
}

public enum eCurrentNodeType
{ 
    Null,
    Start,
    Chat,
    Option
}

public enum eRewardType
{
    Item = 1,
    Hero = 2,
    Soldier = 3,
    Pet = 4,
    UnlockBuilding = 5,
    UnlockDecoration = 6,
}

/// <summary>
/// 统计类型
/// </summary>
public enum eStatisticsType
{
    UnlockPaper, //玩家解锁图纸次数
    EquipBlalst, //玩家爆高品质次数
    FightTimes,  //玩家战斗次数
    LoseFight,   //战斗失败次数
    GotoFormMap , //进入大地图场景
    OccuPyWin,
    SpCustomerDispatchNum,//特殊顾客派遣次数
    GetToukui,//收取了锤子
    GotoMapbuildingPage,//进入大地图的建造工坊

}

public enum eAttributeType
{
    Null,             //0   无属性
    Prosperity,       //1	繁荣度
    PriceAdd,         //2	售价加成
    MakeTime,         //3	制造时间缩短
    Attack,           //4	攻击力
    Defence,          //5	防御力
    Hp,               //6	血量
    BreakArmor,       //7	破甲
    OutputTime,       //8	单位产出耗时（秒）
    OutputCount,      //9	单位产出量
    OutputMax,        //10	产出上限
    StorageMax,       //11	仓库存储上限
    GreatQualityProb, //12	高品质概率
    UnlockPaper,      //13	解锁图纸
    PriceAdd2,        //14	售价提高
    CostMinus,          //15	减少耗材
    QualityAdd,          //16	品质提升
    Price,              //17	售价
    Trust,              //18	装饰物互动获得信任度
    BuyPersonCount,     //19	求购人数
    InhabitantCount,    //20	部落内能容纳人数
    BaseMaterialMax,     //21	基础材料存储上限
    TrustMax,           //22  信任度上限
    SaleGetTrust,       //22  售卖获得信任度
}

//功能建筑Buff类型
public enum eBuffType
{
    AddHeroOrPetProperty = 1, //1提升英雄或宠物属性
    AddEquipSalePrice,        //2提升求购装备售价
    MinusMapPointTime,        //3野外战斗、采集等待时间减少
    MinusShopPrice,           //4减少黑市道具售价
    MinusMakeTime,            //5制造时间缩短
    AddEquipBlastingRate,     //6制造高品质概率提高
}

public enum eItemQualityLvlParamType
{
    MakeTime,        //制造时间
    Trust_Discount,  //打折增加信任度
    Trust_Rise,      //涨价扣除信任度
    Trust_Recommend, //推销扣除信任度
    BurnishCost,     //打磨消耗钻石
    BreakRate,       //破损率
    Break_RepairCost,       //破损修复消耗钻石
}