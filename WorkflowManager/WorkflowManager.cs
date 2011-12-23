using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace WorkflowManager
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
        
    public struct ContextInfo
    {
        public DefaultModelBinder DefaultModelBinder;
        public Action<ValidationError> AddValidationError;
        public ViewDataDictionary ViewData;
        public TempDataDictionary TempData;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
        
    public class WorkflowManager<S, W>
        where S : struct, IComparable
        where W : IWorkflow<S>
    {
        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        public Action<W, WorkflowStateManager<S>> OnBeforeProcess { get; set; }
        public Action<W, WorkflowStateManager<S>> OnAfterProcess { get; set; }
        public Action<W, WorkflowStateManager<S>> OnBeforeRequire { get; set; }
        public Action<W, WorkflowStateManager<S>> OnAfterRequire { get; set; }
        public Action<W, WorkflowStateManager<S>> OnBeforeEnsure { get; set; }
        public Action<W, WorkflowStateManager<S>> OnAfterEnsure { get; set; }

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        protected W Workflow { get; private set; }
        protected ContextInfo ContextInfo { get; private set; }

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        public WorkflowManager(
            W workflow,
            DefaultModelBinder dmb,
            Action<ValidationError> addValidationError,
            ViewDataDictionary viewData,
            TempDataDictionary tempData = null
        )
            : this(workflow, new ContextInfo()
            {
                DefaultModelBinder = dmb,
                AddValidationError = addValidationError,
                ViewData = viewData,
                TempData = tempData
            })
        {
            ;
        }

        public WorkflowManager(W workflow, ContextInfo contextInfo)
        {
            this.Workflow = workflow;
            this.ContextInfo = contextInfo;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        public abstract WorkflowStateManager<S> GetWorkflowStateManager();

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        public virtual ActionResult Run(HttpRequestBase request, bool skipProcess = false)
        {
            var stateManager = this.GetWorkflowStateManager();

            if (request.IsPostRequest() && !skipProcess)
            {
                if (OnBeforeProcess != null) 
                    OnBeforeProcess.Invoke(this.Workflow, stateManager);
                stateManager.Process(request.Form, this.ContextInfo.DefaultModelBinder);
                if (OnAfterProcess != null) 
                    OnAfterProcess.Invoke(this.Workflow, stateManager);

                if (stateManager.ValidationErrors.Any())
                {
                    stateManager.ValidationErrors.ForEach(v => 
                        this.ContextInfo.AddValidationError(v)
                    );
                }
                else
                {
                    if (OnBeforeEnsure != null) 
                        OnBeforeEnsure.Invoke(this.Workflow, stateManager);
                    stateManager.Ensure();
                    if (OnAfterEnsure != null) 
                        OnAfterEnsure.Invoke(this.Workflow, stateManager);
                }
            }

            stateManager = this.RunStateMachine(request, stateManager);
            stateManager.AddViewData();

            return stateManager.GetActionResult(request);
        }

        protected virtual WorkflowStateManager<S> RunStateMachine(HttpRequestBase request, 
            WorkflowStateManager<S> stateManager)
        {
            while (true)
            {
                var nextState = stateManager.GetNextState();

                if (IComparable.Equals(nextState, this.Workflow.State) != true)
                {
                    this.Workflow.State = nextState;
                    stateManager = this.GetWorkflowStateManager();
                }
                else
                {
                    if (OnBeforeRequire != null) 
                        OnBeforeRequire.Invoke(this.Workflow, stateManager);
                    stateManager.Require();
                    if (OnAfterRequire != null)
                        OnAfterRequire.Invoke(this.Workflow, stateManager);

                    if (stateManager.GetActionResult(request) != null)
                    {
                        break;
                    }
                    else
                    {
                        if (OnBeforeProcess != null) 
                            OnBeforeProcess.Invoke(this.Workflow, stateManager);
                        stateManager.Process(request.Form, this.ContextInfo.DefaultModelBinder);
                        if (OnAfterProcess != null) 
                            OnAfterProcess.Invoke(this.Workflow, stateManager);

                        if (OnBeforeEnsure != null) 
                            OnBeforeEnsure.Invoke(this.Workflow, stateManager);
                        stateManager.Ensure();
                        if (OnAfterEnsure != null) 
                            OnAfterEnsure.Invoke(this.Workflow, stateManager);
                    }
                }
            }

            return stateManager;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
    }
}
