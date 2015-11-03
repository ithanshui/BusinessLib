namespace BusinessLib.BasicAuthentication
{
    public class Session : ISession
    {
        //user data
        public System.String Account { get; set; }
        public System.String Password { get; set; }
        public System.String SecurityCode { get; set; }
        //front clinet
        public System.String Site { get; set; }
        public System.String IP { get; set; }
        //sys set
        public System.String Key { get; set; }
        public System.DateTime Time { get; set; }
        public RoleCompetence RoleCompetence { get; set; }

        public ISession Clone()
        {
            return new Session { Site = Site, Account = Account, Password = Password, IP = IP, Time = Time, SecurityCode = SecurityCode, RoleCompetence = RoleCompetence };
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class SysInfo : ISysInfo
    {
        public string OS { get; set; }

        public System.Collections.Generic.List<string> Mac { get; set; }

        public string Cpu { get; set; }

        public string Memory { get; set; }

        public string Board { get; set; }

        public string Disk { get; set; }

        public string Browser { get; set; }

        public string Sign { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public struct BoarStruct
    {
        public object SerialNumber;
        public object Manufacturer;
        public object Product;
        public object Version;
    }
    public struct OSStruct
    {
        public object Name;
        public object Version;
        public object SystemDirectory;
    }
    public struct DiskStruct
    {
        public object SerialNumber;
        public object Model;
        public object Size;
    }
    public struct CpuStruct
    {
        public object Name;
        public object ProcessorId;
        public object SystemName;
    }
    public struct MemoryStruct
    {
        public object TotalPhysicalMemory;
    }
}