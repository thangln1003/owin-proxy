using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace AngularDemo.SpaServices
{
    public class MvcRouteHandler : IRouteHandler
    {
        private readonly IControllerFactory _controllerFactory;

        public MvcRouteHandler()
        {
        }

        public MvcRouteHandler(IControllerFactory controllerFactory)
        {
            _controllerFactory = controllerFactory;
        }

        protected virtual IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            requestContext.HttpContext.SetSessionStateBehavior(
                GetSessionStateBehavior(requestContext));
            return new MvcHandler(requestContext);
        }

        protected virtual SessionStateBehavior
            GetSessionStateBehavior(RequestContext requestContext)
        {
            var controllerName = (string)requestContext.RouteData.Values["controller"];
            IControllerFactory controllerFactory = _controllerFactory ?? ControllerBuilder.Current.GetControllerFactory();
            return controllerFactory.GetControllerSessionBehavior
                (requestContext, controllerName);
        }

        #region IRouteHandler Members

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return GetHttpHandler(requestContext);
        }

        #endregion
    }
}