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
using {0}.Models;

namespace {0}.Controllers
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
            var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                        
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
                var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                                    
                return View(returnEntity);
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {    
            var filterEntity = new {1}() { Id = id };
            persistAdapter.Delete(filterEntity);
                                                                                    
            return RedirectToAction(""Index"");
        }

        public ActionResult Enable(int id)
        {
            var filterEntity = new {1}() { Id = id };
            var updatedEntity = persistAdapter.Get(filterEntity, false);
            updatedEntity.Active = true;
            persistAdapter.Edit(updatedEntity, filterEntity, false);

            return RedirectToAction(""Index"");
        }

        public ActionResult Disable(int id)
        {
            var filterEntity = new {1}() { Id = id };
            var updatedEntity = persistAdapter.Get(filterEntity, false);
            updatedEntity.Active = false;
            persistAdapter.Edit(updatedEntity, filterEntity, false);

            return RedirectToAction(""Index"");
        }

        protected override void Dispose(bool disposing)
        {
            persistAdapter = null;
            base.Dispose(disposing);
        }
    }
}";

        public static string TemplateWithExceptionManager = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.RopSql;
using System.Security.InMemProfile;
using {0}.Models;

namespace {0}.Controllers
{ 
    {2}public class {1}Controller : Controller
    {
        private RopSqlDataAdapter persistAdapter = new RopSqlDataAdapter();
        
        public ViewResult Index()
        {
            var filterEntity = new {1}();

            try {

                return View(persistAdapter.List<{1}>(filterEntity, false));
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex);
            }
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
                try {
            
                    persistAdapter.Create(newEntity, false);
                    return RedirectToAction(""Index"");
                }
                catch(Exception ex)
                {
                    new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                    RedirectToAction(""Error"");
                }
            }

