using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ViELearn.DAL.ModulesDAL;

namespace ViELearn.BackEnd
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected static SelectList lstLvlMenu;
        protected static SelectList lstYesOrNo;
        protected void Application_Start()
        {	
            AreaRegistration.RegisterAllAreas();
            CreatGobalValues();            
            GlobalFilters.Filters.Add(new SetDefaultSysParameters(), 0);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelMetadataProviders.Current = new CustomModelMetadataProvider();

            var binder = new UTCDateTimeModelBinder();
            ModelBinders.Binders.Add(typeof(DateTime), binder);
            ModelBinders.Binders.Add(typeof(DateTime?), binder);
        }

        public void CreatGobalValues()
        {
            SystemParametersDAL ctrlSP = new SystemParametersDAL();

            lstLvlMenu = new SelectList(ctrlSP.GetLstByCode("lvlMenu"),"Value", "Name");

            lstYesOrNo = new SelectList(ctrlSP.GetLstByCode("YesOrNo"), "Value", "Name");
        }
        public class SetDefaultSysParameters : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                filterContext.Controller.ViewBag.lstLvlMenu = lstLvlMenu;
                filterContext.Controller.ViewBag.lstYesOrNo = lstYesOrNo;
            }
        }

        public interface IFilteredModelBinder : IModelBinder
        {
            bool IsMatch(Type modelType);
        }
        public class SmartBinder : DefaultModelBinder
        {
            private readonly IFilteredModelBinder[] _filteredModelBinders;

            public SmartBinder(IFilteredModelBinder[] filteredModelBinders)
            {
                _filteredModelBinders = filteredModelBinders;
            }

            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                foreach (var filteredModelBinder in _filteredModelBinders)
                {
                    if (filteredModelBinder.IsMatch(bindingContext.ModelType))
                    {
                        return filteredModelBinder.BindModel(controllerContext, bindingContext);
                    }
                }

                return base.BindModel(controllerContext, bindingContext);
            }
        }

        public class UTCDateTimeModelBinder : DefaultModelBinder
        {
            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                
                if (value.AttemptedValue != null && value.AttemptedValue != "")
                {                             
                    var dt = new DateTime();
                    try
                    {
                        string[] formatsDate = { "dd/MM/yyyy", "dd/MM/yy", "dd/MM/yyyy HH:mm", "dd/MM/yy HH:mm" };
                        if (DateTime.TryParseExact(value.AttemptedValue, formatsDate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
                        {
                            return dt;
                        }
                        else
                        {
                            dt = DateTime.Parse(value.AttemptedValue);
                            return dt.ToLocalTime();
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public class CustomModelMetadataProvider : DataAnnotationsModelMetadataProvider
        {
            protected override ModelMetadata CreateMetadata(IEnumerable<System.Attribute> attributes, System.Type containerType, System.Func<object> modelAccessor, System.Type modelType, string propertyName)
            {
                var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
                if (string.IsNullOrEmpty(propertyName)) return modelMetadata;

                if (modelType == typeof(String))
                    modelMetadata.ConvertEmptyStringToNull = false;

                return modelMetadata;
            }
        }
    }
}
