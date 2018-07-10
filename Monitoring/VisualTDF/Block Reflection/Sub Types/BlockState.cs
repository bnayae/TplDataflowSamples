using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    /// <summary>
    /// Execution state
    /// </summary>
    public enum BlockState
    {
        /// <summary>
        /// Active (running)
        /// </summary>
        Active,
        /// <summary>
        /// Exit with cancellation
        /// </summary>
        Canceled,
        /// <summary>
        /// Complete with errors
        /// </summary>
        Faulted,
        /// <summary>
        /// Completed successfully
        /// </summary>
        Completed
    }
}
