﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace OrionMvc.Web
{
    public class Controller : IController
    {
        private string _name = null;
        private Dictionary<string, MethodInfo> _actions;
        private Dictionary<string, string> _actionLayouts;


        public Controller()
        {
            ViewBag = new ViewData();
            _actions = GetActionsList();
            _actionLayouts = new Dictionary<string, string>();

        }

        public HttpContext Context
        {
            get;
            set;
        }



        public new dynamic ViewBag
        {
            set;
            get;
        }


        public HttpRequest Request
        {
            get;
            set;
        }

        public object this[string key]
        {
            get
            {
                return ViewBag[key];
            }
            set
            {
                ViewBag[key] = value;
            }
        }


        public void Render(HttpContext context, string View_)
        {

            context.Response.ContentType = "text/html";

            var h = Application.Instance.View.Render(context, this, View_);

            context.Response.Write(h);
            context.Response.End();
        }

        public object View(string ViewName)
        {
            var h = Application.Instance.View.Render(this.Context, this, ViewName);
            return h;
        }

        //public View View(string ViewName,object Model)
        //{

        //}

        //public View View(string ViewName,string MasterPage)
        //{

        //}






        public void Execute(HttpContext context, RouteMeta routeData)
        {
            var actionName = routeData.Action;
            var action = FindAction(actionName);

            actionName = action.Name;

            InvokeAction(action);


            var html = string.Empty;

            foreach (DictionaryEntry listOfparams in ViewBag)
            {
                html += "Params" + listOfparams.Key + "----" + "value" + listOfparams.Value;
            }
            Render(context, actionName);
        }


        public MethodInfo FindAction(string name)
        {
            name = name.ToLower();
            return _actions.ContainsKey(name) ? _actions[name] : null;
        }

        public bool InvokeAction(MethodInfo action)
        {
            action.Invoke(this, new object[] { });
            return true;
        }

        public Dictionary<string, MethodInfo> GetActionsList()
        {
            var flags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            var allMethods = GetType().GetMethods(flags);
            var baseMethods = from method in typeof(Controller).GetMethods(flags)
                              select method.Name;
            var actionMethods = from method in allMethods
                                where baseMethods.Contains(method.Name) == false && method.MemberType == MemberTypes.Method
                                select method;

            return actionMethods.ToDictionary(m => m.Name.ToLower());
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = GetType().Name;
                }

                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
