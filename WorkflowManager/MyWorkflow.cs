using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;

using WorkflowManager;

namespace MyWorkflowManager
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
    
    public enum MyWorkflowState : int
    {
        Unspecified = WorkflowState.Unspecified,
        Redirect = WorkflowState.Redirect,
        Red, Green, Blue
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
    
    public class MyWorkflow : IWorkflow<MyWorkflowState>
    {
        private MyWorkflowState state = MyWorkflowState.Unspecified;
        public MyWorkflowState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        #region workflow payload
        // ...
        #endregion
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
    
    public class RedStateManager : WorkflowStateManager<MyWorkflowState, MyWorkflow>
    {
        public RedStateManager(MyWorkflow workflow, ViewDataDictionary viewData) 
            : base(workflow, viewData) {}

        #region workflow state manager members
        public override MyWorkflowState GetNextState() { throw new NotImplementedException(); }
        public override void AddViewData() { throw new NotImplementedException(); }
        public override ActionResult GetActionResult(HttpRequestBase request) 
        { throw new NotImplementedException(); }
        public override void Process(NameValueCollection form, DefaultModelBinder dmb) 
        { throw new NotImplementedException(); }
        #endregion
    }
    
    public class GreenStateManager : WorkflowStateManager<MyWorkflowState, MyWorkflow>
    {
        public GreenStateManager(MyWorkflow workflow, ViewDataDictionary viewData) 
            : base(workflow, viewData) {}

        #region workflow state manager members
        public override MyWorkflowState GetNextState() { throw new NotImplementedException(); }
        public override void AddViewData() { throw new NotImplementedException(); }
        public override ActionResult GetActionResult(HttpRequestBase request) 
        { throw new NotImplementedException(); }
        public override void Process(NameValueCollection form, DefaultModelBinder dmb) 
        { throw new NotImplementedException(); }
        #endregion
    }

    public class BlueStateManager : WorkflowStateManager<MyWorkflowState, MyWorkflow>
    {
        public BlueStateManager(MyWorkflow workflow, ViewDataDictionary viewData) 
            : base(workflow, viewData) {}

        #region workflow state manager members
        public override MyWorkflowState GetNextState() { throw new NotImplementedException(); }
        public override void AddViewData() { throw new NotImplementedException(); }
        public override ActionResult GetActionResult(HttpRequestBase request)
        { throw new NotImplementedException(); }
        public override void Process(NameValueCollection form, DefaultModelBinder dmb)
        { throw new NotImplementedException(); }
        #endregion
    }

    public class RedirectStateManager : WorkflowStateManager<MyWorkflowState, MyWorkflow>
    {
        public RedirectStateManager(MyWorkflow workflow, ViewDataDictionary viewData) 
            : base(workflow, viewData) {}

        #region workflow state manager members
        public override MyWorkflowState GetNextState() { throw new NotImplementedException(); }
        public override void AddViewData() { throw new NotImplementedException(); }
        public override ActionResult GetActionResult(HttpRequestBase request)
        { return new RedirectResult("/url/to/controller/action"); }
        public override void Process(NameValueCollection form, DefaultModelBinder dmb)
        { throw new NotImplementedException(); }
        #endregion
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////

    public class MyWorkflowManager : WorkflowManager<MyWorkflowState, MyWorkflow>
    {
        public MyWorkflowManager(MyWorkflow workflow, ContextInfo contextInfo)
            : base(workflow, contextInfo)
        {
            ;
        }
        
        public override WorkflowStateManager<S> GetWorkflowStateManager()
        {
            switch (this.Workflow.State)
            {
                case MyWorkflowState.Red:
                    return new RedStateManager(this.Workflow, this.ContextInfo.ViewData);
                case MyWorkflowState.Green:
                    return new GreenStateManager(this.Workflow, this.ContextInfo.ViewData);
                case MyWorkflowState.Blue:
                    return new BlueStateManager(this.Workflow, this.ContextInfo.ViewData);
                case MyWorkflowState.Redirect:
                    return new RedirectStateManager(this.Workflow, this.ContextInfo.ViewData);
                default:
                    throw new NotSupportedException(this.Workflow.State.ToString());
            }
        }
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
}
