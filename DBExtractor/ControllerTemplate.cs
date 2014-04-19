using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extraction
{
    public static class ControllerTemplate
    {
        public static string TemplateDefault = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.RopSql;
using System.Security.InMemProfile;

namespace {0}
{ 
    {2}public class {1}Controller : Controller
    {
        private RopSqlDataAdapter persistAdapter = new RopSqlDataAdapter();
        
        public ViewResult Index()
        {
            var filterEntity = new {1}();
            return View(persistAdapter.List<{1}>(filterEntity, false));
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create({1} newEntity)
        {
            if (ModelState.IsValid)
            {
                persistAdapter.Create(newEntity, false);
                return RedirectToAction(""Index"");
            }

            return View(newEntity);
        }

        public ActionResult Edit(int id)
        {
            var filterEntity = new {1}();
            var returnEntity = persistAdapter.Get<{1}>(filterEntity, false);
                                                                        
            return View(returnEntity);
        }

        [HttpPost]
        public ActionResult Edit({1} editedEntity)
        {
            if (ModelState.IsValid)
            {
                var filterEntity = new {1}() { Id = editedEntity.Id };
                persistAdapter.Edit(editedEntity, filterEntity, false);

                return RedirectToAction(""Index"");
            }

            return View(editedEntity);
        }

        public ActionResult Delete(int id)
        {
                var filterEntity = new {1}() { Id = id };
                var returnEntity = persistAdapter.Get<{1}>(filterEntity, false);
                                                                                    
                return View(returnEntity);
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {    
            var filterEntity = new {1}() { Id = id };
            persistAdapter.Delete(filterEntity);
                                                                                    
            return RedirectToAction(""Index"");
        }

        protected override void Dispose(bool disposing)
        {
            persistAdapter = null;
            base.Dispose(disposing);
        }
    }
}";

        public static string TemplateWithAccessControl = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.RopSql;
using System.Security.InMemProfile;

namespace {0}
{ 
    {2}public class {1}Controller : Controller
    {
        private RopSqlDataAdapter persistAdapter = new RopSqlDataAdapter();
        
        public ViewResult Index()
        {
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_List, currentUser.Profilekey))
            {
                var filterEntity = new {1}();
                return View(persistAdapter.List<{1}>(filterEntity, false));
            }
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }
        }

        public ActionResult Create()
        {
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
                return View();
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }
        } 

        [HttpPost]
        public ActionResult Create({1} newEntity)
        {
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
            {
                if (ModelState.IsValid)
                {
                    persistAdapter.Create(newEntity, false);
                    return RedirectToAction(""Index"");
                }

                return View(newEntity);
            }
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }
        }

        public ActionResult Edit(int id)
        {
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
            {
                var filterEntity = new {1}();
                var returnEntity = persistAdapter.Get<{1}>(filterEntity, false);
                                                                        
                return View(returnEntity);
            }
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }
        }

        [HttpPost]
        public ActionResult Edit({1} editedEntity)
        {
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
            {
                if (ModelState.IsValid)
                {
                    var filterEntity = new {1}() { Id = editedEntity.Id };
                    persistAdapter.Edit(editedEntity, filterEntity, false);

                    return RedirectToAction(""Index"");
                }
            }
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }

            return View(editedEntity);
        }

        public ActionResult Delete(int id)
        {
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
            {
                var filterEntity = new {1}() { Id = id };
                var returnEntity = persistAdapter.Get<{1}>(filterEntity, false);
                                                                                    
                return View(returnEntity);
            }
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {    
            var currentUser = Session[EntityHashRelation.User.ToString()] as User;

            if ((currentUser != null) 
                 && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
            {
                var filterEntity = new {1}() { Id = id };
                persistAdapter.Delete(filterEntity);
                                                                                    
                return RedirectToAction(""Index"");
            }
            else
            {
                HttpContext.Response.Redirect(""AccessDenied"");
                return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            persistAdapter = null;
            base.Dispose(disposing);
        }
    }
}";
    }
}
