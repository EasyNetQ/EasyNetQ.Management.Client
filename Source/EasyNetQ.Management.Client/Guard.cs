using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace EasyNetQ.Management.Client
{
    public static class Ensure
    {
        /// <summary>
        /// Ensures that the specified argument is not null.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="argument">The argument.</param>
        [DebuggerStepThrough]
        [ContractAnnotation("halt <= argument:null")]
        public static void ArgumentNotNull(object argument, [InvokerParameterName] string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}