            return View(newEntity);
        }

        public ActionResult Edit(int id)
        {
            var filterEntity = new {1}();

            try {

                var returnEntity = persistAdapter.Get(filterEntity, false);
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
                                                                        
            return View(returnEntity);
        }

        [HttpPost]
        public ActionResult Edit({1} editedEntity)
        {
            if (ModelState.IsValid)
            {
                var filterEntity = new {1}() { Id = editedEntity.Id };
                
                try {
                    
                    persistAdapter.Edit(editedEntity, filterEntity, false);
                }
                catch(Exception ex)
                {
                    new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                    RedirectToAction(""Error"");
                }

                return RedirectToAction(""Index"");
            }

            return View(editedEntity);
        }

        public ActionResult Delete(int id)
        {
                var filterEntity = new {1}() { Id = id };
                
                try {
                    
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                }
                catch(Exception ex)
                {
                    new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                    RedirectToAction(""Error"");
                }
                                                                                    
                return View(returnEntity);
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {    
            var filterEntity = new {1}() { Id = id };
            
            try {
                
                persistAdapter.Delete(filterEntity);
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
                                                                                    
            return RedirectToAction(""Index"");
        }

        public ActionResult Enable(int id)
        {
            var filterEntity = new {1}() { Id = id };
            
            try {
            
                var updatedEntity = persistAdapter.Get(filterEntity, false);
                updatedEntity.Active = true;
                persistAdapter.Edit(updatedEntity, filterEntity, false);
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
            
            return RedirectToAction(""Index"");
        }

        public ActionResult Disable(int id)
        {
            var filterEntity = new {1}() { Id = id };
            
            try {
                
                var updatedEntity = persistAdapter.Get(filterEntity, false);
                updatedEntity.Active = false;
                persistAdapter.Edit(updatedEntity, filterEntity, false);
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
            
            return RedirectToAction(""Index"");
        }

        protected override void Dispose(bool disposing)
        {
            persistAdapter = null;
            base.Dispose(disposing);
        }
    }
}";

        public static string TemplateWith_ExcepMan_AndAccessControl = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.RopSql;
using System.Security.InMemProfile;
using {0}.Models;

namespace {0}.Controllers
{ 
    {2}public class {1}Controller : Controller
    {
        private RopSqlDataAdapter persistAdapter = new RopSqlDataAdapter();
        
        public ViewResult Index()
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
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
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                HttpContext.Response.Redirect(""Error"");
            }
        }

        public ActionResult Create()
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
                    return View();
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost]
        public ActionResult Create({1} newEntity)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
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
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Edit(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                        
                    return View(returnEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost]
        public ActionResult Edit({1} editedEntity)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
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
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }

            return View(editedEntity);
        }

        public ActionResult Delete(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                                    
                    return View(returnEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {
            try {    

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    persistAdapter.Delete(filterEntity);
                                                                                    
                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Enable(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var updatedEntity = persistAdapter.Get(filterEntity, false);
                    updatedEntity.Active = true;
                    persistAdapter.Edit(updatedEntity, filterEntity, false);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Disable(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var updatedEntity = persistAdapter.Get(filterEntity, false);
                    updatedEntity.Active = false;
                    persistAdapter.Edit(updatedEntity, filterEntity, false);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        protected override void Dispose(bool disposing)
        {
            persistAdapter = null;
            base.Dispose(disposing);
        }
    }
}";

        public static string TemplateWithExMn_AcCtl_AndRegistry = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.RopSql;
using System.Security.InMemProfile;
using {0}.Models;

namespace {0}.Controllers
{ 
    {2}public class {1}Controller : Controller
    {
        private RopSqlDataAdapter persistAdapter = new RopSqlDataAdapter();
        private SystemRegistry sysRegistry = new SystemRegistry();
        
        public ViewResult Index()
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_List, currentUser.Profilekey))
                {
                    var filterEntity = new {1}();
                    sysRegistry.RegisterRead(filterEntity);

                    return View(persistAdapter.List<{1}>(filterEntity, false));
                }
                else
                {
                    HttpContext.Response.Redirect(""AccessDenied"");
                    return null;
                }
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Create()
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
                    return View();
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost]
        public ActionResult Create({1} newEntity)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
                {
                    if (ModelState.IsValid)
                    {
                        persistAdapter.Create(newEntity, false);
                        sysRegistry.RegisterCreate(newEntity);
                        return RedirectToAction(""Index"");
                    }

                    return View(newEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Edit(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}();
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                        
                    return View(returnEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost]
        public ActionResult Edit({1} editedEntity)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    if (ModelState.IsValid)
                    {
                        var filterEntity = new {1}() { Id = editedEntity.Id };
                        persistAdapter.Edit(editedEntity, filterEntity, false);
                        sysRegistry.RegisterEdit(editedEntity);

                        return RedirectToAction(""Index"");
                    }
                }
                else
                    return RedirectToAction(""AccessDenied"");
        }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }

            return View(editedEntity);
        }

        public ActionResult Delete(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                                    
                    return View(returnEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");                
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    persistAdapter.Delete(filterEntity);
                    sysRegistry.RegisterDelete(filterEntity);
                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Enable(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var updatedEntity = persistAdapter.Get(filterEntity, false);
                    updatedEntity.Active = true;
                    persistAdapter.Edit(updatedEntity, filterEntity, false);
                    sysRegistry.RegisterEdit(updatedEntity);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Disable(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var updatedEntity = persistAdapter.Get(filterEntity, false);
                    updatedEntity.Active = false;
                    persistAdapter.Edit(updatedEntity, filterEntity, false);
                    sysRegistry.RegisterEdit(updatedEntity);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        protected override void Dispose(bool disposing)
        {
            persistAdapter = null;
            base.Dispose(disposing);
        }
    }
}";

        public static string TemplateWithExMn_AcCt_Reg_AndWorkFlow = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.RopSql;
using System.Security.InMemProfile;
using {0}.Models;

namespace {0}.Controllers
{ 
    {2}public class {1}Controller : Controller
    {
        private RopSqlDataAdapter persistAdapter = new RopSqlDataAdapter();
        private SystemRegistry sysRegistry = new SystemRegistry() { EntityHash = EntityHashRelation.{1} };
        private WorkFlow workFlow = new WorkFlow() { EntityHash = EntityHashRelation.{1} };
        
        public ViewResult Index()
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_List, currentUser.Profilekey))
                {
                    var filterEntity = new {1}();
                    sysRegistry.RegisterRead(filterEntity);

                    return View(persistAdapter.List<{1}>(filterEntity, false));
                }
                else
                {
                    HttpContext.Response.Redirect(""AccessDenied"");
                    return null;
                }
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Create()
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
                    return View();
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost]
        public ActionResult Create({1} newEntity)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Create, currentUser.Profilekey))
                {
                    if (ModelState.IsValid)
                    {
                        persistAdapter.Create(newEntity, false);
                        sysRegistry.RegisterCreate(newEntity);

                        var workFlowItem = null;
                        if (workFlow.CheckCondition(newEntity))
                            workFlowItem = workFlow.PutInApproval(newEntity);
                        sysRegistry.RegisterCreate(workFlowItem);

                        return RedirectToAction(""Index"");
                    }

                    return View(newEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Edit(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}();
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                        
                    return View(returnEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost]
        public ActionResult Edit({1} editedEntity)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");            
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    if (ModelState.IsValid)
                    {
                        var filterEntity = new {1}() { Id = editedEntity.Id };
                        persistAdapter.Edit(editedEntity, filterEntity, false);
                        sysRegistry.RegisterEdit(editedEntity);

                        var workFlowItem = null;
                        if (workFlow.CheckCondition(editedEntity))
                            workFlowItem = workFlow.PutInApproval(newEntity);
                        sysRegistry.RegisterCreate(workFlowItem);

                        return RedirectToAction(""Index"");
                    }
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }

            return View(editedEntity);
        }

        public ActionResult Delete(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var returnEntity = persistAdapter.Get(filterEntity, false);
                                                                                    
                    return View(returnEntity);
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        [HttpPost, ActionName(""Delete"")]
        public ActionResult DeleteConfirmed(int id)
        {
            try {
    
                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Delete, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    persistAdapter.Delete(filterEntity);
                    sysRegistry.RegisterDelete(filterEntity);

                    var workFlowItem = null;
                    if (workFlow.CheckCondition(filterEntity))
                        workFlowItem = workFlow.PutInApproval(filterEntity);
                    sysRegistry.RegisterCreate(workFlowItem);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Enable(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var updatedEntity = persistAdapter.Get(filterEntity, false);
                    updatedEntity.Active = true;
                    persistAdapter.Edit(updatedEntity, filterEntity, false);
                    sysRegistry.RegisterEdit(updatedEntity);

                    var workFlowItem = null;
                    if (workFlow.CheckCondition(updatedEntity))
                        workFlowItem = workFlow.PutInApproval(updatedEntity);
                    sysRegistry.RegisterCreate(workFlowItem);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
            }
        }

        public ActionResult Disable(int id)
        {
            try {

                if (Session.Count() == 0) RedirectToAction(""SessionExpired"");
                var currentUser = Session[EntityHashRelation.User.ToString()] as User;

                if ((currentUser != null) 
                     && AccessValidator.CheckPermission(EntityAccessProfile.{1}_Edit, currentUser.Profilekey))
                {
                    var filterEntity = new {1}() { Id = id };
                    var updatedEntity = persistAdapter.Get(filterEntity, false);
                    updatedEntity.Active = false;
                    persistAdapter.Edit(updatedEntity, filterEntity, false);
                    sysRegistry.RegisterEdit(updatedEntity);

                    var workFlowItem = null;
                    if (workFlow.CheckCondition(updatedEntity))
                        workFlowItem = workFlow.PutInApproval(updatedEntity);
                    sysRegistry.RegisterCreate(workFlowItem);

                    return RedirectToAction(""Index"");
                }
                else
                    return RedirectToAction(""AccessDenied"");
            }
            catch(Exception ex)
            {
                new ExceptionManager().RegisterException(user, EntityHashRelation.{1}, ex, {3});
                RedirectToAction(""Error"");
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
