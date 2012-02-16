============================
ASP.NET MVC Workflow Manager
============================

A simple framework for implementing work flows in ASP.NET MVC web applications.
Please note, that this project is still a work in progress. For usage follow 
the code snippet:


```C#
public ActionResult Default (Nullable<int> bufferId)
{
    var workflowManager = new MyWorkflowManager (
        this.GetWorkflow (bufferId),
        this.ModelBinderHelper,
        this.AddValidationError,
        this.ViewData
    );
    
    var actionResult = workflowManager.Run (this.Request);
    if (actionResult is RedirectResult != true)
    {
        this.Serialize (this.GetWorkflow (bufferId));
    } 
    else 
    {
        this.DeleteBuffer (bufferId);
    }
    
    return actionResult;
}

private MyWorkflow GetWorkflow (Nullable<int> bufferId)
{
    // Returns a work flow stored in *buffer* which is reachable by *bufferId*.
    // If latter is null then a new work flow is initialized. The buffer can be
    // implemented as a database table, a *memcached* entry or simply a hidden
    // field in the HTML markup.
}

private void Serialize (MyWorkflow workflow, Nullable<int> bufferId)
{
    // Serializes *workflow* and stores the result in *buffer* reachable by
    // *bufferId*.
}

private void DeleteBuffer (Nullable<int> bufferId)
{
    // Frees buffer resources (if possible) since work flow seems to have
    // reached an exit point.
}
```

Any controller can define such an action-result with an *arbitrary* name (the 
*Default* name is just one possibility). *MyWorkflowManager* defines internally
a (desired) state machine, which is run on every invocation of *Default*. The 
corresponding result defines the next view.

For more information look at the (slightly outdated!) presentation (PDF or ODP)
in the *Documentation* folder, but be aware that some of the class names have 
changed in the implementation but were not updated in the documentation.

