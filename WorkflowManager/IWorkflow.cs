using System;

namespace WorkflowManager
{
    public interface IWorkflow<S> where S : struct, IComparable
    {
        S State { get; set; }
    }
}
