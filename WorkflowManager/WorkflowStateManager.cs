using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace WorkflowManager
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
        
    public class ValidationError
    {
        public string Message { get; set; }
    }

    public class ValidationBase
    {
        private List<ValidationError> validationErrors = new List<ValidationError>();
        public List<ValidationError> ValidationErrors
        {
            get { return this.validationErrors; }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////
    
    public abstract class WorkflowStateManager<S,W> 
        where S : struct, IComparable
        where W : IWorkflow<S>
    {
        protected W Workflow { get; private set; }
        protected ViewDataDictionary ViewDataDictionary { get; private set; }
        protected TempDataDictionary TempDataDictionary { get; private set; }

        protected WorkflowStateManager(W workflow, ViewDataDictionary viewData,
            TempDataDictionary tempData = null)
        {
            this.Workflow = workflow;
            this.ViewDataDictionary = viewData;
            this.TempDataDictionary = tempData;
        }

        public abstract S GetNextState();
        public virtual void AddViewData() {}
        public virtual ActionResult GetActionResult(HttpRequestBase request) { return null; }
        public virtual void Process(NameValueCollection form, ModelBinderHelper mbh) {}

        public virtual void Require() {}
        public virtual void Ensure() {}

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
    
        #region validation
        protected List<ValidationError> validationErrors = null;
        public List<ValidationError> ValidationErrors
        {
            get
            {
                if (this.validationErrors == null)
                {
                    this.validationErrors = new List<ValidationError>();
                }

                return this.validationErrors;
            }

            protected set { this.validationErrors = value; }
        }

        protected bool HasNoValidationErrors
        {
            get { return this.ValidationErrors.Count == 0; }
        }

        protected void AddValidationError(string errorMessage)
        {
            this.ValidationErrors.Add(new ValidationError(errorMessage));
        }

        protected void Validate(ValidationBase validationBase)
        {
            if (this.ValidationErrors == null)
            {
                this.ValidationErrors = new List<ValidationError>();
            }

            this.ValidationErrors.AddRange(validationBase.ValidationErrors);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
        
        #region utils
        protected ActionResult View<TView>() where TView : CodeOnlyView
        {
            return new CodeOnlyResult(typeof(TView)) { ViewData = ViewDataDictionary };
        }

        protected ActionResult Redirect(ActionLink actionLink)
        {
            return new RedirectResult(actionLink.GetUrl());
        }

        public virtual WebContext HttpContext
        {
            get { return WebContext.Current; }
        }
        #endregion
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////

