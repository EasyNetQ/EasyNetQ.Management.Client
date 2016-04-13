namespace EasyNetQ.Management.Client.Model
{
    public class GetRatesCriteria
    {
        /// <summary>
        /// Create a new object for specifying rate age and increment
        /// </summary>
        /// <param name="age">Age (in seconds) of oldest sample to return</param>
        /// <param name="increment">Interval (in seconds) between samples</param>
        public GetRatesCriteria(int age, int increment)
        {
            MsgRatesAge = age;
            MsgRatesIncr = increment;
        }

        public int MsgRatesAge { get; private set; }
        public int MsgRatesIncr { get; private set; }
    }
}