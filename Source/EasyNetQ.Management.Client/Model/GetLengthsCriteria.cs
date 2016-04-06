namespace EasyNetQ.Management.Client.Model
{
    public class GetLengthsCriteria
    {
        /// <summary>
        /// Create a new object for specifying queue length age and increment
        /// </summary>
        /// <param name="age">Age (in seconds) of oldest sample to return</param>
        /// <param name="increment">Interval (in seconds) between samples</param>
        public GetLengthsCriteria(int age, int increment)
        {
            LengthsAge = age;
            LengthsIncr = increment;
        }
        public int LengthsAge { get; private set; }
        public int LengthsIncr { get; private set; }
    }
}