namespace BuYanMod.OverclockingControl.Licalization
{
    using Sandbox.ModAPI;
    using VRage;
    /// <summary>
    /// 模组本地化文本储存
    /// </summary>
    public class ModLicalizationText {
        public string Info;
        public string Overclock;
        public string Power;
        public string Times;
        public string Reactor;
        public string GasGenerator;
        public string Gyro;
        public string Thrust;
        public string Drill;
        public ModLicalizationText() {
            MyLanguagesEnum myLanguagesEnum = MyAPIGateway.Session.Config.Language;
            if (myLanguagesEnum.ToString().Equals("ChineseChina"))
            {
                Info = "当前超频设置:";
                Overclock = "可超频设备";
                Power = "电力设备";
                Times = "倍";
                Reactor = "反应堆";
                GasGenerator = "氢气/氧气制造机";
                Gyro = "陀螺仪";
                Thrust = "推进器";
                Drill = "钻头";
            }
            else
            {
                Info = "Current Overclocking Settings:";
                Overclock = "Overclock Equipment";
                Power = "Power Equipment";
                Times = "Times";
                Reactor = "Reactor";
                GasGenerator = "Gas Generator";
                Gyro = "Gyro";
                Thrust = "Thrust";
                Drill = "Drill";
            }
        }
    }
}