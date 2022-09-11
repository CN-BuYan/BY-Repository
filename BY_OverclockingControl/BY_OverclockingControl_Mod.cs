namespace BuYanMod.OverclockingControl
{
    using BuYanMod.OverclockingControl.Licalization;
    using BuYanMod.OverclockingControl.ModSystem;
    using ProtoBuf;
    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Game;
    using Sandbox.Game.World;
    using Sandbox.ModAPI;
    using Sandbox.ModAPI.Interfaces.Terminal;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VRage;
    using VRage.Game;
    using VRage.Game.Components;
    using VRage.Game.ModAPI;
    using VRage.ModAPI;
    using VRage.ObjectBuilders;
    using static BuYanMod.Utils.Utils;

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MyProgrammableBlock), false, "BY_OverclockingControlSystem_L", "BY_OverclockingControlSystem_S")]
    public class OverclockingControlBlock : MyGameLogicComponent
    {
        //方块参数体
        [ProtoContract]
        public class OverclockingControl
        {
            [ProtoMember(1)]
            public float Reactor = 1;
            [ProtoMember(2)]
            public float GasGenerator = 1;
            [ProtoMember(3)]
            public float Gyro = 1;
            [ProtoMember(4)]
            public float Thrust = 1;
            [ProtoMember(5)]
            public float Drill = 1;
        }
        //主体↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        private readonly Guid GUID = new Guid("f297034ec68e4af0948f3a70f108339c");      //GUID
        private static readonly ModLicalizationText mt = new ModLicalizationText();     //汉化类
        private const string sID = "BY_OverclockingControlSystem";                      //方块子ID
        private static List<IMySlimBlock> GridBlocks = new List<IMySlimBlock>();         //临时网格块存储列表
        protected IMyProgrammableBlock Block => Entity as IMyProgrammableBlock;         //获取实体块
        static bool controlIsCreated = false;                                           //是否初始化控件
        static bool ThreadLock = false;                                                 //线程锁
        
        //方块数据
        public float Reactor { get; protected set; } = 1;
        public float GasGenerator { get; protected set; } = 1;
        public float Gyro { get; protected set; } = 1;
        public float Thrust { get; protected set; } = 1;
        public float Drill { get; protected set; } = 1;

        /// <summary>
        ///序列化配置
        /// </summary>
        public sealed override bool IsSerialized()
        {
            SaveConfig();
            return base.IsSerialized();
        }
        /// <summary>
        ///保存配置
        /// </summary>
        protected void SaveConfig()
        {
            LoadSaveDatas.Init(Block, GUID);
            LoadSaveDatas.Save(Block, LoadSaveDatas.ByteToString(MyAPIGateway.Utilities.SerializeToBinary(ReadValue())), GUID);
        }
        /// <summary>
        ///读取配置
        /// </summary>
        protected void LoadConfig()
        {
            LoadSaveDatas.Init(Block, GUID);
            var bytes = LoadSaveDatas.StringToByte(LoadSaveDatas.Load(Block, GUID));
            if (bytes == null || bytes.Length < 1) return;
            WriteValue(MyAPIGateway.Utilities.SerializeFromBinary<OverclockingControl>(bytes));
        }
        /// <summary>
        ///读取数值
        /// </summary>
        public OverclockingControl ReadValue()
        {
            return new OverclockingControl()
            {
                Reactor = Reactor,
                GasGenerator = GasGenerator,
                Gyro = Gyro,
                Thrust = Thrust,
                Drill = Drill,
            };
        }
        /// <summary>
        ///写入数值
        /// <summary>
        public void WriteValue(OverclockingControl value)
        {
            if (value == null) return;
            Reactor = value.Reactor;
            GasGenerator = value.GasGenerator;
            Gyro = value.Gyro;
            Thrust = value.Thrust;
            Drill = value.Drill;
        }
        /// <summary>
        ///初始化
        /// </summary>
        public sealed override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            //MyAPIGateway.Parallel.StartBackground(方法);创建多线程操作
        }
        

        /// <summary>
        ///对块的终端控件进行删除操作
        ///</summary>
        public void AdjustTerminalControls_controls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {

            //检查方块是不是MOD块
            if (block.BlockDefinition.SubtypeName == Block.BlockDefinition.SubtypeName)
            {
                //循环检测并删除指定控件
                for (int i = controls.Count - 1; i >= 0; i--)
                {
                    if (controls[i].Id == "Recompile" ||
                       controls[i].Id == "TerminalRun" ||
                       controls[i].Id == "ConsoleCommand" ||
                       controls[i].Id == "Edit")
                    {
                        controls.RemoveAt(i);
                    }
                }
            }
        }
        /// <summary>
        ///对块的终端工具条功能进行删除操作(按G拖动方块到工具条时显示的操作项目)
        /// </summary>
        public void AdjustTerminalControls_Action(IMyTerminalBlock block, List<IMyTerminalAction> controls)
        {

            //检查方块是不是MOD块
            if (block.BlockDefinition.SubtypeName == Block.BlockDefinition.SubtypeName)
            {
                //循环检测并删除指定动作
                for (int i = controls.Count - 1; i >= 0; i--)
                {

                    if (controls[i].Id == "RunWithDefaultArgument" ||
                       controls[i].Id == "Run")
                    {
                        controls.RemoveAt(i);
                    }

                }

            }

        }
        /// <summary>
        /// 更新
        /// </summary>
        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();
            if (Block.BlockDefinition.SubtypeId == "BY_OverclockingControlSystem")
            {
                Reactor = 1;
            }
            BuildControl();              //建立控件
            LoadConfig();                //加载配置
            IsEnable();           //加载方块设置信息更新网格
            Block.AppendingCustomInfo += Block_AppendingCustomInfo;//事件:方块终端右下角信息更新
            MyAPIGateway.TerminalControls.CustomControlGetter += AdjustTerminalControls_controls;//事件: 显示终端前更新终端
            MyAPIGateway.TerminalControls.CustomActionGetter += AdjustTerminalControls_Action;//事件: 显示G键功能前更新动作
            Block.CubeGrid.OnBlockAdded += CubeGrid_OnBlockAdded;//事件: 网格添加方块
            Block.CubeGrid.OnGridMerge += CubeGrid_OnGridMerge;//事件: 网格合并
            Block.CubeGrid.OnGridSplit += CubeGrid_OnGridSplit;//事件: 网格拆分
            NeedsUpdate = MyEntityUpdateEnum.EACH_10TH_FRAME;
        }
        public sealed override void UpdateBeforeSimulation10()
        {
            base.UpdateBeforeSimulation10();
            if (!ThreadLock) { MyAPIGateway.Parallel.StartBackground(IsEnable); ThreadLock = true; }
            Block.RefreshCustomInfo();   //触发更新块控制台(右下角)的信息的方法
        }
        private void IsEnable() {
            List<IMySlimBlock> blocks = new List<IMySlimBlock>();
            List<IMyProgrammableBlock> Pbs = new List<IMyProgrammableBlock>();
            Block.CubeGrid.GetBlocks(blocks, (b) => { if (b.BlockDefinition.Id.SubtypeName.Contains(sID)) return true; return false; });
            if (blocks.Count > 1)
            {
                int on = 0;
                int a = 0;
                foreach (IMySlimBlock block in blocks)
                {
                    IMyProgrammableBlock b = (IMyProgrammableBlock)block.FatBlock;
                    if (b.Enabled) { UpdateData(); on++;}
                    a++;
                    if(a==blocks.Count&&on==0) Overclock.Clear(b.CubeGrid);
                }
            }
            else {
                if (Block.Enabled) UpdateData();
                if (!Block.Enabled) Overclock.Clear(Block.CubeGrid);
            }
            ThreadLock = false;
        }

        //网格添加块时调用
        private void CubeGrid_OnBlockAdded(IMySlimBlock obj)
        {
            if (Block.Enabled)
            {
                if (obj.FatBlock is IMyReactor) Overclock.Reactor((IMyTerminalBlock)obj.FatBlock, Reactor, false);
                if (obj.FatBlock is IMyGasGenerator) Overclock.GasGenerator((IMyTerminalBlock)obj.FatBlock, GasGenerator, false);
                if (obj.FatBlock is IMyGyro) Overclock.Gyro((IMyTerminalBlock)obj.FatBlock, Gyro, false);
                if (obj.FatBlock is IMyThrust) Overclock.Thrust((IMyTerminalBlock)obj.FatBlock, Thrust, false);
                if (obj.FatBlock is IMyShipDrill) Overclock.Drill((IMyTerminalBlock)obj.FatBlock, Drill, false);
            }
            if (obj.BlockDefinition.Id.SubtypeName.Contains(sID)) { Sunchronize((IMyTerminalBlock)obj.FatBlock, false); }
        }
        //网格合并时调用
        private void CubeGrid_OnGridMerge(IMyCubeGrid arg1, IMyCubeGrid arg2) { IsEnable(); }
        //网格拆分时调用
        private void CubeGrid_OnGridSplit(IMyCubeGrid arg1, IMyCubeGrid arg2)
        {
            GridBlocks.Clear();
            arg1.GetBlocks(GridBlocks,(b)=> { if (b.BlockDefinition.Id.SubtypeName.Contains(sID)) return true;return false; });
            if (GridBlocks.Count == 0) Overclock.Clear(arg1);
            GridBlocks.Clear();
            arg2.GetBlocks(GridBlocks, (b) => { if (b.BlockDefinition.Id.SubtypeName.Contains(sID)) return true; return false; });
            if (GridBlocks.Count == 0) Overclock.Clear(arg2);
        }

        //多线程方法
        //MyAPIGateway.Parallel.StartBackground(SynchronousData);

        /// <summary>
        ///建立控件
        /// </summary>
        static void BuildControl()
        {
            if (controlIsCreated) return;
            //分隔符/标题
            TerminalRevise.CreateSeparator<IMyProgrammableBlock>("A-", "Test", BlockConfirm, BlockConfirm);
            TerminalRevise.CreateLabel<IMyProgrammableBlock>("BY-", "Lablel-超频设备", mt.Overclock, BlockConfirm, BlockConfirm);
            //滑块-反应堆
            TerminalRevise.CreateSlider<IMyProgrammableBlock>("BY-", "Slider-反应堆", mt.Reactor, BlockConfirm, BlockConfirm,
                (Me) => { var logic = GetTerminal(Me); if (logic.Item1) return logic.Item2.Reactor; return 1; },
                (Me, value) => { var logic = GetTerminal(Me); if (!logic.Item1) return; logic.Item2.Reactor = value;if (logic.Item2.Block.Enabled) Overclock.Reactor(Me, value, true); logic.Item2.SaveConfig(); Sunchronize(Me, true); },
                (Me, value) => { var logic = GetTerminal(Me); value.Append(logic.Item2.Reactor + " "+ mt.Times); }, 1, 10, 1, TerminalRevise.SliderStyle.Log);
            //滑块-气体发生器
            TerminalRevise.CreateSlider<IMyProgrammableBlock>("BY-", "Slider-气体发生器", mt.GasGenerator, BlockConfirm, BlockConfirm,
                (Me) => { var logic = GetTerminal(Me); if (logic.Item1) return logic.Item2.GasGenerator; return 1; },
                (Me, value) => { var logic = GetTerminal(Me); if (!logic.Item1) return; logic.Item2.GasGenerator = value; if (logic.Item2.Block.Enabled) Overclock.GasGenerator(Me, value, true); logic.Item2.SaveConfig(); Sunchronize(Me, true); },
                (Me, value) => { var logic = GetTerminal(Me); value.Append(logic.Item2.GasGenerator + " " + mt.Times); }, 1, 10, 1, TerminalRevise.SliderStyle.Log);
            //滑块-陀螺仪
            TerminalRevise.CreateSlider<IMyProgrammableBlock>("BY-", "Slider-陀螺仪", mt.Gyro, BlockConfirm, BlockConfirm,
                (Me) => { var logic = GetTerminal(Me); if (logic.Item1) return logic.Item2.Gyro; return 1; },
                (Me, value) => { var logic = GetTerminal(Me); if (!logic.Item1) return; logic.Item2.Gyro = value; if (logic.Item2.Block.Enabled) Overclock.Gyro(Me, value, true); logic.Item2.SaveConfig(); Sunchronize(Me, true); },
                (Me, value) => { var logic = GetTerminal(Me); value.Append(logic.Item2.Gyro + " " + mt.Times); }, 1, 10, 1, TerminalRevise.SliderStyle.Log);
            //滑块-推进器
            TerminalRevise.CreateSlider<IMyProgrammableBlock>("BY-", "Slider-推进器", mt.Thrust, BlockConfirm, BlockConfirm,
                (Me) => { var logic = GetTerminal(Me); if (logic.Item1) return logic.Item2.Thrust; return 1; },
                (Me, value) => { var logic = GetTerminal(Me); if (!logic.Item1) return; logic.Item2.Thrust = value; if (logic.Item2.Block.Enabled) Overclock.Thrust(Me, value,true); logic.Item2.SaveConfig(); Sunchronize(Me, true); },
                (Me, value) => { var logic = GetTerminal(Me); value.Append(logic.Item2.Thrust + " " + mt.Times); }, 1, 10, 1, TerminalRevise.SliderStyle.Log);
            //滑块-钻头
            TerminalRevise.CreateSlider<IMyProgrammableBlock>("BY-", "Slider-钻头", mt.Drill, BlockConfirm, BlockConfirm,
                (Me) => { var logic = GetTerminal(Me); if (logic.Item1) return logic.Item2.Drill; return 1; },
                (Me, value) => { var logic = GetTerminal(Me); if (!logic.Item1) return; logic.Item2.Drill = value; if (logic.Item2.Block.Enabled) Overclock.Drill(Me, value, true); logic.Item2.SaveConfig(); Sunchronize(Me,true); },
                (Me, value) => { var logic = GetTerminal(Me); value.Append(logic.Item2.Drill + " " + mt.Times); }, 1, 10, 1, TerminalRevise.SliderStyle.Log);

            controlIsCreated = true;
        }
        /// <summary>
        /// 更新超频数据设置
        /// </summary>
        private void UpdateData() {
            Overclock.Reactor(Block, Reactor, true);
            Overclock.GasGenerator(Block, GasGenerator, true);
            Overclock.Gyro(Block, Gyro, true);
            Overclock.Thrust(Block, Thrust, true);
            Overclock.Drill(Block, Drill, true);
            //ThreadLock = false;//解锁线程
        }
        /// <summary>
        /// 同网格超频设置同步
        /// </summary>
        /// <param name="block">块</param>
        /// <param name="IsSet">是在设置吗?</param>
        public static void Sunchronize(IMyTerminalBlock block ,bool IsSet)
        {
            List<IMySlimBlock> Blocks = new List<IMySlimBlock>();
            block.CubeGrid.GetBlocks(Blocks, (b) => { if (b.BlockDefinition.Id.SubtypeName.Contains("BY_OverclockingControlSystem")) return true; return false; });
            if (IsSet)
            {
                if (Blocks.Count > 1)
                {
                    var Logic1 = GetTerminal(block);
                    foreach (IMySlimBlock b in Blocks)
                    {
                        var Logic2 = GetTerminal((IMyTerminalBlock)b.FatBlock);
                        if (Logic2.Item1)
                        {
                            Logic2.Item2.Reactor = Logic1.Item2.Reactor;
                            Logic2.Item2.GasGenerator = Logic1.Item2.GasGenerator;
                            Logic2.Item2.Gyro = Logic1.Item2.Gyro;
                            Logic2.Item2.Thrust = Logic1.Item2.Thrust;
                            Logic2.Item2.Drill = Logic1.Item2.Drill;
                            Logic2.Item2.SaveConfig();
                        }
                    }
                }
            }
            else
            {
                if (Blocks.Count > 1)
                {
                    float Re = 1, GG = 1, Gy = 1, Th = 1, Dr = 1;
                    foreach (IMySlimBlock b in Blocks)
                    {
                        var Logic = GetTerminal((IMyTerminalBlock)b.FatBlock);
                        if (Logic.Item1)
                        {
                            if (Logic.Item2.Reactor > Re) Re = Logic.Item2.Reactor;
                            if (Logic.Item2.GasGenerator > GG) GG = Logic.Item2.GasGenerator;
                            if (Logic.Item2.Gyro > Gy) Gy = Logic.Item2.Gyro;
                            if (Logic.Item2.Thrust > Th) Th = Logic.Item2.Thrust;
                            if (Logic.Item2.Drill > Dr) Dr = Logic.Item2.Drill;
                        }
                    }
                    foreach (IMySlimBlock b in Blocks)
                    {
                        var Logic = GetTerminal((IMyTerminalBlock)b.FatBlock);
                        if (Logic.Item1)
                        {
                            Logic.Item2.Reactor = Re;
                            Logic.Item2.GasGenerator = GG;
                            Logic.Item2.Gyro = Gy;
                            Logic.Item2.Thrust = Th;
                            Logic.Item2.Drill = Dr;
                            Logic.Item2.SaveConfig();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///更新块控制台(右下角)的信息
        /// </summary>
        private void Block_AppendingCustomInfo(IMyTerminalBlock block, System.Text.StringBuilder arg2)
        {
            arg2.Append("\n"+mt.Info);
            arg2.Append("\n");
            arg2.Append("\n" + mt.Reactor +" :   "+ Math.Round(Reactor, 2) + "  " + mt.Times);
            arg2.Append("\n" + mt.GasGenerator + " :   " + Math.Round(GasGenerator, 2) + "  " + mt.Times);
            arg2.Append("\n" + mt.Gyro + " :   " + Math.Round(Gyro, 2) + "  " + mt.Times);
            arg2.Append("\n" + mt.Thrust + " :   " + Math.Round(Thrust, 2) + "  " + mt.Times);
            arg2.Append("\n" + mt.Drill + " :   " + Math.Round(Drill,2) +"  "+mt.Times);
            TerminalRevise.RefreshBlockTerminal(block);


        }
        /// <summary>
        ///获取控制台界面
        /// </summary>
        public static MyTuple<bool, OverclockingControlBlock> GetTerminal(IMyTerminalBlock Me)
        {
            //判断是否为空
            if (NullCheck.IsNull(Me)) return new MyTuple<bool, OverclockingControlBlock>(false, null);
            //判断是否本MOD方块
            if (!Me.BlockDefinition.SubtypeId.Contains(sID))
                return new MyTuple<bool, OverclockingControlBlock>(false, null);
            //获取游戏逻辑
            var Logic = Me?.GameLogic?.GetAs<OverclockingControlBlock>();
            //判断是否为空
            if (Logic == null) return new MyTuple<bool, OverclockingControlBlock>(false, null);
            return new MyTuple<bool, OverclockingControlBlock>(true, Logic);
        }
        /// <summary>
        ///  方块确认
        /// </summary>
        public static bool BlockConfirm(IMyTerminalBlock Me)
        {
            if (NullCheck.IsNull(Me)) return false;
            if (!Me.BlockDefinition.SubtypeId.Contains(sID)) return false;
            return Me?.GameLogic?.GetAs<OverclockingControlBlock>() != null;
        }
        public sealed override void Close()
        {
            base.Close();
            Overclock.Clear(Block.CubeGrid);
            Block.AppendingCustomInfo -= Block_AppendingCustomInfo;//事件:方块终端右下角信息更新
            MyAPIGateway.TerminalControls.CustomControlGetter -= AdjustTerminalControls_controls;//事件: 显示终端前更新终端
            MyAPIGateway.TerminalControls.CustomActionGetter -= AdjustTerminalControls_Action;//事件: 显示G键功能前更新动作
            Block.CubeGrid.OnBlockAdded -= CubeGrid_OnBlockAdded;//事件: 网格添加方块
            Block.CubeGrid.OnGridMerge -= CubeGrid_OnGridMerge;//事件: 网格合并
            Block.CubeGrid.OnGridSplit -= CubeGrid_OnGridSplit;//事件: 网格拆分
        }
    }
    
}