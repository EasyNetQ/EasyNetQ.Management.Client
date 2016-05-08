using System;
using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    public class Node
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Running { get; set; }
        public string OsPid { get; set; }
        public long MemEts { get; set; }
        public long MemBinary { get; set; }
        public long MemProc { get; set; }
        public long MemProcUsed { get; set; }
        public long MemAtom { get; set; }
        public long MemAtomUsed { get; set; }
        public long MemCode { get; set; }
        public string FdUsed { get; set; }
        public int FdTotal { get; set; }
        public int SocketsUsed { get; set; }
        public int SocketsTotal { get; set; }
        public long MemUsed { get; set; }
        public long MemLimit { get; set; }
        public bool MemAlarm { get; set; }
        public long DiskFreeLimit { get; set; }
        public long DiskFree { get; set; }
        public bool DiskFreeAlarm { get; set; }
        public int ProcUsed { get; set; }
        public int ProcTotal { get; set; }
        public string StatisticsLevel { get; set; }
        public string ErlangVersion { get; set; }
        public Int64 Uptime { get; set; }
        public int RunQueue { get; set; }
        public int Processors { get; set; }
        public List<ExchangeType> ExchangeTypes { get; set; }
        public List<AuthMechanism> AuthMechanisms { get; set; }
        public List<Application> Applications { get; set; }
        public List<string> Partitions { get; set; }
    }
}